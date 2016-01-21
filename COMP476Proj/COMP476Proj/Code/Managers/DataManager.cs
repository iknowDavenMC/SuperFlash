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
            public const int KnockDownPed = 100;
            public const int KnockDownCop = 200;
            public const int SuperFlashKnockDown = 150;
            public const int FleeProximity = 10;
            public const int Dance = 50;
            public const int LoseCop = 500;
            public const int LoseAllCops = 2000;
        }
        /// <summary>
        /// Private instance
        /// </summary>
        private static volatile DataManager instance = null;

        private const int popupTime = 1000;

        public int health;

        public int score;

        public float time;

        public int numberPedestriansKnockedOver;

        public int numberCopsKnockedOver;

        public int numberOfCopsChasing;

        public int numberOfRoboCopsChasing;

        public int numberOfDumbCopsLost;

        public int numberOfSmartCopsLost;

        public int numberOfSteroids;

        public int numberOfGrease;

        public int numberOfSneakers;

        public int numberOfRedBull;

        public float timeDancing;

        public float longestDance;

        public int numberofSuperFlash;

        public int SuperflashVictims;

        public int biggestSuperflash;

        public int highScore;

        private bool canGainPoints = false;
        public bool CanGainPoints { get { return canGainPoints; } }

        private List<ScorePopup> popups;

        #endregion

        #region Properties
        public int PowerUpCount
        {
            get { return numberOfGrease + numberOfRedBull + numberOfSneakers + numberOfSteroids; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        private DataManager()
        {
            Reset();
        }

        public void Reset()
        {
            health = 100;

            score = 0;

            time = 0;

            numberPedestriansKnockedOver = 0;

            numberCopsKnockedOver = 0;

            numberOfCopsChasing = 0;

            numberOfRoboCopsChasing = 0;

            numberOfDumbCopsLost = 0;

            numberOfSmartCopsLost = 0;

            numberOfSteroids = 0;

            numberOfGrease = 0;

            numberOfSneakers = 0;

            numberOfRedBull = 0;

            timeDancing = 0;

            longestDance = 0;

            numberofSuperFlash = 0;

            biggestSuperflash = 0;

            SuperflashVictims = 0;

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
            time += Time.deltaTime * 1000f;

            if (InputManager.GetInstance().IsDoing("Dance", PlayerIndex.One) && canGainPoints)
            {
                timeDancing += Time.deltaTime * 1000f;
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
            if (!canGainPoints && NPC.copsWhoSeeTheStreaker > 0)
            {
                HUD.getInstance().DanceNotify = true;
                canGainPoints = true;
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

        public void IncreaseScore(int amount, bool popup = false, float x = 0, float y = 0, bool overridePointBlock = false)
        {
            if (canGainPoints || overridePointBlock)
            {
                if (amount < 0)
                    amount = 0;
                score += amount;
                HUD.getInstance().increaseScore(amount);
                if (popup)
                    popups.Add(new ScorePopup(x, y, amount, popupTime));
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (ScorePopup popup in popups)
                popup.Draw(spriteBatch, gameTime);
        }

        public void IncreasePowerUp(ConsumableType cType)
        {
            switch (cType)
            {
                case ConsumableType.MASS:
                    numberOfSteroids++;
                    break;
                case ConsumableType.SLIP:
                    numberOfGrease++;
                    break;
                case ConsumableType.SPEED:
                    numberOfRedBull++;
                    break;
                case ConsumableType.TURN:
                    numberOfSneakers++;
                    break;
            }
        }

        #endregion
    }
}
