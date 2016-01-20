using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;

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

        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
