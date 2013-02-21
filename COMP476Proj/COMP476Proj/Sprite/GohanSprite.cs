#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Superflash
{
    public class GohanSprite : SpriteComponent
    {
        /*-------------------------------------------------------------------------*/
        #region Init
 
        public GohanSprite() : base(SpriteDatabase.GetAnimation("GohanDown"),150)
        {
            Pause();
        }

        #endregion

        /*-------------------------------------------------------------------------*/
        #region Update & Draw

        public override void Update(Entity gameObj, GameTime gameTime)
        {
            if (gameObj.velocity.X > 0)
            {
                animation = SpriteDatabase.GetAnimation("GohanRight");
                Play();
            }
            else if (gameObj.velocity.X < 0)
            {
                animation = SpriteDatabase.GetAnimation("GohanLeft");
                Play();
            }
            else if (gameObj.velocity.Y > 0)
            {
                animation = SpriteDatabase.GetAnimation("GohanDown");
                Play();
            }
            else if (gameObj.velocity.Y < 0)
            {
                animation = SpriteDatabase.GetAnimation("GohanUp");
                Play();
            }
            else
            {
                Pause();
            }
            base.Update(gameObj, gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Entity gameObj)
        {
            base.Draw(gameTime, spriteBatch, gameObj);
        }

        #endregion
    }
}
