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
    public enum PedestrianBehavior { DEFAULT, AWARE, COLLIDE };
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

        #region Public Methods

        public void Update(GameTime gameTime, World w)
        {
            if (behavior == PedestrianBehavior.DEFAULT)
            {
                
                if (rect.Collides(w.streaker.BoundingRectangle))
                {
                    behavior = PedestrianBehavior.COLLIDE;
                    state = PedestrianState.FALL;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_fall");
                    draw.Reset();
                    return;
                }

                if(Vector2.Distance(w.streaker.Position, pos) < detectRadius){
                    behavior = PedestrianBehavior.AWARE;
                    state = PedestrianState.FLEE;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_walk");
                    draw.Play();
                    return;
                }

                switch (state)
                {
                    case PedestrianState.STATIC:
                        physics.SetTargetValues(true, null, null, null);
                        break;
                    case PedestrianState.WANDER:
                        movement.Wander(ref physics);
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
                    state = PedestrianState.FALL;
                    draw.animation = SpriteDatabase.GetAnimation(studentType + "_fall");
                    draw.Reset();
                    return;
                }

                switch (state)
                {
                    case PedestrianState.WANDER:
                        if (Vector2.Distance(w.streaker.Position, pos) < detectRadius)
                        {
                            state = PedestrianState.FLEE;
                            
                        }
                        else
                        {
                            movement.Wander(ref physics);
                        }
                        break;
                    case PedestrianState.FLEE:
                        if (Vector2.Distance(w.streaker.ComponentPhysics.Position, physics.Position) >= detectRadius)
                        {
                            state = PedestrianState.WANDER;
                        }
                        else
                        {
                            movement.SetTarget(w.streaker.ComponentPhysics.Position);
                            movement.Flee(ref physics);
                        }
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
                        //movement.Stop(ref physics);
                        
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
                        movement.Stop(ref physics);
                        if (rect.Collides(w.streaker.BoundingRectangle))
                        {
                            behavior = PedestrianBehavior.COLLIDE;
                            state = PedestrianState.GET_UP;
                            draw.animation = SpriteDatabase.GetAnimation(studentType + "_getup");
                            draw.Reset();
                            return;
                        }
                        else if (draw.animComplete)
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
            movement.Look(ref physics);
            physics.UpdatePosition(gameTime.ElapsedGameTime.TotalSeconds);
            physics.UpdateOrientation(gameTime.ElapsedGameTime.TotalSeconds);
            pos = physics.Position;
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
        #endregion


    }
}
