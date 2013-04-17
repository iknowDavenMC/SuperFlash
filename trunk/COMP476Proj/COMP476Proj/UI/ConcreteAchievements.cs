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
    /// Achievement earned through playtime
    /// </summary>
    /// 
    public class Achievement_PlaytimeLong : Achievement
    {
        private int timePlayed;
        private const int maxTime = 300000;

        public Achievement_PlaytimeLong()
            : base("Long Streak!", "Streak for 5 minutes", 1000)
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
    /// Achievement earned by picking up lotion
    /// </summary>
    public class Achievement_Slick : Achievement
    {
        protected int count = 0;
        public Achievement_Slick(int i, int numOfSlicks)
            : base("Slick Rick " + i, "Picked up " + numOfSlicks + " Lotion Powerups", numOfSlicks * 250)
        {
            count = numOfSlicks;
        }

        public override void Update(GameTime gameTime) { }

        public override bool IsAchieved()
        {
            return DataManager.GetInstance().numberOfGrease >= count;
        }
    }

    /// <summary>
    /// Achievement earned by picking up lotion
    /// </summary>
    public class Achievement_PowerUp : Achievement
    {
        protected int count = 0;
        public Achievement_PowerUp(int i, int numOfPwr)
            : base("Powered Up " + i, "Picked up " + numOfPwr + " Total Powerups", numOfPwr * 250)
        {
            count = numOfPwr;
        }

        public override void Update(GameTime gameTime) { }

        public override bool IsAchieved()
        {
            return DataManager.GetInstance().PowerUpCount >= Value;
        }
    }

    /// <summary>
    /// Achievement earned by picking up steroids
    /// </summary>
    public class Achievement_Mass : Achievement
    {
        protected int count = 0;
        public Achievement_Mass(int i, int numOfMass)
            : base("Roid Rage " + i, "Picked up " + numOfMass + " Steroid Powerups", numOfMass * 250)
        {
            count = numOfMass;
        }

        public override void Update(GameTime gameTime) { }

        public override bool IsAchieved()
        {
            return DataManager.GetInstance().numberOfSteroids >= Value;
        }
    }

    /// <summary>
    /// Achievement earned by picking up energy drinks
    /// </summary>
    public class Achievement_Speed : Achievement
    {
        protected int count = 0;
        public Achievement_Speed(int i, int numOfSpeed)
            : base("Speedy Streakzales " + i, "Picked up " + numOfSpeed + " Energy Drink Powerups", numOfSpeed * 250)
        {
            count = numOfSpeed;
        }

        public override void Update(GameTime gameTime) { }

        public override bool IsAchieved()
        {
            return DataManager.GetInstance().numberOfRedBull >= Value;
        }
    }

    /// <summary>
    /// Achievement earned by picking up Sneakers
    /// </summary>
    public class Achievement_Turn : Achievement
    {
        protected int count = 0;
        public Achievement_Turn(int i, int numOfTurn)
            : base("Turn on a Dime " + i, "Picked up " + numOfTurn + " Sneaker Powerups", numOfTurn * 250)
        {
            count = numOfTurn;
        }

        public override void Update(GameTime gameTime) { }

        public override bool IsAchieved()
        {
            return DataManager.GetInstance().numberOfSneakers >= Value;
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
    /// Achievement earned by entering a room (given the room's trigger ID)
    /// </summary>
    public class Achievement_EnterRoom : Achievement
    {
        private int triggerID;
        public Achievement_EnterRoom(int triggerID, string roomName, int points)
            : base("Enter the " + roomName, "Enter the " + roomName, points)
        {
            this.triggerID = triggerID;
        }

        public override void Update(GameTime gameTime) { }

        public override bool IsAchieved()
        {
            foreach (Trigger trigger in Game1.world.map.triggers)
            {
                if (trigger.ID == triggerID)
                {
                    return trigger.hasTriggered();
                }
            }
            return false;
        }
    }

    public class Achievement_DoomHallway : Achievement
    {
        private bool leftSide = false;
        private bool rightSide = false;
        private int leftID, rightID;
        private bool complete = false;
        public Achievement_DoomHallway(int leftID, int rightID)
            : base("Traverse the hallway of DOOM", "Ran all the way through the top hallway", 2000)
        {
            this.leftID = leftID;
            this.rightID = rightID;
        }

        public override void Update(GameTime gameTime)
        {
            if (!complete)
                foreach (Trigger trigger in Game1.world.map.triggers)
                {
                    if (Game1.world.streaker.BoundingRectangle.Collides(trigger.BoundingRectangle))
                    {
                        if (trigger.ID == leftID)
                        {
                            if (!rightSide)
                                leftSide = true;
                            else
                            {
                                rightSide = false;
                                complete = true;
                            }
                        }

                        else if (trigger.ID == rightID)
                        {
                            if (!leftSide)
                                rightSide = true;
                            else
                            {
                                leftSide = false;
                                complete = true;
                            }
                        }
                        else
                        {
                            Game1.world.map.triggers.Find(t => t.ID == leftID).clearTriggered();
                            Game1.world.map.triggers.Find(t => t.ID == rightID).clearTriggered();
                            leftSide = false;
                            rightSide = false;
                        }
                    }
                }
        }
        public override bool IsAchieved()
        {
            return complete;
        }
    }

    public class Achievement_LoseCops : Achievement
    {
        private int countTo;
        public Achievement_LoseCops(int countTo, int points)
            : base("Lost " + (countTo == 1 ? "A" : countTo.ToString()) + " Dumb Cop" + (countTo == 1 ? "" : "s"), "", points)
        {
            this.countTo = countTo;
        }

        public override void Update(GameTime gameTime) { }

        public override bool IsAchieved()
        {
            return DataManager.GetInstance().numberOfDumbCopsLost >= countTo;
        }
    }

    public class Achievement_LoseAllCops : Achievement
    {
        private bool chased = false;
        private int loseNumber;
        public Achievement_LoseAllCops(int numToLose, int points)
            : base("Dastardly Escape x" + numToLose, "Lose all cops after" + (numToLose) + "or more are chasing you", points)
        {
            loseNumber = numToLose;
        }

        public override void Update(GameTime gameTime)
        {
            if (NPC.copsWhoSeeTheStreaker >= loseNumber)
                chased = true;
        }

        public override bool IsAchieved()
        {
            return chased && DataManager.GetInstance().numberOfCopsChasing == 0;
        }
    }

    public class Achievement_TriggerRobocop : Achievement
    {
        public Achievement_TriggerRobocop()
            : base("Dead or alive, you're coming with me!", "Spotted by RoboCop", 1000) { }
        public override void Update(GameTime gameTime) { }
        public override bool IsAchieved()
        {
            return DataManager.GetInstance().numberOfRoboCopsChasing > 0;
        }
    }

    public class Achievement_DanceTime : Achievement
    {
        private int targetTime;
        public Achievement_DanceTime(int timeToDance, int points)
            : base("Get Jiggy With It", "Dance for " + timeToDance/1000f + " seconds", points)
        {
            targetTime = timeToDance;
        }

        public override void Update(GameTime gameTime) { }
        public override bool IsAchieved()
        {
            return DataManager.GetInstance().timeDancing >= targetTime;
        }
    }

    public class Achievement_LongDance : Achievement
    {
        private int targetTime;
        public Achievement_LongDance(int timeToDance, int points)
            : base("Put on a show", "Dance for " + timeToDance / 1000f + " seconds in a row", points)
        {
            targetTime = timeToDance;
        }
        public override void Update(GameTime gameTime) { }
        public override bool IsAchieved()
        {
            return DataManager.GetInstance().longestDance >= targetTime;
        }
    }

    public class Achievement_SuperFlash : Achievement
    {
        private int targetCount;
        public Achievement_SuperFlash(int flashCount, int points)
            : base("SUPERFLASH! x " + flashCount, "Superflash " + flashCount + " time" + (flashCount == 1 ? "" : "s"), points)
        {
            targetCount = flashCount;
        }
        public override void Update(GameTime gameTime) { }
        public override bool IsAchieved()
        {
            return DataManager.GetInstance().numberofSuperFlash >= targetCount;
        }
    }

    public class Achievement_BigSuperFlash : Achievement
    {
        private int targetCount;
        public Achievement_BigSuperFlash(int victimCount, int points)
            : base("Oh, the huge manatee!", "Superflash " + victimCount + (victimCount == 1 ? "" : "s") + " at once", points)
        {
            targetCount = victimCount;
        }
        public override void Update(GameTime gameTime) { }
        public override bool IsAchieved()
        {
            return DataManager.GetInstance().biggestSuperflash >= targetCount;
        }
    }

    public class Achievement_SuperFlashVictims : Achievement
    {
        private int targetCount;
        public Achievement_SuperFlashVictims(int victimCount, int points)
            : base("Exhibitionist x " + victimCount, "Superflash " + victimCount + " victim" + (victimCount == 1 ? "" : "s") + " at once", points)
        {
            targetCount = victimCount;
        }
        public override void Update(GameTime gameTime) { }
        public override bool IsAchieved()
        {
            return DataManager.GetInstance().biggestSuperflash >= targetCount;
        }
    }

    public class Achievement_KnockDownPed : Achievement
    {
        private int targetCount;
        public Achievement_KnockDownPed(int victimCount, int points)
            : base("Ramming Speed! x " + victimCount, "Knock over " + victimCount + " pedestrian" + (victimCount == 1 ? "" : "s"), points)
        {
            targetCount = victimCount;
        }
        public override void Update(GameTime gameTime) { }

        public override bool IsAchieved()
        {
            return DataManager.GetInstance().numberPedestriansKnockedOver >= targetCount;
        }
    }

    public class Achievement_KnockDownCop : Achievement
    {
        private int targetCount;
        public Achievement_KnockDownCop(int victimCount, int points)
            : base("Obstruction of Justice x " + victimCount, "Knock over " + victimCount + " cop" + (victimCount == 1 ? "" : "s"), points)
        {
            targetCount = victimCount;
        }
        public override void Update(GameTime gameTime) { }

        public override bool IsAchieved()
        {
            return DataManager.GetInstance().numberCopsKnockedOver >= targetCount;
        }
    }

}
