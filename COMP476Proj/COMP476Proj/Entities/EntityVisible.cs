using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace COMP476Proj
{
    public abstract class EntityVisible : Entity
    {
        #region Fields
        protected DrawComponent draw;
        #endregion

        #region Properties
        public DrawComponent ComponentDraw
        {
            get { return draw; }
        }
        #endregion

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
    }
}
