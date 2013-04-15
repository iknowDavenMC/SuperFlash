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
    /// 
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
    /// <summary>
    /// Achievement earned by entering the Principals office
    /// </summary>
    public class Achievement_PrincipalsOffice : Achievement
    {

        public Achievement_PrincipalsOffice() : base("Enter Principals Office", "", 1000) { }
        public override void Update(GameTime gameTime)
        {

        }
        public override bool IsAchieved()
        {
            foreach (Trigger trigger in Game1.world.map.triggers)
            {
                if (trigger.ID == 7)
                {
                    return trigger.hasTriggered();
                }
            }
            return false;
        }
    }
    /// <summary>
    /// Achievement earned by entering the Principals office
    /// </summary>
    public class Achievement_Cafeteria : Achievement
    {

        public Achievement_Cafeteria() : base("Enter the Cafeteria", "", 1000) { }
        public override void Update(GameTime gameTime)
        {

        }
        public override bool IsAchieved()
        {
            foreach (Trigger trigger in Game1.world.map.triggers)
            {
                if (trigger.ID == 8)
                {
                    return trigger.hasTriggered();
                }
            }
            return false;
        }
    }
    /// <summary>
    /// Achievement earned by entering the Principals office
    /// </summary>
    public class Achievement_GirlsLockerRoom : Achievement
    {

        public Achievement_GirlsLockerRoom() : base("Enter Girls LockerRoom", "", 8008) { }
        public override void Update(GameTime gameTime)
        {

        }
        public override bool IsAchieved()
        {
            foreach (Trigger trigger in Game1.world.map.triggers)
            {
                if (trigger.ID == 6)
                {
                    return trigger.hasTriggered();
                }
            }
            return false;
        }
    }
    /// <summary>
    /// Achievement earned by entering the Boys Locker Room
    /// </summary>
    public class Achievement_BoysLockerRoom : Achievement
    {

        public Achievement_BoysLockerRoom() : base("Enter the Boys Locker Room", "", 1000) { }
        public override void Update(GameTime gameTime)
        {

        }
        public override bool IsAchieved()
        {
            foreach (Trigger trigger in Game1.world.map.triggers)
            {
                if (trigger.ID == 5)
                {
                    return trigger.hasTriggered();
                }
            }
            return false;
        }
    }
    /// <summary>
    /// Achievement earned by entering the Boys Locker Room
    /// </summary>
    public class Achievement_BasketBallCourt : Achievement
    {

        public Achievement_BasketBallCourt() : base("Enter the B-Ball Court", "", 1000) { }
        public override void Update(GameTime gameTime)
        {

        }
        public override bool IsAchieved()
        {
            foreach (Trigger trigger in Game1.world.map.triggers)
            {
                if (trigger.ID == 4)
                {
                    return trigger.hasTriggered();
                }
            }
            return false;
        }
    }
    /// <summary>
    /// Achievement earned by entering the lower classroom
    /// </summary>
    public class Achievement_LowerClassRoom : Achievement
    {

        public Achievement_LowerClassRoom() : base("Enter the Math Class", "", 1000) { }
        public override void Update(GameTime gameTime)
        {

        }
        public override bool IsAchieved()
        {
            foreach (Trigger trigger in Game1.world.map.triggers)
            {
                if (trigger.ID == 2)
                {
                    return trigger.hasTriggered();
                }
            }
            return false;
        }
    }
    /// <summary>
    /// Achievement earned by entering the lower classroom
    /// </summary>
    public class Achievement_UpperClassRoom : Achievement
    {

        public Achievement_UpperClassRoom() : base("Enter the Biology Class", "", 1000) { }
        public override void Update(GameTime gameTime)
        {

        }
        public override bool IsAchieved()
        {
            foreach (Trigger trigger in Game1.world.map.triggers)
            {
                if (trigger.ID == 1)
                {
                    return trigger.hasTriggered();
                }
            }
            return false;
        }
    }
    /// <summary>
    /// Achievement earned by entering the lower classroom
    /// </summary>
    public class Achievement_LectureHall : Achievement
    {

        public Achievement_LectureHall() : base("Enter the Lecture Hall", "", 1000) { }
        public override void Update(GameTime gameTime)
        {

        }
        public override bool IsAchieved()
        {
            foreach (Trigger trigger in Game1.world.map.triggers)
            {
                if (trigger.ID == 3)
                {
                    return trigger.hasTriggered();
                }
            }
            return false;
        }
    }
}
