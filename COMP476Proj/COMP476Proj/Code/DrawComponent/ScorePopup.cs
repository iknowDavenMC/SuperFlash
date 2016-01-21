using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;

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
            position.x -= font.MeasureString(value.ToString()).x / 2f;
        }

        public void Update()
        {
            float time = Time.deltaTime * 1000f;
            age += (int)time;
            if (age >= lifespan)
            {
                done = true;
            }
            else
            {
                position.y -= riseSpeed * time/1000f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, value.ToString(), position, Color.black);
            spriteBatch.DrawString(font, value.ToString(), position + new Vector2(1,-1), Color.white);
        }
    }
}
