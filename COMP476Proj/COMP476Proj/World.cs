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
        public Map map;
        #endregion

        #region Init
        public World()
        {
            streaker = new Streaker(new PhysicsComponent2D(new Vector2(100, 100), 0, new Vector2(20,20),150, 750, 150, 750, 8, 50, 0.25f, true),
                new DrawComponent(SpriteDatabase.GetAnimation("streaker_static"), Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f));

            pedestrians = new List<Pedestrian>();
            moveableObjectsX = new List<EntityMoveable>();
            moveableObjectsY = new List<EntityMoveable>();

            moveableObjectsX.Add(streaker);
            moveableObjectsY.Add(streaker);

            map = new Map();
        }

        public void LoadMap(string filename)
        {
            map.Load(filename);
            foreach (NPC npc in map.startingNPCs)
            {
                if (npc is Pedestrian)
                    pedestrians.Add((Pedestrian)npc);
                moveableObjectsX.Add(npc);
                moveableObjectsY.Add(npc);
            }
            moveableObjectsX = moveableObjectsX.OrderBy(o => o.ComponentPhysics.Position.X).ToList();
            moveableObjectsY = moveableObjectsY.OrderBy(o => o.ComponentPhysics.Position.Y).ToList();
            streaker.ComponentPhysics.Position = map.playerStart;

            // Set up map grid

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
                foreach (Wall wall in map.walls)
                {
                    if (moveableObjectsY[i].BoundingRectangle.Collides(wall.BoundingRectangle))
                    {
                        moveableObjectsY[i].ResolveCollision(wall);
                    }
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

            AchievementManager.getInstance().Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 drawPos = new Vector2(0, 0);
            spriteBatch.Draw(SpriteDatabase.GetAnimation("level_1").Texture, drawPos, Color.White);

            foreach (Wall wall in map.walls)
            {
                wall.BoundingRectangle.Draw(spriteBatch);
            }

            // Draw all other moveable objects
            foreach (EntityMoveable moveable in moveableObjectsY)
            {
                moveable.Draw(gameTime, spriteBatch);
            }
            AchievementManager.getInstance().Draw(gameTime, spriteBatch);
        }
        #endregion
    }
}
