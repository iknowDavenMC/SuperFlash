using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace COMP476Proj
{
    class AchievementManager
    {
        private static AchievementManager instance;
        private List<Achievement> achievList;
        private List<AchievementToast> toasts;
        private int toastWidth = 411;
        private int toastHeight = 83;
        private float dropSpeed = 250;

        private AchievementManager()
        {
            achievList = new List<Achievement>();
            toasts = new List<AchievementToast>();

            achievList.Add(new Achievement_Playtime());
            achievList.Add(new Achievement_Playtime2());
            achievList.Add(new Achievement_PressSpace());
        }

        public static AchievementManager getInstance()
        {
            if (instance == null)
                instance = new AchievementManager();
            return instance;
        }

        public void LoadContent(ContentManager content)
        {
            AchievementToast.LoadContent(content);
        }

        public bool AddAchievement(Achievement achv)
        {
            if (!achievList.Contains(achv))
            {
                achievList.Add(achv);
                return true;
            }
            return false;
        }

        public void Update(GameTime gameTime)
        {
            int time = gameTime.ElapsedGameTime.Milliseconds;
            foreach (Achievement achv in achievList)
            {
                if (achv.Locked)
                {
                    achv.Update(gameTime);
                    if (achv.IsAchieved())
                    {
                        achv.Locked = false;
                        int toastX = Game1.SCREEN_WIDTH/2 - toastWidth/2;
                        int toastY = Game1.SCREEN_HEIGHT - 45 - (toasts.Count+1)*toastHeight;
                        toasts.Add(new AchievementToast(achv, 3000, new Vector2(toastX, toastY), toastWidth, toastHeight));
                    }
                }
            }

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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (AchievementToast toast in toasts)
                toast.Draw(gameTime, spriteBatch);
        }
    }
}
