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
            Rectanglef overlap = Rectanglef.Intersect(rect.Bounds, other.BoundingRectangle.Bounds);

            if (overlap.X == 0 && overlap.Y == 0 && overlap.Width == 0 && overlap.Height == 0)
            {
                // No collision
                return;
            }

            isColliding = true;

            // Handle collision
            if (other is EntityMoveable)
            {
                float mass1 = ComponentPhysics.Mass;
                float mass2 = ((EntityMoveable)other).ComponentPhysics.Mass;

                if (mass1 > mass2)
                {
                    if (other is Pedestrian && this is Streaker && ((Pedestrian)other).State != PedestrianState.FALL)
                        DataManager.GetInstance().IncreaseScore(DataManager.Points.KnockDown, true,
                            ((EntityMoveable)other).ComponentPhysics.Position.X,
                            ((EntityMoveable)other).ComponentPhysics.Position.Y - 64);
                    ((EntityMoveable)other).Fall(false);
                }
                if (mass1 < mass2)
                {
                    if (other is Streaker && this is Pedestrian && ((Pedestrian)this).State != PedestrianState.FALL)
                        DataManager.GetInstance().IncreaseScore(DataManager.Points.KnockDown, true, physics.Position.X, physics.Position.Y - 64);
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

        public virtual bool IsVisible(Vector2 position)
        {
            LineSegment test = new LineSegment(Position, position);

            // Check the grid for walls
            int startX = (int)Math.Round(Math.Min(Position.X, position.X) / World.gridLength);
            int startY = (int)Math.Round(Math.Min(Position.Y, position.Y) / World.gridLength);
            int endX = (int)Math.Round(Math.Max(Position.X, position.X) / World.gridLength);
            int endY = (int)Math.Round(Math.Max(Position.Y, position.Y) / World.gridLength);

            for (int k = startY; k != endY + 1; ++k)
            {
                for (int l = startX; l != endX + 1; ++l)
                {
                    for (int j = 0; j != Game1.world.grid[k, l].Count; ++j)
                    {
                        if (test.IntersectsBox(Game1.world.grid[k, l][j].BoundingRectangle))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public Vector2 GetKeyNode()
        {
            float distance = float.MaxValue;
            Vector2 position = Vector2.Zero;
            bool fail = false;

            foreach (Node node in Game1.world.map.nodes)
            {
                fail = false;

                if (!node.IsKey)
                {
                    continue;
                }

                if (IsVisible(node.Position) && (Position - Position).Length() < distance)
                {
                    distance = (Position - Position).Length();
                    position = node.Position;
                }
            }

            return position;
        }

        #endregion
    }
}
