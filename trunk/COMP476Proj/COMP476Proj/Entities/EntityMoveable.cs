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

        public virtual bool LineOfSight()
        {
            LineSegment test = new LineSegment(Position, Game1.world.streaker.Position);

            // Check the grid for walls
            int startX = (int)Math.Round(Math.Min(Position.X, Game1.world.streaker.Position.X) / World.gridLength);
            int startY = (int)Math.Round(Math.Min(Position.Y, Game1.world.streaker.Position.Y) / World.gridLength);
            int endX = (int)Math.Round(Math.Max(Position.X, Game1.world.streaker.Position.X) / World.gridLength);
            int endY = (int)Math.Round(Math.Max(Position.Y, Game1.world.streaker.Position.Y) / World.gridLength);

            for (int k = startY; k != endY + 1; ++k)
            {
                for (int l = startX; l != endX + 1; ++l)
                {
                    for (int j = 0; j != Game1.world.grid[k, l].Count; ++j)
                    {
                        if (Game1.world.grid[k, l][j].IsSeeThrough)
                        {
                            continue;
                        }

                        if (test.IntersectsBox(Game1.world.grid[k, l][j].BoundingRectangle))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        #endregion
    }
}
