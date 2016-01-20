using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using StreakerLibrary;

namespace COMP476Proj
{
    /// <summary>
    /// A popup for when an achievement is earned
    /// </summary>
    public class AchievementToast
    {
        private string title;
        private string description;
        private ParticleSpewer leftSpewer;
        private ParticleSpewer rightSpewer;
        private Vector2 position;
        private int age;
        private int lifespan;
        private int width;
        private int height;
        private int borderWidth = 2;
        public static Texture2D banner;

        public int Age { get { return age; } }
        public int Lifespan { get { return lifespan; } }
        public float X
        {
            get { return position.X; }
            set
            {
                position.X = value;
                leftSpewer.X = value + 53;
                rightSpewer.X = value + width;
            }
        }
        public float Y
        {
            get { return position.Y; }
            set
            {
                position.Y = value;
                leftSpewer.Y = value + 52;
                rightSpewer.Y = value + 52;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="achv">Associated achievement</param>
        /// <param name="duration">Duration to stay visible (ms)</param>
        /// <param name="position">Starting position</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public AchievementToast(Achievement achv, int duration, Vector2 position, int width, int height)
        {
            title = achv.Name;
            description = achv.Description;
            age = 0;
            lifespan = duration;
            this.position = position;
            this.width = width;
            this.height = height;
            banner = SpriteDatabase.GetAnimation("achievement_banner").Texture;

            // Yellow particles spew out on the left and right in semi-circles away from the banner
            leftSpewer = new ParticleSpewer(
                position.X+53, position.Y + 52,
                10000, 30, MathHelper.ToRadians(90), MathHelper.ToRadians(270),
                0, 500, 2, 120, 60, 60, 0, 1, 1, 1, true, 0.5f);
            leftSpewer.Absolute = true;
            leftSpewer.Start();

            rightSpewer = new ParticleSpewer(
                position.X + width, position.Y + 52,
                10000, 30, MathHelper.ToRadians(-90), MathHelper.ToRadians(90),
                0, 500, 2, 120, 60, 60, 0, 1, 1, 1, true, 0.5f);
            rightSpewer.Absolute = true;
            rightSpewer.Start();
        }

        /// <summary>
        /// Kill the popup (ie: the particle spewers)
        /// </summary>
        public void Kill()
        {
            leftSpewer.Stop();
            rightSpewer.Stop();
        }

        public void Update(GameTime gameTime)
        {
            int time = gameTime.ElapsedGameTime.Milliseconds;
            age += time;

            leftSpewer.Update(gameTime);
            rightSpewer.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float scale = 1f / CustomCamera.Scale;
            Vector2 offset = new Vector2(-CustomCamera.Width / 2, -CustomCamera.Height / 2);
            offset *= scale;
            offset.X += CustomCamera.X;
            offset.Y += CustomCamera.Y;
            Vector2 bannerPos =  new Vector2(X*scale + offset.X, Y*scale + 4*scale + offset.Y);
            
            leftSpewer.Draw(gameTime, spriteBatch);
            rightSpewer.Draw(gameTime, spriteBatch);
            spriteBatch.Draw(banner, bannerPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);

            FontManager fontMan = FontManager.getInstance();
            SpriteFont titleFont = fontMan.getFont("AchieveTitle");
            SpriteFont textFont = fontMan.getFont("AchieveText");

            Vector2 titleSize = titleFont.MeasureString(title);
            Vector2 textSize = textFont.MeasureString(description);
            Vector2 titlePos = new Vector2(
                (X + 125 + (width-125) / 2 - titleSize.X / 2)*scale + offset.X,
                (Y + 40 + borderWidth * 2)*scale + offset.Y);

            spriteBatch.DrawString(titleFont, title, titlePos, Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(titleFont, title, titlePos + new Vector2(2,-2) * scale, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
