using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace COMP476Proj
{
    public class DataManager
    {
        #region Attributes

        public struct Points
        {
            public const int KnockDown = 100;
            public const int SuperFlashKnockDown = 150;
            public const int FleeProximity = 10;
            public const int Dance = 20;
            public const int LoseCop = 300;
            public const int LoseAllCops = 2000;
        }
        /// <summary>
        /// Private instance
        /// </summary>
        private static volatile DataManager instance = null;

        private const int popupTime = 1000;

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

        private List<ScorePopup> popups;

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

            popups = new List<ScorePopup>();
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

            for (int i = 0; i != popups.Count; ++i)
            {
                ScorePopup popup = popups[i];
                popup.Update(gameTime);
                if (popup.IsDone)
                {
                    popups.RemoveAt(i);
                    --i;
                }
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

        public void IncreaseScore(int amount, bool popup = false, float x = 0, float y = 0)
        {
            if (amount < 0)
                amount = 0;
            score += amount;
            HUD.getInstance().increaseScore(amount);
            if (popup)
                popups.Add(new ScorePopup(x, y, amount, popupTime));
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (ScorePopup popup in popups)
                popup.Draw(spriteBatch, gameTime);
        }

        #endregion
    }
}
