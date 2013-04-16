#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StreakerLibrary;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace COMP476Proj
{
    public enum PedestrianState { STATIC, WANDER, FLEE, PATH, FALL, GET_UP };
    public enum PedestrianBehavior { DEFAULT, AWARE, KNOCKEDUP };
    public class Pedestrian : NPC
    {
        #region Attributes

        /// <summary>
        /// Direction the character is moving
        /// </summary>
        private PedestrianState state;
        private PedestrianBehavior behavior;
        private string studentType;
        #endregion

        #region Constructors
        public Pedestrian(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, PedestrianState pState)
        {
            movement = move;
            physics = phys;
            this.draw = draw;
            state = pState;
            behavior = PedestrianBehavior.DEFAULT;
            studentType = draw.animation.animationId.Substring(0, 8);
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }

        public Pedestrian(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, PedestrianState pState,
            float radius)
        {
            movement = move;
            physics = phys;
            this.draw = draw;
            state = pState;
            behavior = PedestrianBehavior.DEFAULT;
            detectRadius = radius;
            studentType = draw.animation.animationId.Substring(0, 8);
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }
        #endregion
        
        #region Private Methods
        private void transitionToState(PedestrianState pState)
        {
            switch (pState)
            {
                case PedestrianState.STATIC:
                    state = PedestrianState.STATIC;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_static");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case PedestrianState.WANDER:
                    state = PedestrianState.WANDER;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_walk");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case PedestrianState.FLEE:
                    state = PedestrianState.FLEE;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_flee");
                    physics.SetSpeed(true);
                    physics.SetAcceleration(true);
                    draw.Reset();
                    break;
                case PedestrianState.FALL:
                    state = PedestrianState.FALL;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_fall");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case PedestrianState.GET_UP:
                    state = PedestrianState.GET_UP;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_getup");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case PedestrianState.PATH:
                    state = PedestrianState.PATH;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_walk");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
            }
        }

        private void updateState(World w)
        {
            //--------------------------------------------------------------------------
            //        DEFAULT BEHAVIOR TRANSITIONS --> Before aware of streaker
            //--------------------------------------------------------------------------
            if (behavior == PedestrianBehavior.DEFAULT)
            {
                if (Vector2.Distance(w.streaker.Position, pos) < detectRadius && LineOfSight())
                {
                    playSound("Exclamation");
                    behavior = PedestrianBehavior.AWARE;
                    transitionToState(PedestrianState.FLEE);
                }
            }
            //--------------------------------------------------------------------------
            //               AWARE BEHAVIOR TRANSITION --> Knows about streaker
            //--------------------------------------------------------------------------
            else if (behavior == PedestrianBehavior.AWARE)
            {
                if (Vector2.Distance(w.streaker.Position, pos) > detectRadius)
                {
                    behavior = PedestrianBehavior.DEFAULT;
                    transitionToState(PedestrianState.WANDER);
                }
            }
            //--------------------------------------------------------------------------
            //               COLLIDE BEHAVIOR TRANSITION
            //--------------------------------------------------------------------------
            else if (behavior == PedestrianBehavior.KNOCKEDUP)
            {
                switch (state)
                {
                    case PedestrianState.FALL:
                        if (draw.animComplete)
                        {
                            SoundManager.GetInstance().PlaySound("Common", "Fall", w.streaker.Position, Position);
                            transitionToState(PedestrianState.GET_UP);
                        }
                        break;
                    case PedestrianState.GET_UP:
                        if (draw.animComplete && Vector2.Distance(w.streaker.Position, pos) < detectRadius)
                        {
                            behavior = PedestrianBehavior.AWARE;
                            transitionToState(PedestrianState.FLEE);
                        }
                        else if (draw.animComplete && Vector2.Distance(w.streaker.Position, pos) >= detectRadius)
                        {
                            behavior = PedestrianBehavior.DEFAULT;
                            transitionToState(PedestrianState.WANDER);
                        }
                        break;
                }
            }

            //--------------------------------------------------------------------------
            //       CHAR STATE --> ACTION
            //--------------------------------------------------------------------------
            
            switch (state)
            {
                case PedestrianState.STATIC:
                    movement.Stop(ref physics);
                    break;
                case PedestrianState.WANDER:
                    movement.Wander(ref physics);
                    break;
                case PedestrianState.FLEE:
                    movement.SetTarget(w.streaker.ComponentPhysics.Position);
                    movement.Flee(ref physics);
                    break;
                case PedestrianState.PATH:
                    //TO DO
                    break;
                case PedestrianState.FALL:
                    movement.Stop(ref physics);
                    break;
                case PedestrianState.GET_UP:
                    movement.Stop(ref physics);
                    break;
                default:
                    break;
            }
            
        }

        private void playSound(string soundName)
        {
            if (studentType == "student1")
            {
                SoundManager.GetInstance().PlaySound("WhiteBoy", soundName, Game1.world.streaker.Position, Position);
            }
            else if (studentType == "student2")
            {
                SoundManager.GetInstance().PlaySound("BlackBoy", soundName, Game1.world.streaker.Position, Position);
            }
            else if (studentType == "student3")
            {
                SoundManager.GetInstance().PlaySound("Girl", soundName, Game1.world.streaker.Position, Position);
            }
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Update
        /// </summary>
        public void Update(GameTime gameTime, World w)
        {
            updateState(w);
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
            if (draw.animComplete && (state == PedestrianState.FALL || state == PedestrianState.GET_UP))
            {
                draw.GoToPrevFrame();
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
            behavior = PedestrianBehavior.KNOCKEDUP;
            
            if (state != PedestrianState.FALL)
            {
                if (isSuperFlash)
                {
                    playSound("SuperFlash");
                }

                transitionToState(PedestrianState.FALL);
            }
            movement.Stop(ref physics);
        }

        #endregion


    }
}
