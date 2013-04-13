using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    /// <summary>
    /// Abstract class representing an achievement
    /// </summary>
    public abstract class Achievement
    {
        private string name;
        private string description;
        private int value;
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public int Value { get { return value; } }
        public bool Locked;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the achievement (this wil lbe shown in the popup)</param>
        /// <param name="description">Description of the achievement</param>
        /// <param name="value">Score value for the achievement</param>
        public Achievement(string name, string description, int value)
        {
            Locked = true;
            this.name = name;
            this.description = description;
            this.value = value;
        }

        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Check if the achievement is achieved
        /// </summary>
        /// <returns>True if the requirements for the achievement have been met</returns>
        public virtual bool IsAchieved()
        {
            return false;
        }
    }
}
