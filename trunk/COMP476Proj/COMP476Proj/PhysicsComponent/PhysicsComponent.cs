#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace COMP476Proj
{
    public class PhysicsComponent
    {
        #region Fields
        public Vector2 position = new Vector2(200f,200f);
        public Vector2 velocity = Vector2.Zero;
        #endregion
    }
}
