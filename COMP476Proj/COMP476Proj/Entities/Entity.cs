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
        #region Virtual Functions
        public virtual DrawComponent GetDrawComponent() { return null; }
        public virtual PhysicsComponent GetPhysicsComponent() { return null; }
        public virtual IntelligenceComponent GetIntelligenceComponent() { return null; }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
        #endregion
    }
}
