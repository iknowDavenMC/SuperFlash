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
                    draw.Reset();
                    break;
                case PedestrianState.WANDER:
                    state = PedestrianState.WANDER;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_walk");
                    draw.Reset();
                    break;
                case PedestrianState.FLEE:
                    state = PedestrianState.FLEE;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_walk");
                    draw.Reset();
                    break;
                case PedestrianState.FALL:
                    state = PedestrianState.FALL;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_fall");
                    draw.Reset();
                    break;
                case PedestrianState.GET_UP:
                    state = PedestrianState.GET_UP;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_getup");
                    draw.Reset();
                    break;
                case PedestrianState.PATH:
                    state = PedestrianState.PATH;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_walk");
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
                if (isColliding)
                {
                    isColliding = false;
                    behavior = PedestrianBehavior.KNOCKEDUP;
                    transitionToState(PedestrianState.FALL);
                }
                else if (Vector2.Distance(w.streaker.Position, pos) < detectRadius)
                {
                    behavior = PedestrianBehavior.AWARE;
                    transitionToState(PedestrianState.FLEE);
                }
            }
            //--------------------------------------------------------------------------
            //               AWARE BEHAVIOR TRANSITION --> Knows about streaker
            //--------------------------------------------------------------------------
            else if (behavior == PedestrianBehavior.AWARE)
            {
                if (isColliding)
                {
                    isColliding = false;
                    behavior = PedestrianBehavior.KNOCKEDUP;
                    return;
                }
                else if (Vector2.Distance(w.streaker.Position, pos) > farRadius)
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
                            transitionToState(PedestrianState.GET_UP);
                        }
                        break;
                    case PedestrianState.GET_UP:
                        if (draw.animComplete)
                        {
                            behavior = PedestrianBehavior.AWARE;
                            transitionToState(PedestrianState.FLEE);
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
                    physics.SetTargetValues(true, null, null, null);
                    break;
                case PedestrianState.WANDER:
                    movement.SetTarget(w.streaker.ComponentPhysics.Position);
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
                    break;
                case PedestrianState.GET_UP:
                    break;
                default:
                    
                    break;
            }
            
           

        }

        //private void updateState(World w)
        //{
        //    //--------------------------------------------------------------------------
        //    //               DEFAULT BEHAVIOR --> Before aware of streaker
        //    //--------------------------------------------------------------------------
        //    if (behavior == PedestrianBehavior.DEFAULT)
        //    {
        //        //Behavior Changes
        //        if (isColliding)
        //        {
        //            isColliding = false;
        //            behavior = PedestrianBehavior.COLLIDE;
        //            transitionToState(PedestrianState.FALL);
        //        }
        //        else if (Vector2.Distance(w.streaker.Position, pos) < detectRadius)
        //        {
        //            behavior = PedestrianBehavior.AWARE;
        //            state = PedestrianState.FLEE;
        //            draw.animation = SpriteDatabase.GetAnimation(studentType + "_walk");
        //            draw.Play();
        //        }


        //    }
        //    //--------------------------------------------------------------------------
        //    //               AWARE BEHAVIOR --> Knows about streaker
        //    //--------------------------------------------------------------------------
        //    else if (behavior == PedestrianBehavior.AWARE)
        //    {
        //        //Behavior Changes
        //        if (isColliding)
        //        {
        //            isColliding = false;
        //            behavior = PedestrianBehavior.COLLIDE;
        //            return;
        //        }

        //        switch (state)
        //        {
        //            case PedestrianState.WANDER:
        //                if (Vector2.Distance(w.streaker.Position, pos) < detectRadius)
        //                {
        //                    state = PedestrianState.FLEE;

        //                }
        //                else
        //                {
        //                    movement.Wander(ref physics);
        //                }
        //                break;
        //            case PedestrianState.FLEE:
        //                if (Vector2.Distance(w.streaker.ComponentPhysics.Position, physics.Position) >= detectRadius)
        //                {
        //                    state = PedestrianState.WANDER;
        //                }
        //                else
        //                {
        //                    movement.SetTarget(w.streaker.ComponentPhysics.Position);
        //                    movement.Flee(ref physics);
        //                }
        //                break;
        //            default:
        //                state = PedestrianState.WANDER;
        //                break;
        //        }
        //    }
        //    //--------------------------------------------------------------------------
        //    //                                 COLLIDE BEHAVIOR
        //    //--------------------------------------------------------------------------
        //    else
        //    {
        //        switch (state)
        //        {
        //            case PedestrianState.FALL:
        //                //movement.Stop(ref physics);

        //                if (draw.animComplete)
        //                {
        //                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_getup");
        //                    state = PedestrianState.GET_UP;
        //                    draw.Reset();
        //                }
        //                else
        //                {
        //                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_fall");
        //                    draw.Play();
        //                }

        //                break;
        //            case PedestrianState.GET_UP:
        //                movement.Stop(ref physics);
        //                if (isColliding)
        //                {
        //                    isColliding = false;
        //                    behavior = PedestrianBehavior.COLLIDE;
        //                    state = PedestrianState.GET_UP;
        //                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_getup");
        //                    draw.Reset();
        //                    return;
        //                }
        //                else if (draw.animComplete)
        //                {
        //                    behavior = PedestrianBehavior.AWARE;
        //                    state = PedestrianState.FLEE;
        //                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_walk");
        //                    draw.Reset();
        //                }
        //                else
        //                {
        //                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_getup");
        //                    draw.Play();
        //                }

        //                break;
        //            default:
        //                //state = PedestrianState.FALL;
        //                behavior = PedestrianBehavior.AWARE;
        //                state = PedestrianState.FLEE;
        //                draw.animation = SpriteDatabase.GetAnimation(studentType + "_walk");
        //                draw.Play();
        //                break;
        //        }
        //    }
        //}
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
            base.Update(gameTime);
            
        }

        

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            draw.Draw(gameTime, spriteBatch, physics.Position);
            base.Draw(gameTime, spriteBatch);
        }

        public override void Fall()
        {
            if (state != PedestrianState.FALL)
            {
                draw.Reset();
            }

            state = PedestrianState.FALL;

            physics.SetTargetValues(true, null, null, null);

            return;
        }

        #endregion


    }
}
