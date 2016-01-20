using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CollisionBoxTool
{
    class Trigger
    {
        public Rectangle rect;
        public int id;
        private static int count = 0;
        public Trigger(int x, int y, int w, int h)
        {
            rect = new Rectangle(x, y, w, h);
            id = ++count;
        }
        public Trigger(int x, int y, int w, int h, int id)
        {
            rect = new Rectangle(x, y, w, h);
            this.id = id;
            if (count < id)
                count = id + 1;
        }
        
    }
}
