using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public class Trigger
    {
        public Rectanglef rect;
        private int id;
        public int ID { get { return id; } }
        private static int count = 0;
        public Trigger(int x, int y, int w, int h)
        {
            rect = new Rectanglef(x, y, w, h);
            id = ++count;
        }
        public Trigger(int x, int y, int w, int h, int id)
        {
            rect = new Rectanglef(x, y, w, h);
            this.id = id;
            if (count < id)
                count = id + 1;
        }
    }
}
