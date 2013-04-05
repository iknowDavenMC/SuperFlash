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
    public abstract class Entity
    {
        #region Fields
        protected Vector2 pos;
        protected BoundingRectangle rect;
        #endregion

        #region Properties
        public Vector2 Position
        {
            get { return pos; }
            set { pos = value; }
        }
        public BoundingRectangle BoundingRectangle
        {
            get { return rect; }
            set { rect = value; }
        }
        #endregion

        #region Virtual Functions
        public virtual void Update(GameTime gameTime) { }
        #endregion
    }
}
