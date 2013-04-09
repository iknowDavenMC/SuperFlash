using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public class EntityMoveable : EntityVisible
    {
        #region Fields
        protected PhysicsComponent2D physics;
        #endregion

        #region Properties
        public PhysicsComponent2D ComponentPhysics
        {
            get { return physics; }
        }
        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            rect.Update(physics.Position);
        }

        public override void ResolveCollision(Entity other)
        {
            isColliding = true;

            // Handle collision
            if (other is EntityMoveable)
            {
                physics.ResolveCollision(((EntityMoveable)other).physics);
            }
            else if (other is Wall)
            {
                physics.ResolveWallCollision(Rectanglef.Intersect(rect.Bounds, other.BoundingRectangle.Bounds));
            }

            // Resolve inter penetration
            physics.ResolveInterPenetration(Rectanglef.Intersect(rect.Bounds, other.BoundingRectangle.Bounds));
        }

        #endregion
    }
}
