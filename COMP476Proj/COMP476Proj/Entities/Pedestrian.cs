#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace COMP476Proj.Entities
{
    public enum PedestrianState { STATIC, WANDER, FLEE, PATH, FALL, GET_UP };
    public enum PedestrianBehavior { DEFAULT, AWARE, COLLIDE };
    public class Pedestrian : NPC
    {
        #region Attributes

        /// <summary>
        /// Direction the character is moving
        /// </summary>
        private Vector2 direction;
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
        }
        #endregion

        #region Public Methods

        public void Update(GameTime gameTime, World w)
        {
            if (behavior == PedestrianBehavior.DEFAULT)
            {
                
                if (rect.Collides(w.streaker.BoundingRectangle))
                {
                    behavior = PedestrianBehavior.COLLIDE;
                    return;
                }

                if(Vector2.Distance(w.streaker.Position, pos) > detectRadius){
                    behavior = PedestrianBehavior.AWARE;
                    return;
                }

                switch (state)
                {
                    case PedestrianState.STATIC:
                        physics.SetTargetValues(true, null, null, null);
                        break;
                    case PedestrianState.WANDER:
                        
                        break;
                    case PedestrianState.PATH:
                        //TO DO
                        break;
                    default:
                        state = PedestrianState.WANDER;
                        break;
                }
            }
            else if (behavior == PedestrianBehavior.AWARE)
            {
                if (rect.Collides(w.streaker.BoundingRectangle))
                {
                    behavior = PedestrianBehavior.COLLIDE;
                    return;
                }

                switch (state)
                {
                    case PedestrianState.WANDER:
                        break;
                    case PedestrianState.FLEE:
                        break;
                    default:
                        state = PedestrianState.WANDER;
                        break;
                }
            }
            else
            {
                switch (state)
                {
                    case PedestrianState.FALL:
                        if (draw.animComplete)
                        {
                            draw.animation = SpriteDatabase.GetAnimation(studentType + "_getup");
                            state = PedestrianState.GET_UP;
                            draw.Reset();
                        }
                        else
                        {
                            draw.animation = SpriteDatabase.GetAnimation(studentType + "_fall");
                            draw.Play();
                        }
                    
                        break;
                    case PedestrianState.GET_UP:
                        if (draw.animComplete)
                        {
                            behavior = PedestrianBehavior.AWARE;
                            state = PedestrianState.WANDER;
                            draw.animation = SpriteDatabase.GetAnimation(studentType + "_walk");
                            draw.Reset();
                        }
                        else
                        {
                            draw.animation = SpriteDatabase.GetAnimation(studentType + "_getup");
                            draw.Play();
                        }

                        break;
                    default:
                        state = PedestrianState.FALL;
                        break;
                }
            }
            /*if (charState != StreakerState.FALL && charState != StreakerState.GET_UP)
            {
                handleUserInput(gameTime);
            }

            physics.UpdatePosition(gameTime.ElapsedGameTime.TotalSeconds);
            physics.UpdateOrientation(gameTime.ElapsedGameTime.TotalSeconds);

            draw.Update(gameTime, this);
            UpdateStates(draw.animComplete);

            */
            //Debugger.getInstance().pointsToDraw.Add(physics.position);
            base.Update(gameTime);
        }

        #endregion


    }
}
