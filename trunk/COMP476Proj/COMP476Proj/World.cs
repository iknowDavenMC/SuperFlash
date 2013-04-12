#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;
using Microsoft.Xna.Framework.Content;
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
        public List<Wall>[,] grid;
        public const int gridLength = 200;
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

        public void LoadMap(string filename, ContentManager content)
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
            createMapGrid();
        }

        private void createMapGrid()
        {
            // Add walls to the grid
            BoundingRectangle test = new BoundingRectangle(Vector2.Zero, gridLength/2);

            grid = new List<Wall>[(int)Math.Ceiling((Map.HEIGHT + 100) / gridLength), (int)Math.Ceiling((Map.WIDTH + 100) / gridLength)];

            int y = grid.GetLength(0);
            int x = grid.GetLength(1);

            for (int i = 0; i != y; ++i)
            {
                for (int j = 0; j != x; ++j)
                {
                    test.Update(new Vector2(j * test.Bounds.Height, i * test.Bounds.Width));

                    grid[i, j] = new List<Wall>();

                    // Check collision
                    for (int k = 0; k != map.walls.Count; ++k)
                    {
                        if (test.Collides(map.walls[k].BoundingRectangle))
                        {
                            grid[i, j].Add(map.walls[k]);
                        }
                    }
                }
            }
        }
        #endregion

        #region Update & Draw
        public void Update(GameTime gameTime)
        {
            // Update lists
            moveableObjectsX = moveableObjectsX.OrderBy(o => o.ComponentPhysics.Position.X).ToList();
            moveableObjectsY = moveableObjectsY.OrderBy(o => o.ComponentPhysics.Position.Y).ToList();

            // Check collision for X
            for (int i = 0; i != moveableObjectsX.Count-1; ++i)
            {
                if (moveableObjectsX[i].BoundingRectangle.Collides(moveableObjectsX[i + 1].BoundingRectangle))
                {
                    moveableObjectsX[i + 1].ResolveCollision(moveableObjectsX[i]);
                    moveableObjectsX[i].ResolveCollision(moveableObjectsX[i + 1]);
                }
            }

            // Check collision for Y
            for (int i = 0; i != moveableObjectsY.Count; ++i)
            {
                if (i < moveableObjectsY.Count-1 && moveableObjectsY[i].BoundingRectangle.Collides(moveableObjectsY[i + 1].BoundingRectangle))
                {
                    moveableObjectsY[i].ResolveCollision(moveableObjectsY[i + 1]);
                    moveableObjectsY[i + 1].ResolveCollision(moveableObjectsY[i]);
                }
            }

            // Check collision for walls
            for (int i = 0; i != moveableObjectsX.Count; ++i)
            {
                int startX = (int)Math.Round(moveableObjectsX[i].BoundingRectangle.Bounds.X / gridLength);
                int startY = (int)Math.Round(moveableObjectsX[i].BoundingRectangle.Bounds.Y / gridLength);
                int endX = (int)Math.Round((moveableObjectsX[i].BoundingRectangle.Bounds.X + moveableObjectsX[i].BoundingRectangle.Bounds.Width) / gridLength);
                int endY = (int)Math.Round((moveableObjectsX[i].BoundingRectangle.Bounds.Y + moveableObjectsX[i].BoundingRectangle.Bounds.Height) / gridLength);

                // Make sure loops go from small values to larger ones
                if (startY > endY)
                {
                    int temp = startY;
                    startY = endY;
                    endY = temp;
                }
                if (startX > endX)
                {
                    int temp = startX;
                    startX = endX;
                    endX = temp;
                }

                for (int k = startY; k != endY + 1; ++k)
                {
                    for (int l = startX; l != endX + 1; ++l)
                    {
                        for (int j = 0; j != grid[k, l].Count; ++j)
                        {
                            if (moveableObjectsX[i].BoundingRectangle.Collides(grid[k, l][j].BoundingRectangle))
                            {
                                moveableObjectsX[i].ResolveCollision(grid[k, l][j]);
                            }
                        }
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

            // Update achievements
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
