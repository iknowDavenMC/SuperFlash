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
    public class Streaker : Entity
    {
        public PhysicsComponent physics;
        public DrawComponent draw;
        public IntelligenceComponent intelligence;
        public Streaker()
        {
            //Initialize Components using Entitybuilder!
        }
        public override void Update(GameTime gameTime)
        {
            draw.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            draw.Draw(gameTime, spriteBatch);
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

        public override IntelligenceComponent GetIntelligenceComponent()
        {
            return intelligence;
        }

        public void moveLeft() {
            //physics.velocity.X = -1;
            intelligence.charState = CharacterState.WALK_LEFT;
        }
        public void moveRight() {
            //physics.velocity.X = 1;
            intelligence.charState = CharacterState.WALK_RIGHT;
        }
        public void moveDown() {
            //physics.velocity.Y = 1;
        }
        public void moveUp() {
            //physics.velocity.Y = -1;
        }

    }
}
