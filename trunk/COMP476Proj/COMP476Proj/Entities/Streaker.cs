#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;
#endregion

namespace COMP476Proj
{
    public enum StreakerState { STATIC, WALK, FALL, GET_UP, DANCE, SUPERFLASH };

    public class Streaker : EntityMoveable
    {
        #region Attributes
        /// <summary>
        /// Is the streaker dead
        /// </summary>
        private bool isDead;
        private bool stayDown;
        /// <summary>
        /// Determines how many milliseconds have gone by since input was last checked
        /// </summary>
        private int inputTimer;

        /// <summary>
        /// Determines how many milliseconds must have gone by before input is checked again
        /// </summary>
        private int inputDelay;

        /// <summary>
        /// Determines how many milliseconds have gone by since last super flash
        /// </summary>
        private int superFlashTimer;

        /// <summary>
        /// Determines how many milliseconds must have gone by before super flash can be done
        /// </summary>
        private int superFlashDelay;

        /// <summary>
        /// Direction the character is moving
        /// </summary>
        private Vector2 direction;

        /// <summary>
        /// Recover Timer
        /// </summary>
        private int recoverTimer = 1000;

        /// <summary>
        /// Whether or not the image is flipped
        /// </summary>
        public bool flip = false;

        /// <summary>
        /// Character state
        /// </summary>
        public StreakerState charState = StreakerState.STATIC;

        /// <summary>
        /// Superflash distance
        /// </summary>
        private const int SUPER_FLASH_DISTANCE = 300;

        /// <summary>
        /// Time to recover
        /// </summary>
        private const int TIME_TO_RECOVER = 1000;

        /// <summary>
        /// Is the player running faster
        /// </summary>
        private bool isSpeedBoost = false;

        /// <summary>
        /// Is the player heavier
        /// </summary>
        private bool isMassBoost = false;

        /// <summary>
        /// Is the player turning more quickly
        /// </summary>
        private bool isGripBoost = false;

        /// <summary>
        /// Is the player turning more quickly
        /// </summary>
        private bool isSlickBoost = false;

        /// <summary>
        /// Time for consumable object to run out
        /// </summary>
        private float consumableDelay = 15000;

        /// <summary>
        /// Timer for consumable object
        /// </summary>
        private float consumableTimer = 0;

        private ParticleSpewer superFlashParticles;
        private ParticleSpewer danceParticles;
        private ParticleSpewer powerParticles;

        private const int particleTimeout = 50;
        private int particleTimer = 0;
        private const int dancePointTimeout = 500;
        private int danceTimer = 0;
        private int danceTotalTime = 0;
        #endregion

        #region Constructors

        public Streaker(PhysicsComponent2D phys, DrawComponent draw)
        {
            physics = phys;
            this.draw = draw;
            //Initialize Components using Entitybuilder!

            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);

            inputTimer = 0;
            recoverTimer = 0;
            inputDelay = 100;
            superFlashTimer = 0;
            superFlashDelay = 20000;

            superFlashParticles = new ParticleSpewer(
                phys.Position.X + draw.animation.FrameWidth / 2, phys.Position.Y + draw.animation.FrameHeight / 2,
                10000, 100, 0, MathHelper.TwoPi,
                500, 1000, 2, 600, 30, 60, 0.1f, 0.1f, 1, 1, true, 0.75f);

            danceParticles = new ParticleSpewer(
                phys.Position.X + draw.animation.FrameWidth / 2, phys.Position.Y + draw.animation.FrameHeight / 2,
                10000, 100, 0, MathHelper.TwoPi,
                50, 300, 2, 350, 180, 200, 0.25f, 0.5f, 1, 1, true, 0f);

            powerParticles = new ParticleSpewer(
                phys.Position.X + draw.animation.FrameWidth / 2, phys.Position.Y + draw.animation.FrameHeight / 2,
                10000, 10, 0, MathHelper.TwoPi,
                50, 300, 2, 350, 180, 200, 0.5f, 1f, 0.5f, 1, true, 0f);
            powerParticles.Start();
        }

        #endregion

        #region Properties

        public bool IsMassBoost
        {
            get { return isMassBoost; }
        }
        public bool IsSlickBoost
        {
            get { return isSlickBoost; }
        }
        public bool IsSpeedBoost
        {
            get { return isSpeedBoost; }
        }
        public bool IsGripBoost
        {
            get { return isGripBoost; }
        }

        public bool hasSuperFlash()
        {
            return (superFlashTimer >= superFlashDelay);
        }

        public bool IsGhost
        {
            get { return recoverTimer < TIME_TO_RECOVER; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Deals with the user input from gamepad or keyboard
        /// </summary>
        /// <param name="gameTime">Game time</param>
        private void handleUserInput(GameTime gameTime)
        {
            inputTimer += gameTime.ElapsedGameTime.Milliseconds;
            superFlashTimer += gameTime.ElapsedGameTime.Milliseconds;
            recoverTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (inputTimer < inputDelay)
            {
                return;
            }
            else
            {
                inputTimer = 0;
            }

            // Reset direction
            direction = Vector2.Zero;

            // Get input manager instance
            InputManager input = InputManager.GetInstance();

            // Check input
            // Dance takes precedence
            if (input.IsDoing("Dance", PlayerIndex.One))
            {
                HUD.getInstance().DanceNotify = false;
                if (charState != StreakerState.DANCE)
                {
                    draw.Reset();
                    SoundManager.GetInstance().PlaySound("Streaker", "Dance", Position, Position);
                }

                charState = StreakerState.DANCE;

                physics.SetTargetValues(true, null, null, null);
            }
            // Dance takes precedence
            else if (input.IsDoing("Superflash", PlayerIndex.One) && superFlashTimer > superFlashDelay)
            {
                if (charState != StreakerState.SUPERFLASH)
                {
                    draw.Reset();
                    SoundManager.GetInstance().PlaySound("Streaker", "SuperFlash", Position, Position);
                }

                charState = StreakerState.SUPERFLASH;

                physics.SetTargetValues(true, null, null, null);
            }
            // Else check movement
            else
            {
                if (input.IsDoing("Left", PlayerIndex.One))
                {
                    moveLeft();
                }
                if (input.IsDoing("Right", PlayerIndex.One))
                {
                    moveRight();
                }
                if (input.IsDoing("Up", PlayerIndex.One))
                {
                    moveUp();
                }
                if (input.IsDoing("Down", PlayerIndex.One))
                {
                    moveDown();
                }

                // If no movement, static
                if (direction == Vector2.Zero)
                {
                    if (charState != StreakerState.STATIC)
                    {
                        draw.Reset();
                    }

                    charState = StreakerState.STATIC;

                    physics.SetTargetValues(true, null, null, null);
                }
                else
                {
                    physics.SetTargetValues(false, direction, null, null);
                }
            }
        }

        /// <summary>
        /// Move left
        /// </summary>
        private void moveLeft()
        {
            direction += -Vector2.UnitX;
            charState = StreakerState.WALK;
            flip = true;
        }

        /// <summary>
        /// Move right
        /// </summary>
        private void moveRight()
        {
            direction += Vector2.UnitX;
            charState = StreakerState.WALK;
            flip = false;
        }

        /// <summary>
        /// Move down
        /// </summary>
        private void moveDown()
        {
            direction += Vector2.UnitY;
            charState = StreakerState.WALK;
        }

        /// <summary>
        /// Move up
        /// </summary>
        private void moveUp()
        {
            direction += -Vector2.UnitY;
            charState = StreakerState.WALK;
        }

        private void superFlash()
        {
            int victimCount = 0;
            foreach (EntityMoveable entity in Game1.world.moveableObjectsX)
            {
                if (entity is Streaker)
                {
                    continue;
                }

                if ((entity.Position - Position).Length() > SUPER_FLASH_DISTANCE)
                {
                    continue;
                }

                bool canBeHit = true;

                LineSegment test = new LineSegment(Position, entity.Position);

                // Check the grid for walls
                int startX = (int)Math.Round(Math.Min(Position.X, entity.Position.X) / World.gridLength);
                int startY = (int)Math.Round(Math.Min(Position.Y, entity.Position.Y) / World.gridLength);
                int endX = (int)Math.Round(Math.Max(Position.X, entity.Position.X) / World.gridLength);
                int endY = (int)Math.Round(Math.Max(Position.Y, entity.Position.Y) / World.gridLength);

                try
                {
                    for (int k = startY; k != endY + 1; ++k)
                    {
                        for (int l = startX; l != endX + 1; ++l)
                        {
                            for (int j = 0; j != Game1.world.grid[k, l].Count; ++j)
                            {
                                if (Game1.world.grid[k, l][j].IsSeeThrough)
                                {
                                    continue;
                                }

                                if (test.IntersectsBox(Game1.world.grid[k, l][j].BoundingRectangle))
                                {
                                    canBeHit = false;
                                    break;
                                }
                            }

                            if (!canBeHit)
                            {
                                break;
                            }
                        }

                        if (!canBeHit)
                        {
                            break;
                        }
                    }

                    if (canBeHit)
                    {
                        entity.Fall(true);
                        if (!(entity is RoboCop))
                        {
                            ++victimCount;
                            DataManager.GetInstance().IncreaseScore(DataManager.Points.SuperFlashKnockDown, true, entity.ComponentPhysics.Position.X, entity.ComponentPhysics.Position.Y - 64);
                        }
                    }
                }
                catch (IndexOutOfRangeException e)
                {

                }
            }
            DataManager dataMan = DataManager.GetInstance();
            dataMan.SuperflashVictims += victimCount;
            if (dataMan.biggestSuperflash < victimCount)
                dataMan.biggestSuperflash = victimCount;
            dataMan.numberofSuperFlash++;
        }

        private void undoConsumable()
        {
            if (isGripBoost)
            {
                GripBoost();
            }
            else if (isSlickBoost)
            {
                SlickBoost();
            }
            else if (isMassBoost)
            {
                MassBoost();
            }
            else if (isSpeedBoost)
            {
                SpeedBoost();
            }
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            if (isGripBoost || isMassBoost || isSlickBoost || isSpeedBoost)
            {
                consumableTimer += gameTime.ElapsedGameTime.Milliseconds;
            }

            if (consumableTimer >= consumableDelay)
            {
                consumableTimer = 0;

                undoConsumable();
            }

            if (charState != StreakerState.FALL && charState != StreakerState.GET_UP && charState != StreakerState.SUPERFLASH)
            {
                handleUserInput(gameTime);
            }

            physics.UpdatePosition(gameTime.ElapsedGameTime.TotalSeconds, out pos);
            physics.UpdateOrientation(gameTime.ElapsedGameTime.TotalSeconds);

            if (!stayDown)
            {
                draw.Update(gameTime);
            }

            UpdateStates(draw.animComplete);
            if (superFlashParticles.IsStarted)
            {
                particleTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (particleTimer >= particleTimeout)
                {
                    particleTimer = 0;
                    superFlashParticles.Stop();
                }
            }
            superFlashParticles.X = physics.Position.X;
            superFlashParticles.Y = physics.Position.Y - 020;
            superFlashParticles.Update(gameTime);

            danceParticles.Stop();
            if (charState == StreakerState.DANCE)
            {
                Camera.Scale = 2f;
                danceParticles.Start();
                danceTimer += gameTime.ElapsedGameTime.Milliseconds;
                danceTotalTime += gameTime.ElapsedGameTime.Milliseconds;
                DataManager dataMan = DataManager.GetInstance();
                if (danceTimer >= dancePointTimeout)
                {
                    danceTimer = 0;
                    dataMan.IncreaseScore(DataManager.Points.Dance, true, physics.Position.X, physics.Position.Y - 64);
                }
                if (danceTotalTime > dataMan.longestDance && dataMan.CanGainPoints)
                    dataMan.longestDance = danceTotalTime;
            }
            else
            {
                Camera.Scale = 1f;
                danceTotalTime = 0;
            }
            danceParticles.X = physics.Position.X;
            danceParticles.Y = physics.Position.Y - 020;
            danceParticles.Update(gameTime);

            powerParticles.Start();
            if (isGripBoost)
                powerParticles.ChangeColor(0, 30, 0.5f, 1, 0.75f, 1);
            else if (IsMassBoost)
                powerParticles.ChangeColor(45, 75, 0.5f, 1, 0.75f, 1);
            else if (isSlickBoost)
                powerParticles.ChangeColor(105, 135, 0.5f, 1, 0.75f, 1);
            else if (IsSpeedBoost)
                powerParticles.ChangeColor(210, 240, 0.5f, 1, 0.75f, 1);
            else
                powerParticles.Stop();
            powerParticles.X = physics.Position.X;
            powerParticles.Y = physics.Position.Y - 020;
            powerParticles.Update(gameTime);

            //Debugger.getInstance().pointsToDraw.Add(physics.position);
            base.Update(gameTime);
        }

        public void UpdateStates(bool animComplete)
        {
            switch (charState)
            {
                case StreakerState.STATIC:
                    draw.animation = SpriteDatabase.GetAnimation("streaker_static");
                    draw.Play();
                    break;
                case StreakerState.WALK:
                    if (flip)
                    {
                        draw.SpriteEffect = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        draw.SpriteEffect = SpriteEffects.None;
                    }
                    draw.animation = SpriteDatabase.GetAnimation("streaker_walk");
                    draw.Play();
                    break;
                case StreakerState.FALL:
                    if (animComplete)
                    {
                        SoundManager.GetInstance().PlaySound("Common", "Fall", Position, Position);
                        draw.animation = SpriteDatabase.GetAnimation("streaker_getup");
                        charState = StreakerState.GET_UP;
                        draw.Reset();
                    }
                    else
                    {
                        draw.animation = SpriteDatabase.GetAnimation("streaker_fall");
                        draw.Play();
                    }
                    break;
                case StreakerState.GET_UP:

                    if (animComplete)
                    {
                        draw.animation = SpriteDatabase.GetAnimation("streaker_static");
                        charState = StreakerState.STATIC;
                        draw.Reset();
                    }
                    else
                    {
                        draw.animation = SpriteDatabase.GetAnimation("streaker_getup");
                        draw.Play();
                    }

                    if (isDead)
                    {
                        stayDown = true;
                    }
                    break;
                case StreakerState.DANCE:
                    draw.animation = SpriteDatabase.GetAnimation("streaker_dance");
                    break;
                case StreakerState.SUPERFLASH:
                    if (animComplete)
                    {
                        superFlashTimer = 0;
                        superFlash();
                        draw.animation = SpriteDatabase.GetAnimation("streaker_static");
                        charState = StreakerState.STATIC;
                        draw.Reset();
                    }
                    else
                    {
                        if (draw.CurrentFrame == 4)
                            superFlashParticles.Start();

                        draw.animation = SpriteDatabase.GetAnimation("streaker_flash");
                        draw.Play();
                    }
                    break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            superFlashParticles.Draw(gameTime, spriteBatch);
            danceParticles.Draw(gameTime, spriteBatch);
            powerParticles.Draw(gameTime, spriteBatch);
            draw.Draw(gameTime, spriteBatch, physics.Position);
            base.Draw(gameTime, spriteBatch);
        }

        public override void Fall(bool isSuperFlash)
        {
            recoverTimer = 0;

            if (charState != StreakerState.FALL)
            {
                draw.Reset();
            }

            charState = StreakerState.FALL;

            physics.SetTargetValues(true, null, null, null);

            return;
        }

        public void GetHit()
        {
            if (stayDown)
            {
                return;
            }
            if (!isSlickBoost && charState != StreakerState.FALL && charState != StreakerState.GET_UP
                && recoverTimer > TIME_TO_RECOVER)
            {
                SoundManager.GetInstance().PlaySound("Common", "Hit");
                recoverTimer = 0;
                draw.Reset();
                charState = StreakerState.FALL;

                HUD.getInstance().decreaseHealth(10);

                physics.SetTargetValues(true, null, null, null);
            }
        }

        public void Kill()
        {
            isDead = true;
        }

        public void SpeedBoost()
        {
            if (isSpeedBoost)
            {
                isSpeedBoost = false;
                physics.SetSpeed(false);
            }
            else
            {
                undoConsumable();
                isSpeedBoost = true;
                physics.SetSpeed(true);
                consumableTimer = 0;
            }
        }

        public void MassBoost()
        {
            if (isMassBoost)
            {
                isMassBoost = false;
                physics.Mass = 50;
            }
            else
            {
                undoConsumable();
                isMassBoost = true;
                physics.Mass = 70;
                consumableTimer = 0;
            }
        }

        public void GripBoost()
        {
            if (isGripBoost)
            {
                isGripBoost = false;
                physics.SetAcceleration(false);
            }
            else
            {
                undoConsumable();
                isGripBoost = true;
                physics.SetAcceleration(true);
                consumableTimer = 0;
            }
        }

        public void SlickBoost()
        {
            if (isSlickBoost)
            {
                isSlickBoost = false;
            }
            else
            {
                undoConsumable();
                isSlickBoost = true;
                consumableTimer = 0;
            }
        }

        #endregion
    }
}
