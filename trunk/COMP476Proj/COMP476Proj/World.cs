#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;
#endregion

namespace COMP476Proj
{
    public class World
    {
        #region Fields
        public Streaker streaker;
        public Pedestrian ped;
        public List<Wall> walls;
        #endregion

        #region Init
        public World()
        {
            streaker = new Streaker(new PhysicsComponent2D(new Vector2(100, 100), 0, new Vector2(20,20),150, 750, 150, 750, 8, 50, 0.25f, true),
                new DrawComponent(SpriteDatabase.GetAnimation("streaker_static"), Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f));

            ped = new Pedestrian(new PhysicsComponent2D(new Vector2(200, 200), 0, new Vector2(20, 20), 150, 750, 75, 1000, 8, 50, 0.25f, true), new MovementAIComponent2D(),
                new DrawComponent(SpriteDatabase.GetAnimation("student3_static"), Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f),PedestrianState.WANDER);
        }
        #endregion

        #region Update & Draw
        public void Update(GameTime gameTime)
        {
            streaker.Update(gameTime);
            
            Camera.X = (int)streaker.ComponentPhysics.Position.X - Camera.Width/2;
            Camera.Y = (int)streaker.ComponentPhysics.Position.Y - Camera.Height / 2;
            if (Camera.X < 0)
                Camera.X = 0;
            if (Camera.Y < 0)
                Camera.Y = 0;

            //for each enemy --> update
            ped.Update(gameTime,this);

            // Temp collision checks
            if (streaker.BoundingRectangle.Collides(ped.BoundingRectangle))
            {
                ped.ResolveCollision(streaker);
                streaker.ResolveCollision(ped);
                streaker.Fall();
            }


        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 drawPos = new Vector2(0, 0);
            spriteBatch.Draw(SpriteDatabase.GetAnimation("level_1").Texture, drawPos, Color.White);
            streaker.Draw(gameTime, spriteBatch);
            ped.Draw(gameTime, spriteBatch);
            
        }
        #endregion
    }
}
