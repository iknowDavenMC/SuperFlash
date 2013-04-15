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
        /// Determines how many milliseconds have gone by since input was last checked
        /// </summary>
        private int inputTimer;

        /// <summary>
        /// Determines how many milliseconds must have gone by before input is checked again
        /// </summary>
        private int inputDelay;

        /// <summary>
        /// Direction the character is moving
        /// </summary>
        private Vector2 direction;

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
        private const int SUPER_FLASH_DISTANCE = 500;

        #endregion

        #region Constructors

        public Streaker(PhysicsComponent2D phys, DrawComponent draw)
        {
            physics = phys;
            this.draw = draw;
            //Initialize Components using Entitybuilder!

            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);

            inputTimer = 0;
            inputDelay = 100;
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
                if (charState != StreakerState.DANCE)
                {
                    draw.Reset();
                    SoundManager.GetInstance().PlaySound("Streaker", "Dance", Position, Position);
                }

                charState = StreakerState.DANCE;

                physics.SetTargetValues(true, null, null, null);
            }
            // Dance takes precedence
            else if (input.IsDoing("Superflash", PlayerIndex.One))
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
                }
            }
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            if (charState != StreakerState.FALL && charState != StreakerState.GET_UP && charState != StreakerState.SUPERFLASH)
            {
                handleUserInput(gameTime);
            }

            physics.UpdatePosition(gameTime.ElapsedGameTime.TotalSeconds, out pos);
            physics.UpdateOrientation(gameTime.ElapsedGameTime.TotalSeconds);

            draw.Update(gameTime, this);
            UpdateStates(draw.animComplete);

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
                    break;
                case StreakerState.DANCE:
                    draw.animation = SpriteDatabase.GetAnimation("streaker_dance");
                    break;
                case StreakerState.SUPERFLASH:
                    if (animComplete)
                    {
                        superFlash();
                        draw.animation = SpriteDatabase.GetAnimation("streaker_static");
                        charState = StreakerState.STATIC;
                        draw.Reset();
                    }
                    else
                    {
                        draw.animation = SpriteDatabase.GetAnimation("streaker_flash");
                        draw.Play();
                    }
                    break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            draw.Draw(gameTime, spriteBatch, physics.Position);
            base.Draw(gameTime, spriteBatch);
        }

        public override void Fall(bool isSuperFlash)
        {
            if (charState != StreakerState.FALL)
            {
                draw.Reset();
            }

            charState = StreakerState.FALL;

            physics.SetTargetValues(true, null, null, null);

            return;
        }

        #endregion
    }
}
