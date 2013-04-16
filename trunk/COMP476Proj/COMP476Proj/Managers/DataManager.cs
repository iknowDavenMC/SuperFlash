using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public class DataManager
    {
        #region Attributes

        /// <summary>
        /// Private instance
        /// </summary>
        private static volatile DataManager instance = null;

        public int health;

        public int score;

        private float time;

        public int numberPedestriansKnockedOver;

        public int numberCopsKnockedOver;

        public int numberOfDumbCopsLost;

        public int numberOfSmartCopsLost;

        public int numberOfSteroids;

        public int numberOfGrease;

        public int numberOfSneakers;

        public int numberOfRedBull;

        private float timeDancing;

        public int numberofSuperFlash;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        private DataManager()
        {
            health = 100;

            score = 0;

            time = 0;

            numberPedestriansKnockedOver = 0;

            numberCopsKnockedOver = 0;

            numberOfDumbCopsLost = 0;

            numberOfSmartCopsLost = 0;

            numberOfSteroids = 0;

            numberOfGrease = 0;

            numberOfSneakers = 0;

            numberOfRedBull = 0;

            timeDancing = 0;

            numberofSuperFlash = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows the instance to be retrieved. This acts as the constructor
        /// </summary>
        /// <returns>The only instance of data manager</returns>
        public static DataManager GetInstance()
        {
            if (instance == null)
            {
                instance = new DataManager();
            }
            
            return instance;
        }

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        public void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (InputManager.GetInstance().IsDoing("Dance", PlayerIndex.One))
            {
                timeDancing += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public void DecreaseHealth(int amount)
        {
            health -= amount;

            if (health < 0)
            {
                health = 0;
            }
        }

        public void IncreaseScore(int amount)
        {
            score += amount;
            HUD.getInstance().increaseScore(amount);
        }

        #endregion
    }
}
