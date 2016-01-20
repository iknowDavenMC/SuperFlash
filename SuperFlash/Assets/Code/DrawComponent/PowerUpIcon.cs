using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StreakerLibrary;
using Microsoft.Xna.Framework.Graphics;
namespace COMP476Proj
{
    public class PowerUpIcon : DrawOscillate
    {
        public PowerUpIcon(Vector2 pos,float depth, bool osc) : 
            base(SpriteDatabase.GetAnimation("pwr_mass"), pos, depth, osc)
        {
            scale.X = .35f;
            scale.Y = .35f;
        }

        public override void Update(GameTime gameTime)
        {
            if (Game1.world.streaker.IsGripBoost)
            {
                OscillateAlpha = true;
            }
            else if (Game1.world.streaker.IsMassBoost)
            {
                OscillateAlpha = true;
            }
            else if (Game1.world.streaker.IsSlickBoost)
            {
                OscillateAlpha = true;
            }
            else if (Game1.world.streaker.IsSpeedBoost)
            {
                OscillateAlpha = true;
            }
            else
            {
                OscillateAlpha = false;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float offsetX, float offsetY)
        {
            if ( Game1.world.streaker.IsGripBoost){
                animation = SpriteDatabase.GetAnimation("pwr_turn");
                base.Draw(gameTime, spriteBatch, offsetX, offsetY);
            }
            else if( Game1.world.streaker.IsMassBoost){
                animation = SpriteDatabase.GetAnimation("pwr_mass");
                base.Draw(gameTime, spriteBatch, offsetX, offsetY);
            }
            else if (Game1.world.streaker.IsSlickBoost){
                animation = SpriteDatabase.GetAnimation("pwr_slick");
                base.Draw(gameTime, spriteBatch, offsetX, offsetY);
            }
            else if (Game1.world.streaker.IsSpeedBoost)
            {
                animation = SpriteDatabase.GetAnimation("pwr_speed");
                base.Draw(gameTime, spriteBatch, offsetX, offsetY);
            }
            else
            {
                OscillateAlpha = false;
            }
            
        }

    }
}
