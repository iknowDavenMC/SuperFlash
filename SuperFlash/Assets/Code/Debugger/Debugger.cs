#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;
#endregion

namespace COMP476Proj
{
    public class Debugger
    {
        private static Debugger instance = null;

        public List<Vector2> pointsToDraw;
        public List<Rectangle> rectsToDraw;

        private Debugger()
        {
            pointsToDraw = new List<Vector2>();
            rectsToDraw = new List<Rectangle>();
        }

        public static Debugger getInstance()
        {
            if (instance == null)
            {
                instance = new Debugger();
            }
            return instance;
        }

        public void Clear()
        {
            pointsToDraw.Clear();
            rectsToDraw.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Vector2 point in pointsToDraw)
            {
                spriteBatch.Draw(SpriteDatabase.GetAnimation("happyface").Texture, point, Color.White);
            }

            foreach (Rectangle rect in rectsToDraw)
            {
                
                spriteBatch.Draw(SpriteDatabase.GetAnimation("happyface").Texture, new Vector2(rect.Left,rect.Top), Color.White);
                spriteBatch.Draw(SpriteDatabase.GetAnimation("happyface").Texture, new Vector2(rect.Left, rect.Bottom), Color.White);
                spriteBatch.Draw(SpriteDatabase.GetAnimation("happyface").Texture, new Vector2(rect.Right, rect.Top), Color.White);
                spriteBatch.Draw(SpriteDatabase.GetAnimation("happyface").Texture, new Vector2(rect.Right, rect.Bottom), Color.White);
            }
        }
    }
}
