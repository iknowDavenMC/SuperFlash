using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;

namespace COMP476Proj
{
    
    public class ConsumableSpawnpoint
    {
        Consumable myConsumable;
        float spawnTimer = 0f;
        float spawnDelay;
        bool spawned = false;
        private const int MAX_SPAWN_DELAY = 60000;
        private const int MIN_SPAWN_DELAY = 25000;

        public ConsumableSpawnpoint(Vector2 pos, ConsumableType type)
        {   
            myConsumable = new Consumable(pos,type);
            spawnDelay = SuperFlashGame.random.Next(MIN_SPAWN_DELAY,MAX_SPAWN_DELAY);
        }

        public void Update()
        {

            spawnTimer += Time.deltaTime * 1000f;
            if (spawned)
            {
                myConsumable.Update();
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
                    spawnDelay = SuperFlashGame.random.Next(MIN_SPAWN_DELAY, MAX_SPAWN_DELAY);
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (spawned)
            {
                myConsumable.Draw(gameTime, spriteBatch);
            }
        }
    }
}
