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

        public Streaker()
        {
            //Initialize Components using Entitybuilder!
        }
        public override void Update(GameTime gameTime){
        
            physics.Update(gameTime);
            draw.Update(gameTime, this);
            //Debugger.getInstance().pointsToDraw.Add(physics.position);
            base.Update(gameTime);
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
            physics.velocity.X = -1;
            charState = StreakerState.WALK;
            flip = true;
        }
        public void moveRight() {
            physics.velocity.X = 1;
            charState = StreakerState.WALK;
            flip = false;
        }
        public void moveDown() {
            physics.velocity.Y = 1;
            charState = StreakerState.WALK;
        }
        public void moveUp() {
            physics.velocity.Y = -1;
            charState = StreakerState.WALK;
        }

    }
}
