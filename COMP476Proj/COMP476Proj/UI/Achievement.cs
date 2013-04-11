using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public abstract class Achievement
    {
        private string name;
        private string description;
        private int value;
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public int Value { get { return value; } }
        public bool Locked;

        public Achievement(string name, string description, int value)
        {
            Locked = true;
            this.name = name;
            this.description = description;
        }

        public abstract void Update(GameTime gameTime);

        public virtual bool IsAchieved()
        {
            return false;
        }
    }
}
