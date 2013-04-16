using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace COMP476Proj
{
    
    public class ConsumableSpawnpoint
    {
        Consumable myConsumable;
        int spawnTimer = 0;
        int spawnDelay;
        bool spawned = true;
        private const int MAX_SPAWN_DELAY = 50000;
        private const int MIN_SPAWN_DELAY = 25000;

        public ConsumableSpawnpoint(Vector2 pos, ConsumableType type)
        {   
            myConsumable = new Consumable(pos,type);
            spawnDelay = Game1.random.Next(MIN_SPAWN_DELAY,MIN_SPAWN_DELAY);
        }

        public void Update(GameTime gameTime)
        {

            spawnTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (spawned)
            {
                myConsumable.Update(gameTime);
                if (myConsumable.isConsumed)
                {
                    spawned = false;
                    spawnTimer = 0;
                }
            }
            else
            {
                if (spawnTimer > spawnDelay)
                {
                    spawned = true;
                    myConsumable.ResetConsumable();
                    spawnDelay = Game1.random.Next(MIN_SPAWN_DELAY, MIN_SPAWN_DELAY);
                }
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (spawned)
            {
                myConsumable.Draw(gameTime, spriteBatch);
            }
        }
    }
}
