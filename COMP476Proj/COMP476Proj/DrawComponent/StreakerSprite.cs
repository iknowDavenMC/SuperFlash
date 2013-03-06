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
        public StreakerSprite(Entity e)
            : base(e, SpriteDatabase.GetAnimation("streaker_static"), 150)
        {
            scale = new Vector2(.5f,.5f);
            Pause();
        }
        #endregion

        #region Update & Draw
        public override void Update(GameTime gameTime)
        {
            PhysicsComponent pc = entity.GetPhysicsComponent();
            if (pc == null)
            {
                Pause();
            }
            CharacterState cState = entity.GetIntelligenceComponent().charState;

            switch (cState)
            {
                case CharacterState.STATIC:
                    animation = SpriteDatabase.GetAnimation("streaker_static");
                    Play();
                    break;
                case CharacterState.WALK:
                    if (entity.GetIntelligenceComponent().flipped)
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
                case CharacterState.FALL:
                    animation = SpriteDatabase.GetAnimation("streaker_fall");
                    Play();
                    break;
                case CharacterState.GET_UP:
                    animation = SpriteDatabase.GetAnimation("streaker_getup");
                    Play();
                    break;
                case CharacterState.DANCE:
                    animation = SpriteDatabase.GetAnimation("streaker_dance");
                    Play();
                    break;
                default:
                    Pause();
                    break;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
        #endregion
    }
}
