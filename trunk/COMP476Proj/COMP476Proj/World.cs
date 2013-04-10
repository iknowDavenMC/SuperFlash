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
        public List<Pedestrian> pedestrians;
        public List<Wall> walls;
        public List<EntityMoveable> moveableObjectsX;
        public List<EntityMoveable> moveableObjectsY;
        #endregion

        #region Init
        public World()
        {
            streaker = new Streaker(new PhysicsComponent2D(new Vector2(100, 100), 0, new Vector2(20,20),150, 750, 150, 750, 8, 50, 0f, true),
                new DrawComponent(SpriteDatabase.GetAnimation("streaker_static"), Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f));

            pedestrians = new List<Pedestrian>();

            pedestrians.Add(new Pedestrian(new PhysicsComponent2D(new Vector2(200, 200), 0, new Vector2(20, 20), 150, 750, 75, 1000, 8, 40, 0f, true), new MovementAIComponent2D(),
                new DrawComponent(SpriteDatabase.GetAnimation("student3_static"), Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f),PedestrianState.WANDER));

            pedestrians.Add(new Pedestrian(new PhysicsComponent2D(new Vector2(300, 300), 0, new Vector2(20, 20), 150, 750, 75, 1000, 8, 40, 0f, true), new MovementAIComponent2D(),
                new DrawComponent(SpriteDatabase.GetAnimation("student2_static"), Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f), PedestrianState.WANDER));

            pedestrians.Add(new Pedestrian(new PhysicsComponent2D(new Vector2(200, 300), 0, new Vector2(20, 20), 150, 750, 75, 1000, 8, 40, 0f, true), new MovementAIComponent2D(),
                new DrawComponent(SpriteDatabase.GetAnimation("student1_static"), Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f), PedestrianState.WANDER));

            pedestrians.Add(new Pedestrian(new PhysicsComponent2D(new Vector2(300, 200), 0, new Vector2(20, 20), 150, 750, 75, 1000, 8, 40, 0f, true), new MovementAIComponent2D(),
                new DrawComponent(SpriteDatabase.GetAnimation("student2_static"), Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f), PedestrianState.WANDER));

            moveableObjectsX = new List<EntityMoveable>();
            moveableObjectsY = new List<EntityMoveable>();

            moveableObjectsX.Add(streaker);
            moveableObjectsY.Add(streaker);

            foreach (Pedestrian pedestrian in pedestrians)
            {
                moveableObjectsX.Add(pedestrian);
                moveableObjectsY.Add(pedestrian);
            }

            moveableObjectsX = moveableObjectsX.OrderBy(o => o.ComponentPhysics.Position.X).ToList();
            moveableObjectsY = moveableObjectsY.OrderBy(o => o.ComponentPhysics.Position.Y).ToList();
        }
        #endregion

        #region Update & Draw
        public void Update(GameTime gameTime)
        {
            // Update lists
            moveableObjectsX = moveableObjectsX.OrderBy(o => o.ComponentPhysics.Position.X).ToList();
            moveableObjectsY = moveableObjectsY.OrderBy(o => o.ComponentPhysics.Position.Y).ToList();

            // Check collision for X
            for (int i = 0; i != moveableObjectsX.Count - 1; ++i)
            {
                if (moveableObjectsX[i].BoundingRectangle.Collides(moveableObjectsX[i + 1].BoundingRectangle))
                {
                    moveableObjectsX[i + 1].ResolveCollision(moveableObjectsX[i]);
                    moveableObjectsX[i].ResolveCollision(moveableObjectsX[i + 1]);
                }
            }

            // Check collision for Y
            for (int i = 0; i != moveableObjectsY.Count - 1; ++i)
            {
                if (moveableObjectsY[i].BoundingRectangle.Collides(moveableObjectsY[i + 1].BoundingRectangle))
                {
                    moveableObjectsY[i].ResolveCollision(moveableObjectsY[i + 1]);
                    moveableObjectsY[i + 1].ResolveCollision(moveableObjectsY[i]);
                }
            }

            // Update streaker
            streaker.Update(gameTime);
            
            // Update camera
            Camera.X = (int)streaker.ComponentPhysics.Position.X - Camera.Width/2;
            Camera.Y = (int)streaker.ComponentPhysics.Position.Y - Camera.Height / 2;
            if (Camera.X < 0)
                Camera.X = 0;
            if (Camera.Y < 0)
                Camera.Y = 0;

            // Update all other moveable objects
            foreach (Pedestrian pedestrian in pedestrians)
            {
                pedestrian.Update(gameTime, this);
            }

            /*
            // Temp collision checks
            if (streaker.BoundingRectangle.Collides(ped.BoundingRectangle))
            {
                ped.ResolveCollision(streaker);
                streaker.ResolveCollision(ped);
                streaker.Fall();
            }
            */

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 drawPos = new Vector2(0, 0);
            spriteBatch.Draw(SpriteDatabase.GetAnimation("level_1").Texture, drawPos, Color.White);

            // Draw streaker
            streaker.Draw(gameTime, spriteBatch);

            // Draw all other moveable objects
            foreach (Pedestrian pedestrian in pedestrians)
            {
                pedestrian.Draw(gameTime, spriteBatch);
            }
            
        }
        #endregion
    }
}
