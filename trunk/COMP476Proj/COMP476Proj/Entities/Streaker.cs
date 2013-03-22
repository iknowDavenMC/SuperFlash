#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace COMP476Proj
{
    public enum StreakerState { STATIC, WALK, FALL, GET_UP, DANCE };

    public class Streaker : Entity
    {
        public bool flip = false;
        public PhysicsComponent physics;
        public DrawComponent draw;
        public StreakerState charState = StreakerState.STATIC;

        private int velocity = 5;

        public Streaker(PhysicsComponent phys, DrawComponent draw)
        {
            physics = phys;
            this.draw = draw;
            //Initialize Components using Entitybuilder!
        }
        public override void Update(GameTime gameTime){
        
            physics.Update(gameTime);
            draw.Update(gameTime, this);
            UpdateStates(draw.animComplete);
            
            
            //Debugger.getInstance().pointsToDraw.Add(physics.position);
            base.Update(gameTime);
        }

        public void UpdateStates(bool animComplete)
        {
            switch (charState)
            {
                case StreakerState.STATIC:
                    draw.animation = SpriteDatabase.GetAnimation("streaker_static");
                    draw.Play();
                    break;
                case StreakerState.WALK:
                    if (flip)
                    {
                        draw.SpriteEffect = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        draw.SpriteEffect = SpriteEffects.None;
                    }
                    draw.animation = SpriteDatabase.GetAnimation("streaker_walk");
                    draw.Play();
                    break;
                case StreakerState.FALL:
               
                    if (animComplete)
                    {
                        draw.animation = SpriteDatabase.GetAnimation("streaker_getup");
                        charState = StreakerState.GET_UP;
                        draw.Reset();
                    }
                    else
                    {
                        draw.animation = SpriteDatabase.GetAnimation("streaker_fall");
                        draw.Play();
                    }
                    break;
                case StreakerState.GET_UP:
                    
                    if (animComplete)
                    {
                        draw.animation = SpriteDatabase.GetAnimation("streaker_static");
                        charState = StreakerState.STATIC;
                        draw.Reset();
                    }
                    else
                    {
                        draw.animation = SpriteDatabase.GetAnimation("streaker_getup");
                        draw.Play();
                    }
                    break;
                case StreakerState.DANCE:
                    draw.animation = SpriteDatabase.GetAnimation("streaker_dance");
                    draw.Play();
                    break;
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            draw.Draw(gameTime, spriteBatch, physics.position);
            base.Draw(gameTime, spriteBatch);
        }

        public override DrawComponent GetDrawComponent()
        {
            return draw;
        }

        public override PhysicsComponent GetPhysicsComponent()
        {
            return physics;
        }

        public void moveLeft() {
            physics.velocity.X = -velocity;
            charState = StreakerState.WALK;
            flip = true;
        }
        public void moveRight() {
            physics.velocity.X = velocity;
            charState = StreakerState.WALK;
            flip = false;
        }
        public void moveDown() {
            physics.velocity.Y = velocity;
            charState = StreakerState.WALK;
        }
        public void moveUp() {
            physics.velocity.Y = -velocity;
            charState = StreakerState.WALK;
        }

    }
}
