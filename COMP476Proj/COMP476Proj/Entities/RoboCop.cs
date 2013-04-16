using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;
namespace COMP476Proj
{
    
    public enum RoboCopState { STATIC, PURSUE, HIT };

    public class RoboCop : NPC
    {
        #region Attributes

        Node startNode;
        Node endNode;
        Node targetNode;
        private RoboCopState state;

        private bool lineOfSight = false;
        private bool withinHitRadius = false;

        private const int HIT_DISTANCE_X = 40;
        private const int HIT_DISTANCE_Y = 15;
        #endregion

        #region Constructors
        public RoboCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw)
        {
            detectRadius = 400;
            movement = move;
            physics = phys;
            this.draw = draw;
            state = RoboCopState.STATIC;
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }

        public RoboCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, RoboCopState pState,
            float radius)
        {
            movement = move;
            physics = phys;
            this.draw = draw;
            state = pState;
            detectRadius = radius;
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }
        #endregion

        #region Private Methods
        private void transitionToState(RoboCopState pState)
        {
            if (state == pState)
            {
                return;
            }
            switch (pState)
            {
                case RoboCopState.STATIC:
                    state = RoboCopState.STATIC;
                    draw.animation = SpriteDatabase.GetAnimation("roboCop_static");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case RoboCopState.HIT:
                    state = RoboCopState.HIT;
                    draw.animation = SpriteDatabase.GetAnimation("roboCop_attack");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case RoboCopState.PURSUE:
                    playSound("Activation");
                    state = RoboCopState.PURSUE;
                    draw.animation = SpriteDatabase.GetAnimation("roboCop_walk");
                    physics.SetSpeed(true);
                    physics.SetAcceleration(true);
                    draw.Reset();
                    break;
            }
        }

        public void updateState()
        {
            lineOfSight = LineOfSight();
            withinHitRadius = Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                              Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y;
            if (state == RoboCopState.STATIC)
            {
                if (Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius && lineOfSight)
                {
                    playSound("Activation");
                    transitionToState(RoboCopState.PURSUE);
                }
            }
            //--------------------------------------------------------------------------
            //               AWARE BEHAVIOR TRANSITION --> Knows about streaker
            //--------------------------------------------------------------------------
            else if (state == RoboCopState.PURSUE)
            {
                float distance = Vector2.Distance(Game1.world.streaker.Position, pos);
                if (withinHitRadius)
                {
                    transitionToState(RoboCopState.HIT);
                }
            }
            //--------------------------------------------------------------------------
            //        STATIC BEHAVIOR TRANSITIONS --> Before aware of streaker
            //-------------------------------------------------------------------------- 
            else if (state == RoboCopState.HIT)
            {
                if (!withinHitRadius)
                {
                    transitionToState(RoboCopState.PURSUE);
                }
            }

            //--------------------------------------------------------------------------
            //       CHAR STATE --> ACTION
            //--------------------------------------------------------------------------

            switch (state)
            {
                case RoboCopState.STATIC:
                    movement.Stop(ref physics);
                    break;
                case RoboCopState.PURSUE:
                    if (lineOfSight)
                    {
                        movement.SetTarget(Game1.world.streaker.Position);
                    }
                    else
                    {

                    }
                    movement.Seek(ref physics);
                    break;
                case RoboCopState.HIT:
                    movement.Stop(ref physics);
                    break;
                default:
                    break;
            }
        }

        private void playSound(string soundName)
        {
            SoundManager.GetInstance().PlaySound("RoboCop", soundName, Game1.world.streaker.Position, Position);
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
            physics.UpdateOrientation(gameTime.ElapsedGameTime.TotalSeconds);
            if (physics.Orientation > 0)
            {
                draw.SpriteEffect = SpriteEffects.None;
            }
            else if (physics.Orientation < 0)
            {
                draw.SpriteEffect = SpriteEffects.FlipHorizontally;
            }

            draw.Update(gameTime);
            if (draw.animComplete && state == RoboCopState.HIT &&
                Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
            {
                Game1.world.streaker.GetHit();
                Game1.world.streaker.ResolveCollision(this);
            }
            base.Update(gameTime);

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            draw.Draw(gameTime, spriteBatch, physics.Position);
            base.Draw(gameTime, spriteBatch);
        }

        #endregion

    }
}
