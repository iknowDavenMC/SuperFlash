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

            Rectanglef overlap = Rectanglef.Intersect(rect.Bounds, other.BoundingRectangle.Bounds);

            // Handle collision
            if (other is EntityMoveable)
            {
                float mass1 = ComponentPhysics.Mass;
                float mass2 = ((EntityMoveable)other).ComponentPhysics.Mass;

                if (mass1 > mass2)
                {
                    ((EntityMoveable)other).Fall(false);
                }
                if (mass1 < mass2)
                {
                    Fall(false);
                }

                physics.ResolveCollision(((EntityMoveable)other).physics, overlap);
            }
            else if (other is Wall)
            {
                physics.ResolveWallCollision(overlap);
            }

            // Resolve inter penetration
            physics.ResolveInterPenetration(overlap, rect.Bounds);
        }

        /// <summary>
        /// Fall
        /// </summary>
        public virtual void Fall(bool isSuperFlash) { }

        #endregion
    }
}
