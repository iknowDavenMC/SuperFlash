﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;

namespace COMP476Proj
{
    public enum ConsumableType { SLIP, MASS, TURN, SPEED }
    public class Consumable : EntityVisible
    {
        #region Fields
        protected int fadeTimer = 0;
        protected int fadeDelay = 10000;

        protected bool consumed = false;
        ConsumableType cType;
        #endregion

        #region Properties
        public bool isConsumed
        {
            get { return consumed; }
        }
        #endregion

        public Consumable(Vector2 position, ConsumableType type)
        {
            pos = position;
            rect = new BoundingRectangle(position, 10);
            cType = type;
            switch (type)
            {
                case ConsumableType.MASS:
                    draw = new DrawOscillate(SpriteDatabase.GetAnimation("student1_static"), rect.Bounds.Center);
                    break;
                case ConsumableType.SLIP:
                    draw = new DrawOscillate(SpriteDatabase.GetAnimation("student1_static"), rect.Bounds.Center);
                    break;
                case ConsumableType.SPEED:
                    draw = new DrawOscillate(SpriteDatabase.GetAnimation("student1_static"), rect.Bounds.Center);
                    break;
                case ConsumableType.TURN:
                    draw = new DrawOscillate(SpriteDatabase.GetAnimation("student1_static"), rect.Bounds.Center);
                    break;
            }
        }

        public Consumable(Vector2 position)
        {
            pos = position;
            rect = new BoundingRectangle(position, 10);
            int randNum = Game1.random.Next(3);
            switch (randNum)
            {
                case 0:
                    cType = ConsumableType.MASS;
                    draw = new DrawOscillate(SpriteDatabase.GetAnimation("student1_static"), rect.Bounds.Center);
                    break;
                case 1:
                    cType = ConsumableType.SLIP;
                    draw = new DrawOscillate(SpriteDatabase.GetAnimation("student1_static"), rect.Bounds.Center);
                    break;
                case 2:
                    cType = ConsumableType.SPEED;
                    draw = new DrawOscillate(SpriteDatabase.GetAnimation("student1_static"), rect.Bounds.Center);
                    break;
                case 3:
                    cType = ConsumableType.TURN;
                    draw = new DrawOscillate(SpriteDatabase.GetAnimation("student1_static"), rect.Bounds.Center);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            fadeTimer += gameTime.ElapsedGameTime.Milliseconds;
            draw.Update(gameTime);
            ResolveCollision(Game1.world.streaker);
            if (fadeTimer > fadeDelay)
            {
                consumed = true;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (fadeTimer < fadeDelay * .75 || (float)Math.Sin(gameTime.TotalGameTime.Milliseconds/10) > 0)
            {
                draw.Draw(gameTime, spriteBatch, pos);
            }
            base.Draw(gameTime, spriteBatch);
        }

        public override void ResolveCollision(Entity other)
        {
            if (rect.Collides(other.BoundingRectangle))
            {
                consumed = true;
                switch (cType)
                {
                    case ConsumableType.MASS:
                        ((Streaker)other).MassBoost();
                        break;
                    case ConsumableType.SLIP:
                        ((Streaker)other).SlickBoost();
                        break;
                    case ConsumableType.SPEED:
                        ((Streaker)other).SpeedBoost();
                        break;
                    default:
                        ((Streaker)other).GripBoost();
                        break;
                }
            }
        }

    }
}
