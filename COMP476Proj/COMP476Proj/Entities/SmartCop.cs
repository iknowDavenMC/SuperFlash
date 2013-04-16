using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;

namespace COMP476Proj
{
    public enum SmartCopState { STATIC, WANDER, PATH, SEEK, FALL, GET_UP, HIT };
    public enum SmartCopBehavior { DEFAULT, AWARE, KNOCKEDUP };

    public class SmartCop : NPC
    {
        #region Attributes
        public static SmartCop closest = null;
        public static float closestDistSq = float.MaxValue;
        Node startNode;
        Node endNode;
        Node targetNode;
        private SmartCopState state;
        private SmartCopBehavior behavior;
        private SmartCopState defaultState;
        private const int HIT_DISTANCE_X = 40;
        private const int HIT_DISTANCE_Y = 15;
        #endregion

        #region Constructors
        public SmartCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, SmartCopState pState)
        {
            detectRadius = 400;
            movement = move;
            physics = phys;
            this.draw = draw;
            behavior = SmartCopBehavior.DEFAULT;
            state = defaultState = pState;
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }

        public SmartCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, SmartCopState pState,
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
        private void transitionToState(SmartCopState pState)
        {
            if (state == pState)
            {
                return;
            }
            switch (pState)
            {
                case SmartCopState.STATIC:
                    behavior = SmartCopBehavior.DEFAULT;
                    state = SmartCopState.STATIC;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_static");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.WANDER:
                    behavior = SmartCopBehavior.DEFAULT;
                    state = SmartCopState.WANDER;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_walk");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.FALL:
                    behavior = SmartCopBehavior.KNOCKEDUP;
                    state = SmartCopState.FALL;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_fall");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.GET_UP:
                    behavior = SmartCopBehavior.KNOCKEDUP;
                    state = SmartCopState.GET_UP;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_getup");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.PATH:
                    state = SmartCopState.PATH;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_walk");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.HIT:
                    behavior = SmartCopBehavior.AWARE;
                    state = SmartCopState.HIT;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_attack");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.SEEK:
                    behavior = SmartCopBehavior.AWARE;
                    state = SmartCopState.SEEK;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_walk");
                    physics.SetSpeed(true);
                    physics.SetAcceleration(true);
                    draw.Reset();
                    break;
            }
        }

        public void updateState()
        {

            //--------------------------------------------------------------------------
            //        DEFAULT BEHAVIOR TRANSITIONS --> Before aware of streaker
            //--------------------------------------------------------------------------
            if (behavior == SmartCopBehavior.DEFAULT)
            {
                if (Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius && LineOfSight())
                {
                    playSound("Exclamation");
                    behavior = SmartCopBehavior.AWARE;
                    transitionToState(SmartCopState.SEEK);
                }
            }
            //--------------------------------------------------------------------------
            //               AWARE BEHAVIOR TRANSITION --> Knows about streaker
            //--------------------------------------------------------------------------
            else if (behavior == SmartCopBehavior.AWARE)
            {
                if (LineOfSight())
                {
                    float distance = Vector2.Distance(Game1.world.streaker.Position, pos);
                    if (Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                        Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
                    {
                        transitionToState(SmartCopState.HIT);
                    }
                    else if (distance > detectRadius)
                    {
                        behavior = SmartCopBehavior.DEFAULT;
                        transitionToState(defaultState);
                    }
                    else
                    {
                        transitionToState(SmartCopState.SEEK);
                    }
                }
                else
                {
                    transitionToState(SmartCopState.SEEK);
                }
            }
            //--------------------------------------------------------------------------
            //               COLLIDE BEHAVIOR TRANSITION
            //--------------------------------------------------------------------------
            else if (behavior == SmartCopBehavior.KNOCKEDUP)
            {
                switch (state)
                {
                    case SmartCopState.FALL:
                        if (draw.animComplete)
                        {
                            SoundManager.GetInstance().PlaySound("Common", "Fall", Game1.world.streaker.Position, Position);
                            transitionToState(SmartCopState.GET_UP);
                        }
                        break;
                    case SmartCopState.GET_UP:
                        if (draw.animComplete && Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius)
                        {
                            behavior = SmartCopBehavior.AWARE;
                            transitionToState(SmartCopState.SEEK);
                        }
                        else if (draw.animComplete && Vector2.Distance(Game1.world.streaker.Position, pos) >= detectRadius)
                        {
                            behavior = SmartCopBehavior.DEFAULT;
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
                case SmartCopState.STATIC:
                    movement.Stop(ref physics);
                    break;
                case SmartCopState.WANDER:
                    movement.Wander(ref physics);
                    break;
                case SmartCopState.SEEK:
                    movement.SetTarget(Game1.world.streaker.Position);
                    movement.SetTargetVelocity(Game1.world.streaker.ComponentPhysics.Velocity);
                    if (closest == this)
                        movement.Seek(ref physics);
                    else
                        movement.Pursue(ref physics);                    
                    break;
                case SmartCopState.PATH:
                    //TO DO
                    break;
                case SmartCopState.FALL:
                    movement.Stop(ref physics);
                    break;
                case SmartCopState.HIT:
                    movement.Stop(ref physics);
                    break;
                case SmartCopState.GET_UP:
                    movement.Stop(ref physics);
                    break;
                default:
                    break;
            }
        }

        private void playSound(string soundName)
        {
            SoundManager.GetInstance().PlaySound("SmartCop", soundName, Game1.world.streaker.Position, Position);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Update
        /// </summary>
        public void Update(GameTime gameTime, World w)
        {
            updateState();

            if (closest == this)
            {
                closestDistSq = (Game1.world.streaker.ComponentPhysics.Position - physics.Position).LengthSquared();
            }
            else
            {
                float distSq = (Game1.world.streaker.ComponentPhysics.Position - physics.Position).LengthSquared();
                if (distSq < closestDistSq)
                {

                }
            }

            movement.Look(ref physics);
            physics.UpdatePosition(gameTime.ElapsedGameTime.TotalSeconds, out pos);
            physics.UpdateOrientation(gameTime.ElapsedGameTime.TotalSeconds);
            if (physics.Orientation > 0)
            {
                draw.SpriteEffect = SpriteEffects.None;
            }
            else if (physics.Orientation < 0)
            {
                draw.SpriteEffect = SpriteEffects.FlipHorizontally;
            }

            draw.Update(gameTime, this);
            if (draw.animComplete && (state == SmartCopState.FALL || state == SmartCopState.GET_UP))
            {
                draw.GoToPrevFrame();
            }
            if (draw.animComplete && state == SmartCopState.HIT &&
                Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
            {
                Game1.world.streaker.GetHit();
                Game1.world.streaker.ResolveCollision(this);
                playSound("Hit");
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
            if (state != SmartCopState.FALL)
            {
                if (isSuperFlash)
                {
                    playSound("SuperFlash");
                }

                behavior = SmartCopBehavior.KNOCKEDUP;
                
                transitionToState(SmartCopState.FALL);

                movement.Stop(ref physics);
            }
        }

        #endregion

    }
}

