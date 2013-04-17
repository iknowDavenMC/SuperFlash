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
        public List<NPC> npcs;
        public List<Wall> walls;
        public List<EntityMoveable> moveableObjectsX;
        public List<EntityMoveable> moveableObjectsY;
        //public List<Consumable> consumables;
        public List<ConsumableSpawnpoint> consumableSpawns;
        public Map map;
        public List<Wall>[,] grid;
        public QuadTree qTree;
        public const int gridLength = 200;
        private Flock flock;
        protected int copSpawnTimer = 0;
        protected int copSpawnDelay = 30000;
        #endregion

        #region Init
        public World()
        {
            streaker = new Streaker(new PhysicsComponent2D(new Vector2(-50, -50), 0, new Vector2(20, 20), 200, 1250, 150, 750, 8, 50, 0.25f, true),
                new DrawComponent(SpriteDatabase.GetAnimation("streaker_static"), Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f));

            npcs = new List<NPC>();
            moveableObjectsX = new List<EntityMoveable>();
            moveableObjectsY = new List<EntityMoveable>();

            moveableObjectsX.Add(streaker);
            moveableObjectsY.Add(streaker);

            qTree = new QuadTree((int)Map.WIDTH, (int)Map.HEIGHT, 3);
            map = new Map();
            flock = new Flock();
            consumableSpawns = new List<ConsumableSpawnpoint>();
        }

        public void LoadMap(string filename, ContentManager content)
        {
            map.Load(filename);
            foreach (NPC npc in map.startingNPCs)
            {
                npcs.Add(npc);
                moveableObjectsX.Add(npc);
                moveableObjectsY.Add(npc);
                if (npc is SmartCop)
                {
                    flock.Members.Add(npc);
                    npc.flock = flock;
                }
            }

            streaker.ComponentPhysics.Position = map.playerStart;
            moveableObjectsX = moveableObjectsX.OrderBy(o => o.ComponentPhysics.Position.X).ToList();
            moveableObjectsY = moveableObjectsY.OrderBy(o => o.ComponentPhysics.Position.Y).ToList();

            consumableSpawns = map.consumables;
            //consumables = consumables.OrderBy(o => ((DrawOscillate)o.ComponentDraw).)ToList();
            foreach (Wall w in map.walls)
            {
                qTree.insert(w);
            }
            // Set up map grid
            createMapGrid();
        }

        private void createMapGrid()
        {
            // Add walls to the grid
            BoundingRectangle test = new BoundingRectangle(Vector2.Zero, gridLength / 2);

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

        private void spawnCop()
        {
            int randNum = Game1.random.Next(map.nodes.Count);
            Node randNode = map.nodes.ElementAt(randNum);
            while (!((Camera.X - Camera.Width / 2 > randNode.Position.X ||
                   Camera.X + Camera.Width / 2 < randNode.Position.X) &&
                   (Camera.Y - Camera.Height / 2 > randNode.Position.Y ||
                   Camera.Y + Camera.Height / 2 < randNode.Position.Y)))
            {
                randNum = Game1.random.Next(map.nodes.Count);
                randNode = map.nodes.ElementAt(randNum);
            }   
            SmartCop newNpc = new SmartCop(
                            new PhysicsComponent2D(new Vector2(randNode.Position.X, randNode.Position.Y), 0, new Vector2(20, 20),
                                100, 750, 75, 1000, 8, 50, 0.25f, true),
                            new MovementAIComponent2D(3, 2, MathHelper.ToRadians(45), 0.5f, 50, 25, Vector2.Zero, Vector2.Zero, 0.1f),
                            new DrawComponent(SpriteDatabase.GetAnimation("smartCop_walk"), Color.White, Vector2.Zero,
                                new Vector2(.4f, .4f), .5f), SmartCopState.WANDER);
            npcs.Add(newNpc);
            moveableObjectsX.Add(newNpc);
            moveableObjectsY.Add(newNpc);
            moveableObjectsX = moveableObjectsX.OrderBy(o => o.ComponentPhysics.Position.X).ToList();
            moveableObjectsY = moveableObjectsY.OrderBy(o => o.ComponentPhysics.Position.Y).ToList();
            if (newNpc is SmartCop)
            {
                flock.Members.Add(newNpc);
                newNpc.flock = flock;
            }
        }
        #endregion



        #region Update & Draw
        public void Update(GameTime gameTime)
        {
            copSpawnTimer += gameTime.ElapsedGameTime.Milliseconds;
            if(copSpawnDelay < copSpawnTimer){
                spawnCop();
                copSpawnTimer = 0;
            }
            // Update node costs
            foreach (Node node in map.nodes)
            {
                node.Update(gameTime);
            }

            // Update lists
            moveableObjectsX = moveableObjectsX.OrderBy(o => o.ComponentPhysics.Position.X).ToList();
            moveableObjectsY = moveableObjectsY.OrderBy(o => o.ComponentPhysics.Position.Y).ToList();

            // Check collision for walls
            for (int i = 0; i != moveableObjectsX.Count; ++i)
            {
                int startX = (int)Math.Round(moveableObjectsX[i].BoundingRectangle.Bounds.X / gridLength);
                int startY = (int)Math.Round(moveableObjectsX[i].BoundingRectangle.Bounds.Y / gridLength);
                int endX = (int)Math.Round((moveableObjectsX[i].BoundingRectangle.Bounds.X + moveableObjectsX[i].BoundingRectangle.Bounds.Width) / gridLength);
                int endY = (int)Math.Round((moveableObjectsX[i].BoundingRectangle.Bounds.Y + moveableObjectsX[i].BoundingRectangle.Bounds.Height) / gridLength);

                if (startX < 0)
                    startX = 0;
                if (startY < 0)
                    startY = 0;
                if (endX > Game1.world.grid.GetUpperBound(1))
                    endX = Game1.world.grid.GetUpperBound(1);
                if (endY > Game1.world.grid.GetUpperBound(0))
                    endY = Game1.world.grid.GetUpperBound(0);

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

            // Check collision for X
            for (int i = 0; i != moveableObjectsX.Count - 1; ++i)
            {
                if (moveableObjectsX[i].BoundingRectangle.Collides(moveableObjectsX[i + 1].BoundingRectangle))
                {
                    //moveableObjectsX[i + 1].ResolveCollision(moveableObjectsX[i]);
                    moveableObjectsX[i].ResolveCollision(moveableObjectsX[i + 1]);
                }
            }

            // Check collision for Y
            for (int i = 0; i != moveableObjectsY.Count; ++i)
            {
                if (i < moveableObjectsY.Count - 1 && moveableObjectsY[i].BoundingRectangle.Collides(moveableObjectsY[i + 1].BoundingRectangle))
                {
                    moveableObjectsY[i].ResolveCollision(moveableObjectsY[i + 1]);
                    //moveableObjectsY[i + 1].ResolveCollision(moveableObjectsY[i]);
                }
            }


            foreach (Trigger trigger in map.triggers)
            {
                if (trigger.BoundingRectangle.Collides(streaker.BoundingRectangle))
                {
                    trigger.ResolveCollision(streaker);
                }
            }
            // Update streaker
            streaker.Update(gameTime);

            // Update camera
            Camera.X = (int)streaker.ComponentPhysics.Position.X - Camera.Width / 2;
            Camera.Y = (int)streaker.ComponentPhysics.Position.Y - Camera.Height / 2;
            if (Camera.X < 0)
                Camera.X = 0;
            if (Camera.Y < 0)
                Camera.Y = 0;

            // Update all other moveable objects
            foreach (NPC myNPC in npcs)
            {
                if (myNPC is Pedestrian)
                {
                    ((Pedestrian)myNPC).Update(gameTime, this);
                }
                else if (myNPC is DumbCop)
                {
                    ((DumbCop)myNPC).Update(gameTime, this);
                }
                if (myNPC is SmartCop)
                {
                    ((SmartCop)myNPC).Update(gameTime, this);
                }
                else if (myNPC is RoboCop)
                {
                    ((RoboCop)myNPC).Update(gameTime, this);
                }
            }

            //Update consumables --> if we want random consumable drops
            //foreach (Consumable c in consumables)
            //{
            //    c.Update(gameTime);
            //}
            //consumables.RemoveAll(o => o.isConsumed);

            foreach (ConsumableSpawnpoint c in consumableSpawns)
            {
                c.Update(gameTime);
            }
            
            // Update achievements
            AchievementManager.getInstance().Update(gameTime);
            DataManager.GetInstance().Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 drawPos = new Vector2(0, 0);
            spriteBatch.Draw(SpriteDatabase.GetAnimation("level_1").Texture, drawPos, null, Color.White, 0, Vector2.Zero,1f,SpriteEffects.None,.1f);

            
            foreach (Wall wall in map.walls)
            {
                wall.BoundingRectangle.Draw(spriteBatch);
            }

            // Draw all other moveable objects
            foreach (EntityMoveable moveable in moveableObjectsY)
            {
                moveable.Draw(gameTime, spriteBatch);
            }

            Texture2D blank = SpriteDatabase.GetAnimation("blank").Texture;

#if (DEBUG)
            foreach (Node n in map.nodes)
            {
                Rectangle destRect = new Rectangle((int)n.Position.X - 3, (int)n.Position.Y - 3, 6, 6);
                spriteBatch.Draw(blank, destRect, Color.Cyan);
            }
#endif
            foreach (ConsumableSpawnpoint c in consumableSpawns)
            {
                c.Draw(gameTime,spriteBatch);
            }
            AchievementManager.getInstance().Draw(gameTime, spriteBatch);
            DataManager.GetInstance().Draw(spriteBatch, gameTime);
        }
        #endregion
    }
}
