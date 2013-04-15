using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace COMP476Proj
{
    /// <summary>
    /// Manager to track and update achievements
    /// </summary>
    class AchievementManager
    {
        private static AchievementManager instance;
        private List<Achievement> achievList;
        private List<AchievementToast> toasts;
        private int toastWidth = 411;
        private int toastHeight = 83;
        private float dropSpeed = 250;

        /// <summary>
        /// Constructor. Add achievements to track here.
        /// </summary>
        private AchievementManager()
        {
            achievList = new List<Achievement>();
            toasts = new List<AchievementToast>();

            achievList.Add(new Achievement_Playtime());
            achievList.Add(new Achievement_Playtime2());
            achievList.Add(new Achievement_PressSpace());
            achievList.Add(new Achievement_PrincipalsOffice());
            achievList.Add(new Achievement_Cafeteria());
            achievList.Add(new Achievement_GirlsLockerRoom());
            achievList.Add(new Achievement_BasketBallCourt());
            achievList.Add(new Achievement_BoysLockerRoom());
            achievList.Add(new Achievement_LowerClassRoom());
            achievList.Add(new Achievement_UpperClassRoom());
            achievList.Add(new Achievement_LectureHall());
        }

        /// <summary>
        /// Get the instance of AchievementManager
        /// </summary>
        /// <returns>AchievementManager instance</returns>
        public static AchievementManager getInstance()
        {
            if (instance == null)
                instance = new AchievementManager();
            return instance;
        }

        /// <summary>
        /// Add an achievement (likely more easily done via the constructor though).
        /// </summary>
        /// <param name="achv"></param>
        /// <returns></returns>
        public bool AddAchievement(Achievement achv)
        {
            if (!achievList.Contains(achv))
            {
                achievList.Add(achv);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update all achievements
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            int time = gameTime.ElapsedGameTime.Milliseconds;
            foreach (Achievement achv in achievList)
            {
                if (achv.Locked)
                {
                    achv.Update(gameTime);
                    // If the achievement is earned, unlock it, add the value to the score and show a popup
                    if (achv.IsAchieved())
                    {
                        SoundManager.GetInstance().PlayAchievement();
                        achv.Locked = false;
                        HUD.getInstance().increaseScore(achv.Value);
                        int toastX = Game1.SCREEN_WIDTH/2 - toastWidth/2;
                        int toastY = Game1.SCREEN_HEIGHT - 45 - (toasts.Count+1)*toastHeight;
                        toasts.Add(new AchievementToast(achv, 3000, new Vector2(toastX, toastY), toastWidth, toastHeight));
                    }
                }
            }

            // Update the popups. They stick around for 3 seconds, then drop away
            int toastCount = toasts.Count;
            bool drop = false;
            for(int i=0; i<toastCount; ++i) 
            {
                AchievementToast toast = toasts[i];
                if (i == 0 && toast.Y <= (Game1.SCREEN_HEIGHT - 45 - toastHeight))
                {
                    drop = true;
                }
                if (drop)
                {
                    toast.Y += dropSpeed * time / 1000f;
                }
                toast.Update(gameTime);
                if (toast.Age >= toast.Lifespan) {
                    toast.Kill();
                    toasts.RemoveAt(i);
                    --toastCount;
                    --i;
                }
            }
        }

        /// <summary>
        /// Draw the achievement popups
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (AchievementToast toast in toasts)
                toast.Draw(gameTime, spriteBatch);
        }
    }
}
