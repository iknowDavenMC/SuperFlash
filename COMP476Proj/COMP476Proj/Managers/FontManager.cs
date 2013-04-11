using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace COMP476Proj
{
    public class FontManager
    {
        private static FontManager instance;
        private Dictionary<string, SpriteFont> fonts;

        private FontManager()
        {
            fonts = new Dictionary<string, SpriteFont>();
        }

        public static FontManager getInstance()
        {
            if (instance == null)
                instance = new FontManager();
            return instance;
        }

        public bool addFont(string name, SpriteFont font)
        {
            if (!fonts.Keys.Contains(name))
            {
                fonts.Add(name, font);
                return true;
            }
            return false;
        }

        public SpriteFont getFont(string name)
        {
            if (fonts.Keys.Contains(name))
                return fonts[name];
            return null;
        }
    }
}
