#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;
using StreakerLibrary;
#endregion

namespace COMP476Proj
{
    public class Debugger
    {
        private static Debugger instance = null;

        public List<Vector2> pointsToDraw;
        public List<Rect> rectsToDraw;

        private Debugger()
        {
            pointsToDraw = new List<Vector2>();
            rectsToDraw = new List<Rect>();
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
                spriteBatch.Draw(SpriteDatabase.GetAnimation("happyface").Texture, point, Color.white);
            }

            foreach (Rect rect in rectsToDraw)
            {
                
                spriteBatch.Draw(SpriteDatabase.GetAnimation("happyface").Texture, new Vector2(rect.xMin, rect.yMin), Color.white);
                spriteBatch.Draw(SpriteDatabase.GetAnimation("happyface").Texture, new Vector2(rect.xMin, rect.yMax), Color.white);
                spriteBatch.Draw(SpriteDatabase.GetAnimation("happyface").Texture, new Vector2(rect.xMax, rect.yMin), Color.white);
                spriteBatch.Draw(SpriteDatabase.GetAnimation("happyface").Texture, new Vector2(rect.xMax, rect.yMax), Color.white);
            }
        }
    }
}
