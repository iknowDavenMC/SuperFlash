using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CollisionBoxTool
{
    class Box
    {
        public Rectangle rect;
        public bool seeThrough;
        public Box(int x, int y, int w, int h, bool seeThrough)
        {
            rect = new Rectangle(x, y, w, h);
            this.seeThrough = seeThrough;
        }
    }
}
