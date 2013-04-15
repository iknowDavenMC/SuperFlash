using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StreakerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace COMP476Proj
{
    public enum DumbCopState { STATIC, WANDER, PATH, SEEK, FALL, GET_UP, HIT };
    public enum DumbCopBehavior { DEFAULT, AWARE, KNOCKEDUP };

    public class DumbCop : NPC
    {
        #region Attributes

        Node startNode;
        Node endNode;
        Node targetNode;
        private DumbCopState state;
        private DumbCopBehavior behavior;
        private DumbCopState defaultState;
        private const int HIT_DISTANCE_X = 40;
        private const int HIT_DISTANCE_Y = 15;
        #endregion

        #region Constructors
        public DumbCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, DumbCopState pState)
        {
            detectRadius = 400;
            movement = move;
            physics = phys;
            this.draw = draw;
            behavior = DumbCopBehavior.DEFAULT;
            state = defaultState = pState;
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }

        public DumbCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, DumbCopState pState,
            float radius)
        {
            movement = move;
            physics = phys;
            this.draw = draw;
            state = defaultState = pState;
            detectRadius = radius;
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }
        #endregion

        #region Private Methods
        private void transitionToState(DumbCopState pState)
        {
            if (state == pState)
            {
                return;
            }
            switch (pState)
            {
                case DumbCopState.STATIC:
                    behavior = DumbCopBehavior.DEFAULT;
                    state = DumbCopState.STATIC;
                    draw.animation = SpriteDatabase.GetAnimation("cop_static");
                    physics.SetPace(false);
                    draw.Reset();
                    break;
                case DumbCopState.WANDER:
                    behavior = DumbCopBehavior.DEFAULT;
                    state = DumbCopState.WANDER;
                    draw.animation = SpriteDatabase.GetAnimation("cop_walk");
                    physics.SetPace(false);
                    draw.Reset();
                    break;
                case DumbCopState.FALL:
                    behavior = DumbCopBehavior.KNOCKEDUP;
                    state = DumbCopState.FALL;
                    draw.animation = SpriteDatabase.GetAnimation("cop_fall");
                    physics.SetPace(false);
                    draw.Reset();
                    break;
                case DumbCopState.GET_UP:
                    behavior = DumbCopBehavior.KNOCKEDUP;
                    state = DumbCopState.GET_UP;
                    draw.animation = SpriteDatabase.GetAnimation("cop_getup");
                    physics.SetPace(false);
                    draw.Reset();
                    break;
                case DumbCopState.PATH:
                    state = DumbCopState.PATH;
                    draw.animation = SpriteDatabase.GetAnimation("cop_walk");
                    physics.SetPace(false);
                    draw.Reset();
                    break;
                case DumbCopState.HIT:
                    behavior = DumbCopBehavior.AWARE;
                    state = DumbCopState.HIT;
                    draw.animation = SpriteDatabase.GetAnimation("cop_attack");
                    physics.SetPace(false);
                    draw.Reset();
                    break;
                case DumbCopState.SEEK:
                    behavior = DumbCopBehavior.AWARE;
                    state = DumbCopState.SEEK;
                    draw.animation = SpriteDatabase.GetAnimation("cop_walk");
                    physics.SetPace(true);
                    draw.Reset();
                    break;
            }
        }

        public void updateState()
        {
            
            //--------------------------------------------------------------------------
            //        DEFAULT BEHAVIOR TRANSITIONS --> Before aware of streaker
            //--------------------------------------------------------------------------
            if (behavior == DumbCopBehavior.DEFAULT)
            {
                if (Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius && LineOfSight())
                {
                    playSound("Exclamation");
                    behavior = DumbCopBehavior.AWARE;
                    transitionToState(DumbCopState.SEEK);
                }
            }
            //--------------------------------------------------------------------------
            //               AWARE BEHAVIOR TRANSITION --> Knows about streaker
            //--------------------------------------------------------------------------
            else if (behavior == DumbCopBehavior.AWARE)
            {
                if(LineOfSight()){
                    float distance = Vector2.Distance(Game1.world.streaker.Position, pos);
                    if (Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                        Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
                    {
                        transitionToState(DumbCopState.HIT);
                    }
                    else if (distance > detectRadius)
                    {
                        behavior = DumbCopBehavior.DEFAULT;
                        transitionToState(defaultState);
                    }
                    else
                    {
                        transitionToState(DumbCopState.SEEK);
                    }
                }
                else{
                    transitionToState(DumbCopState.SEEK);
                }
            }
            //--------------------------------------------------------------------------
            //               COLLIDE BEHAVIOR TRANSITION
            //--------------------------------------------------------------------------
            else if (behavior == DumbCopBehavior.KNOCKEDUP)
            {
                switch (state)
                {
                    case DumbCopState.FALL:
                        if (draw.animComplete)
                        {
                            SoundManager.GetInstance().PlaySound("Common", "Fall", Game1.world.streaker.Position, Position);
                            transitionToState(DumbCopState.GET_UP);
                        }
                        break;
                    case DumbCopState.GET_UP:
                        if (draw.animComplete && Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius)
                        {
                            behavior = DumbCopBehavior.AWARE;
                            transitionToState(DumbCopState.SEEK);
                        }
                        else if (draw.animComplete && Vector2.Distance(Game1.world.streaker.Position, pos) >= detectRadius)
                        {
                            behavior = DumbCopBehavior.DEFAULT;
                            transitionToState(defaultState);
                        }
                        break;
                }
            }

            //--------------------------------------------------------------------------
            //       CHAR STATE --> ACTION
            //--------------------------------------------------------------------------

            switch (state)
            {
                case DumbCopState.STATIC:
                    movement.Stop(ref physics);
                    break;
                case DumbCopState.WANDER:
                    movement.Wander(ref physics);
                    break;
                case DumbCopState.SEEK:
                    movement.SetTarget(Game1.world.streaker.Position);
                    movement.Seek(ref physics);
                    break;
                case DumbCopState.PATH:
                    //TO DO
                    break;
                case DumbCopState.FALL:
                    movement.Stop(ref physics);
                    break;
                case DumbCopState.HIT:
                    movement.Stop(ref physics);
                    break;
                case DumbCopState.GET_UP:
                    movement.Stop(ref physics);
                    break;
                default:
                    break;
            }
        }

        private void playSound(string soundName)
        {
            SoundManager.GetInstance().PlaySound("DumbCop", soundName, Game1.world.streaker.Position, Position);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Update
        /// </summary>
        public void Update(GameTime gameTime, World w)
        {
            updateState();
            movement.Look(ref physics);
            physics.UpdatePosition(gameTime.ElapsedGameTime.TotalSeconds, out pos);
            physics.UpdateOrientationInstant(gameTime.ElapsedGameTime.TotalSeconds);
            if (physics.Orientation > 0)
            {
                draw.SpriteEffect = SpriteEffects.None;
            }
            else if (physics.Orientation < 0)
            {
                draw.SpriteEffect = SpriteEffects.FlipHorizontally;
            }

            draw.Update(gameTime, this);
            if (draw.animComplete && (state == DumbCopState.FALL || state == DumbCopState.GET_UP))
            {
                draw.GoToPrevFrame();
            }
            if (draw.animComplete && state == DumbCopState.HIT &&
                Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
            {
                //draw.GoToPrevFrame();
                Game1.world.streaker.GetHit();
                
            }
            base.Update(gameTime);

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            draw.Draw(gameTime, spriteBatch, physics.Position);
            base.Draw(gameTime, spriteBatch);
        }

        public override void Fall(bool isSuperFlash)
        {
            
            if (state != DumbCopState.FALL && isSuperFlash)
            {
                behavior = DumbCopBehavior.KNOCKEDUP;
                playSound("SuperFlash");
                transitionToState(DumbCopState.FALL);
            }
            movement.Stop(ref physics);
        }
        #endregion

    }
}
