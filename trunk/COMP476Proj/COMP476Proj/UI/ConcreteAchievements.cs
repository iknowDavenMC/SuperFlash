using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace COMP476Proj
{
    // Since all of the concrete achievements are small, they can easily be kept together.
    // If this becomes unweildly, we can split it up

    // To create an achievement, create a new public class and override the Update
    // and IsAchieved methods. Make sure to call the base constructor with the name,
    // description, and score value.

    /// <summary>
    /// Achievement earned through playtime
    /// </summary>
    public class Achievement_Playtime : Achievement
    {
        private int timePlayed;
        private const int maxTime = 2000;

        public Achievement_Playtime()
            : base("Easiest achievement ever", "Exist for 2 seconds", 1000)
        {
            timePlayed = 0;
        }

        public override void Update(GameTime gameTime)
        {
            int time = gameTime.ElapsedGameTime.Milliseconds;
            timePlayed += time;
        }

        public override bool IsAchieved()
        {
            return timePlayed >= maxTime;
        }
    }

    /// <summary>
    /// Second achievement earned through playtime
    /// </summary>
    public class Achievement_Playtime2 : Achievement
    {
        private int timePlayed;
        private const int maxTime = 3000;

        public Achievement_Playtime2()
            : base("Easiest achievement ever 2", "Exist for 3 seconds", 1000)
        {
            timePlayed = 0;

        }

        public override void Update(GameTime gameTime)
        {
            int time = gameTime.ElapsedGameTime.Milliseconds;
            timePlayed += time;
        }

        public override bool IsAchieved()
        {
            return timePlayed >= maxTime;
        }
    }

    /// <summary>
    /// Achievement earned by pressing space
    /// </summary>
    public class Achievement_PressSpace : Achievement
    {
        public bool spacePressed = false;
        public Achievement_PressSpace() : base("Press Space", "Press the spacebar", 1000) { }
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                spacePressed = true;
        }
        public override bool IsAchieved()
        {
            return spacePressed;
        }

    }
}
