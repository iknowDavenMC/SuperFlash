
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;

namespace COMP476Proj
{
    public abstract class Entity
    {
        #region Fields
        protected Vector2 pos;
        protected BoundingRectangle rect;
        public bool isColliding = false;
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
        public float X { get { return pos.x; } set { pos.x = value; } }
        public float Y { get { return pos.y; } set { pos.y = value; } }

        public bool IsColliding
        {
            get { return isColliding; }
            set { isColliding = value; }
        }

        #endregion

        #region Virtual Functions
        public virtual void Update() { }
        public abstract void ResolveCollision(Entity other);
        #endregion
    }
}
