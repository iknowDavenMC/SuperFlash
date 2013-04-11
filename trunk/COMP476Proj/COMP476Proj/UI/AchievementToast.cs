using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;

namespace COMP476Proj
{
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

        public int Age { get { return age; } }
        public int Lifespan { get { return lifespan; } }
        public float X
        {
            get { return position.X; }
            set
            {
                position.X = value;
                leftSpewer.X = value;
                rightSpewer.X = value + width;
            }
        }
        public float Y
        {
            get { return position.Y; }
            set
            {
                position.Y = value;
                leftSpewer.Y = value + height / 2;
                rightSpewer.Y = value + height/2;
            }
        }

        public AchievementToast(Achievement achv, int duration, Vector2 position, int width, int height)
        {
            title = achv.Name;
            description = achv.Description;
            age = 0;
            lifespan = duration;
            this.position = position;
            this.width = width;
            this.height = height;

            leftSpewer = new ParticleSpewer(
                position.X, position.Y + height / 2,
                10000, 30, MathHelper.ToRadians(90), MathHelper.ToRadians(270),
                0, 500, 2, 120, 60, 60, 0, 1, 1, 1, true, 0.5f);
            leftSpewer.Absolute = true;
            leftSpewer.Start();

            rightSpewer = new ParticleSpewer(
                position.X + width, position.Y + height / 2,
                10000, 30, MathHelper.ToRadians(-90), MathHelper.ToRadians(90),
                0, 500, 2, 120, 60, 60, 0, 1, 1, 1, true, 0.5f);
            rightSpewer.Absolute = true;
            rightSpewer.Start();
        }

        public void Kill()
        {
            leftSpewer.Stop();
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
            float scale = 1f / Camera.Scale;
            Vector2 offset = new Vector2(-Camera.Width / 2, -Camera.Height / 2);
            offset *= scale;
            offset.X += Camera.X;
            offset.Y += Camera.Y;

            Texture2D blank = SpriteDatabase.GetAnimation("blank").Texture;
            Vector2 borderPos = new Vector2(X + offset.X, Y + offset.Y);
            Vector2 borderScale = new Vector2(width * scale, height * scale);
            Vector2 innerPos = new Vector2(X + borderWidth + offset.X, Y + borderWidth + offset.Y);
            Vector2 innerScale = new Vector2((width - borderWidth * 2) * scale, (height - borderWidth * 2) * scale);

            leftSpewer.Draw(gameTime, spriteBatch);
            rightSpewer.Draw(gameTime, spriteBatch);
            spriteBatch.Draw(blank, borderPos, null, Color.Black, 0f, Vector2.Zero, borderScale, SpriteEffects.None, 0);
            spriteBatch.Draw(blank, innerPos, null, Color.White, 0f, Vector2.Zero, innerScale, SpriteEffects.None, 0);

            FontManager fontMan = FontManager.getInstance();
            SpriteFont titleFont = fontMan.getFont("AchieveTitle");
            SpriteFont textFont = fontMan.getFont("AchieveText");

            Vector2 titleSize = titleFont.MeasureString(title);
            Vector2 textSize = textFont.MeasureString(description);
            Vector2 titlePos = new Vector2(X + width / 2 - titleSize.X / 2 + offset.X, Y + borderWidth * 2 + offset.Y);
            Vector2 textPos = new Vector2(X + width / 2 - textSize.X / 2 + offset.X, titlePos.Y + titleSize.Y);
            spriteBatch.DrawString(titleFont, title, titlePos, Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(textFont, description, textPos, Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
