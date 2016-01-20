using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace COMP476Proj
{
    class ScorePopup
    {
        private Vector2 position;
        private int value;
        private int age, lifespan;
        private const int riseSpeed = 50;
        private bool done;
        private SpriteFont font;
        public bool IsDone { get { return done; } }

        public ScorePopup(float x, float y, int value, int life)
        {
            position = new Vector2(x, y);
            this.value = value;
            age = 0;
            lifespan = life;
            font = FontManager.getInstance().getFont("AchieveTitle");
            position.X -= font.MeasureString(value.ToString()).X / 2;
        }

        public void Update(GameTime gameTime)
        {
            float time = gameTime.ElapsedGameTime.Milliseconds;
            age += (int)time;
            if (age >= lifespan)
            {
                done = true;
            }
            else
            {
                position.Y -= riseSpeed * time/1000f;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(font, value.ToString(), position, Color.Black);
            spriteBatch.DrawString(font, value.ToString(), position + new Vector2(1,-1), Color.White);
        }
    }
}
