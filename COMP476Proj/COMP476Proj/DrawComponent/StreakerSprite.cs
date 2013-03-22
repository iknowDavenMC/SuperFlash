#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace COMP476Proj
{
    public class StreakerSprite : DrawComponent
    {
        #region Init
        public StreakerSprite()
            : base(SpriteDatabase.GetAnimation("streaker_static"), 150)
        {
            scale = new Vector2(.5f,.5f);
            Pause();
        }
        #endregion

        #region Update & Draw
        public override void Update(GameTime gameTime, Entity e)
        {
            Streaker streaker = (Streaker)e;
            switch (streaker.charState)
            {
                case StreakerState.STATIC:
                    animation = SpriteDatabase.GetAnimation("streaker_static");
                    Play();
                    break;
                case StreakerState.WALK:
                    if (streaker.flip)
                    {
                        spriteEffects = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        spriteEffects = SpriteEffects.None;
                    }
                    animation = SpriteDatabase.GetAnimation("streaker_walk");
                    Play();
                    break;
                case StreakerState.FALL:
                    animation = SpriteDatabase.GetAnimation("streaker_fall");
                    Play();
                    break;
                case StreakerState.GET_UP:
                    animation = SpriteDatabase.GetAnimation("streaker_getup");
                    Play();
                    break;
                case StreakerState.DANCE:
                    animation = SpriteDatabase.GetAnimation("streaker_dance");
                    Play();
                    break;
                default:
                    Pause();
                    break;
            }
            base.Update(gameTime, streaker);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 pos)
        {
            base.Draw(gameTime, spriteBatch, pos);
        }
        #endregion
    }
}
