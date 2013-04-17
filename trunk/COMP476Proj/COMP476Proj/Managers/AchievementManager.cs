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
        private int toastWidth = 481;
        private int toastHeight = 83;
        private float dropSpeed = 250;

        /// <summary>
        /// Constructor. Add achievements to track here.
        /// </summary>
        private AchievementManager()
        {
            Reset();
        }

        public void Reset()
        {
            achievList = new List<Achievement>();
            toasts = new List<AchievementToast>();

            achievList.Add(new Achievement_Playtime());
            achievList.Add(new Achievement_PlaytimeLong());
            achievList.Add(new Achievement_PressSpace());
            achievList.Add(new Achievement_EnterRoom(7, "Principal's Office", 1000));
            achievList.Add(new Achievement_EnterRoom(8, "Cafeteria", 1000));
            achievList.Add(new Achievement_EnterRoom(6, "Girls' Locker Room", 8008));
            achievList.Add(new Achievement_EnterRoom(4, "Basketball Court", 1000));
            achievList.Add(new Achievement_EnterRoom(5, "Boys' Locker Room", 1000));
            achievList.Add(new Achievement_EnterRoom(2, "Math Class", 1000));
            achievList.Add(new Achievement_EnterRoom(1, "Biology Class", 1000));
            achievList.Add(new Achievement_EnterRoom(3, "Lecture Hall", 1000));
            achievList.Add(new Achievement_DoomHallway(9, 10));

            achievList.Add(new Achievement_Slick(1, 1));
            achievList.Add(new Achievement_Slick(2, 5));
            achievList.Add(new Achievement_Slick(3, 10));
            achievList.Add(new Achievement_Slick(4, 10));
            achievList.Add(new Achievement_Mass(1, 1));
            achievList.Add(new Achievement_Mass(2, 5));
            achievList.Add(new Achievement_Mass(3, 10));
            achievList.Add(new Achievement_Mass(4, 20));
            achievList.Add(new Achievement_Speed(1, 1));
            achievList.Add(new Achievement_Speed(2, 5));
            achievList.Add(new Achievement_Speed(3, 10));
            achievList.Add(new Achievement_Speed(4, 20));
            achievList.Add(new Achievement_Turn(1, 1));
            achievList.Add(new Achievement_Turn(2, 5));
            achievList.Add(new Achievement_Turn(3, 10));
            achievList.Add(new Achievement_Turn(4, 20));
            achievList.Add(new Achievement_PowerUp(1, 5));
            achievList.Add(new Achievement_PowerUp(2, 15));
            achievList.Add(new Achievement_PowerUp(3, 30));
            achievList.Add(new Achievement_PowerUp(4, 50));

            achievList.Add(new Achievement_LoseCops(1, 1000));
            achievList.Add(new Achievement_LoseCops(5, 1500));
            achievList.Add(new Achievement_LoseCops(10, 2000));
            achievList.Add(new Achievement_LoseAllCops(5, 1000));
            achievList.Add(new Achievement_LoseAllCops(10, 1500));
            achievList.Add(new Achievement_TriggerRobocop());

            achievList.Add(new Achievement_DanceTime(10000, 2000));
            achievList.Add(new Achievement_LongDance(5000, 1000));
            achievList.Add(new Achievement_SuperFlash(1, 1000));
            achievList.Add(new Achievement_SuperFlash(5, 2000));
            achievList.Add(new Achievement_BigSuperFlash(10, 2500));
            achievList.Add(new Achievement_SuperFlashVictims(5, 1000));
            achievList.Add(new Achievement_SuperFlashVictims(15, 1500));
            achievList.Add(new Achievement_SuperFlashVictims(25, 2500));
            achievList.Add(new Achievement_KnockDownPed(5, 1000));
            achievList.Add(new Achievement_KnockDownPed(15, 1500));
            achievList.Add(new Achievement_KnockDownPed(25, 25000));
            achievList.Add(new Achievement_KnockDownPed(50, 5000));
            achievList.Add(new Achievement_KnockDownCop(5, 1000));
            achievList.Add(new Achievement_KnockDownCop(10, 1500));
            achievList.Add(new Achievement_KnockDownCop(15, 2500));
            achievList.Add(new Achievement_KnockDownCop(25, 5000));
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
                        DataManager.GetInstance().IncreaseScore(achv.Value, false, 0, 0, true);
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
