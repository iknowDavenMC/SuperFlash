using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StreakerLibrary;

using UnityEngine;
using Assets.Code._XNA;

namespace COMP476Proj
{
    public class PowerUpIcon : DrawOscillate
    {
        public PowerUpIcon(Vector2 pos,float depth, bool osc) : 
            base(SpriteDatabase.GetAnimation("pwr_mass"), pos, depth, osc)
        {
            scale.x = .35f;
            scale.y = .35f;
        }

        public override void Update()
        {
            if (SuperFlashGame.world.streaker.IsGripBoost)
            {
                OscillateAlpha = true;
            }
            else if (SuperFlashGame.world.streaker.IsMassBoost)
            {
                OscillateAlpha = true;
            }
            else if (SuperFlashGame.world.streaker.IsSlickBoost)
            {
                OscillateAlpha = true;
            }
            else if (SuperFlashGame.world.streaker.IsSpeedBoost)
            {
                OscillateAlpha = true;
            }
            else
            {
                OscillateAlpha = false;
            }
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, float offsetX, float offsetY)
        {
            if ( SuperFlashGame.world.streaker.IsGripBoost){
                animation = SpriteDatabase.GetAnimation("pwr_turn");
                base.Draw(spriteBatch, offsetX, offsetY);
            }
            else if( SuperFlashGame.world.streaker.IsMassBoost){
                animation = SpriteDatabase.GetAnimation("pwr_mass");
                base.Draw(spriteBatch, offsetX, offsetY);
            }
            else if (SuperFlashGame.world.streaker.IsSlickBoost){
                animation = SpriteDatabase.GetAnimation("pwr_slick");
                base.Draw(spriteBatch, offsetX, offsetY);
            }
            else if (SuperFlashGame.world.streaker.IsSpeedBoost)
            {
                animation = SpriteDatabase.GetAnimation("pwr_speed");
                base.Draw(spriteBatch, offsetX, offsetY);
            }
            else
            {
                OscillateAlpha = false;
            }
            
        }

    }
}
