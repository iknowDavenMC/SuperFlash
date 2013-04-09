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
        protected bool isColliding = false;
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
        public float X { get { return pos.X; } set { pos.X = value; } }
        public float Y { get { return pos.Y; } set { pos.Y = value; } }

        public bool IsColliding
        {
            get { return isColliding; }
            set { isColliding = value; }
        }

        #endregion

        #region Virtual Functions
        public virtual void Update(GameTime gameTime) { }
        public abstract void ResolveCollision(Entity other);
        #endregion
    }
}
