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
        public struct Speeds
        {
            public const int Streaker_Run = 200;
            public const int Pedestrian_Run = 115;
            public const int Pedestrian_Walk = 75;
            public const int DumbCop_Run = 135;
            public const int DumbCop_Walk = 75;
            public const int SmartCop_Run = 145;
            public const int SmartCop_Walk = 75;
            public const int RoboCop_Run = 115;
        }

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
        private Flock smartFlock;
        private Flock dumbFlock;
        protected int copSpawnTimer = 0;
        protected int copSpawnDelay = 10000;
        protected int spawnCount = 0;
        protected float smartCopThreshold = 0.0f;
        protected float dumbCopThreshold = 1.0f;
        protected float roboCopThreshold = 0.0f;
        
        #endregion

        #region Init
        public World()
        {
            streaker = new Streaker(new PhysicsComponent2D(new Vector2(-50, -50), 0, new Vector2(20, 20), Speeds.Streaker_Run, 1250, 150, 750, 8, 50, 0.25f, true),
                new DrawComponent(SpriteDatabase.GetAnimation("streaker_static"), Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f));

            npcs = new List<NPC>();
            moveableObjectsX = new List<EntityMoveable>();
            moveableObjectsY = new List<EntityMoveable>();

            moveableObjectsX.Add(streaker);
            moveableObjectsY.Add(streaker);

            qTree = new QuadTree((int)Map.WIDTH, (int)Map.HEIGHT, 3);
            map = new Map();
            smartFlock = new Flock(300);
            dumbFlock = new Flock(150);
            consumableSpawns = new List<ConsumableSpawnpoint>();
            updateSpawnData();
        }

        public void LoadMap(string filename)
        {
            map.Load(filename);
            foreach (NPC npc in map.startingNPCs)
            {
                npcs.Add(npc);
                moveableObjectsX.Add(npc);
                moveableObjectsY.Add(npc);
                if (npc is SmartCop)
                {
                    smartFlock.Members.Add(npc);
                    npc.flock = smartFlock;
                    spawnCount++;
                }
                if (npc is DumbCop)
                {
                    dumbFlock.Members.Add(npc);
                    npc.flock = dumbFlock;
                }
                if (npc is RoboCop || npc is DumbCop)
                {
                    spawnCount++;
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
            double randFloat = Game1.random.NextDouble();
            randNum = Game1.random.Next(2);
            
            NPC newNpc;

            if (randFloat <= dumbCopThreshold)
            {
                Animation animation;
                DumbCopState dcState;
                if (randNum == 1)
                {
                    dcState = DumbCopState.STATIC;
                    animation = SpriteDatabase.GetAnimation("cop_static");

                }
                else
                {
                    dcState = DumbCopState.WANDER;
                    animation = SpriteDatabase.GetAnimation("cop_walk");
                }
                newNpc = new DumbCop(
                    new PhysicsComponent2D(new Vector2(randNode.Position.X, randNode.Position.Y), 0, new Vector2(20, 20), 135, 750, 75, 1000, 8, 50, 0.25f, true),
                    new MovementAIComponent2D(),
                    new DrawComponent(animation, Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f), dcState);
            }
            else if (randFloat <= smartCopThreshold)
            {
                Animation animation;
                SmartCopState scState;
                if (randNum == 1)
                {
                    scState = SmartCopState.STATIC;
                    animation = SpriteDatabase.GetAnimation("smartCop_static");

                }
                else
                {
                    scState = SmartCopState.WANDER;
                    animation = SpriteDatabase.GetAnimation("smartCop_walk");
                }
                newNpc = new SmartCop(
                                new PhysicsComponent2D(new Vector2(randNode.Position.X, randNode.Position.Y), 0, new Vector2(20, 20),
                                    100, 750, 75, 1000, 8, 50, 0.25f, true),
                                new MovementAIComponent2D(3, 2, MathHelper.ToRadians(45), 0.5f, 50, 25, Vector2.Zero, Vector2.Zero, 0.1f),
                                new DrawComponent(animation, Color.White, Vector2.Zero,
                                    new Vector2(.4f, .4f), .5f), scState);
            }
            else
            {
                newNpc = new RoboCop(
                            new PhysicsComponent2D(new Vector2(randNode.Position.X, randNode.Position.Y), 0, new Vector2(20, 20), 100, 750, 75, 1000, 8, 70, 0.25f, true),
                            new MovementAIComponent2D(),
                            new DrawComponent(SpriteDatabase.GetAnimation("roboCop_static"), Color.White,
                                              Vector2.Zero, new Vector2(.4f, .4f), .5f));
            }
            
            npcs.Add(newNpc);
            moveableObjectsX.Add(newNpc);
            moveableObjectsY.Add(newNpc);
            moveableObjectsX = moveableObjectsX.OrderBy(o => o.ComponentPhysics.Position.X).ToList();
            moveableObjectsY = moveableObjectsY.OrderBy(o => o.ComponentPhysics.Position.Y).ToList();
            if (newNpc is SmartCop)
            {
                smartFlock.Members.Add(newNpc);
                newNpc.flock = smartFlock;
            }
            if (newNpc is DumbCop)
            {
                dumbFlock.Members.Add(newNpc);
                newNpc.flock = dumbFlock;
            }

            updateSpawnData();
        }

        public void updateSpawnData()
        {
            switch (Game1.difficulty)
            {
                case Difficulty.EASY:
                    if (spawnCount > 5)
                    {
                        dumbCopThreshold = .75f;
                        smartCopThreshold = 0.25f;
                        copSpawnDelay = 15000;
                    }
                    else if (spawnCount > 10)
                    {
                        dumbCopThreshold = .75f;
                        smartCopThreshold = 0.25f;
                        copSpawnDelay = 20000;
                    }
                    else if (spawnCount > 25)
                    {
                        dumbCopThreshold = .5f;
                        smartCopThreshold = 0.5f;
                        copSpawnDelay = 50000;
                    }
                    break;
                case Difficulty.MEDIUM:
                    if (spawnCount > 5)
                    {
                        dumbCopThreshold = .75f;
                        smartCopThreshold = 0.25f;
                        copSpawnDelay = 10000;
                    }
                    else if (spawnCount > 10)
                    {
                        dumbCopThreshold = .5f;
                        smartCopThreshold = 0.5f;
                        copSpawnDelay = 20000;
                    }
                    else if (spawnCount > 25)
                    {
                        dumbCopThreshold = .5f;
                        smartCopThreshold = 0.49f;
                        copSpawnDelay = 50000;
                    }
                    break;
                case Difficulty.HARD:
                    if (spawnCount > 5)
                    {
                        dumbCopThreshold = .75f;
                        smartCopThreshold = 0.25f;
                        copSpawnDelay = 10000;
                    }
                    else if (spawnCount > 10)
                    {
                        dumbCopThreshold = .75f;
                        smartCopThreshold = 0.25f;
                        copSpawnDelay = 20000;
                    }
                    else if (spawnCount > 25)
                    {
                        dumbCopThreshold = .25f;
                        smartCopThreshold = 0.74f;
                        copSpawnDelay = 45000;
                    }
                    break;
                case Difficulty.IMPOSSIBLE:
                    dumbCopThreshold = 0.0f;
                    smartCopThreshold = 0.65f;
                    copSpawnDelay = 5000;
                    break;
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

                try
                {
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
                catch (IndexOutOfRangeException e)
                {
                    moveableObjectsY.Remove(moveableObjectsX[i]);
                    npcs.Remove((NPC)moveableObjectsX[i]);
                    moveableObjectsX.RemoveAt(i);
                    --i;
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
