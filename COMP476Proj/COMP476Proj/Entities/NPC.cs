using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StreakerLibrary;

namespace COMP476Proj
{
    public abstract class NPC : EntityMoveable
    {
        #region Fields
        protected MovementAIComponent2D movement;
        protected float detectRadius = 300f;
        protected float farRadius = 500f;
        public Node patrolStart = null, patrolEnd = null;
        public Flock flock;

        protected const float rayDist = 10;

        public static int copsWhoSeeTheStreaker = 0;
        private const float AVOIDANCE_ANGLE = 90;
        private const float RAY_LENGTH = 30;
        #endregion

        #region Properties
        public MovementAIComponent2D ComponentMovement
        {
            get { return movement; }
        }
        #endregion

        protected virtual bool testWallCollide()
        {
            Vector2 dir = physics.Velocity;
            if (dir.Length() > 0)
            {
                dir.Normalize();
                dir *= RAY_LENGTH;
            }

            Vector2 endPoint = pos + dir;
            LineSegment ray = new LineSegment(pos, endPoint);

            // Check the grid for walls
            int startX = (int)Math.Round(Math.Min(Position.X, endPoint.X) / World.gridLength);
            int startY = (int)Math.Round(Math.Min(Position.Y, endPoint.Y) / World.gridLength);
            int endX = (int)Math.Round(Math.Max(Position.X, endPoint.X) / World.gridLength);
            int endY = (int)Math.Round(Math.Max(Position.Y, endPoint.Y) / World.gridLength);

            bool collides = false;

            for (int k = startY; k != endY + 1; ++k)
            {
                for (int l = startX; l != endX + 1; ++l)
                {
                    for (int j = 0; j != Game1.world.grid[k, l].Count; ++j)
                    {
                        if (ray.IntersectsBox(Game1.world.grid[k, l][j].BoundingRectangle))
                        {
                            collides = true;
                        }
                    }
                }
            }

            if (collides)
            {
                wallAvoidance();
                return true;
            }
            return false;
        }

        private float? closestWallCollide(LineSegment line)
        {
            int startX = (int)Math.Round(Math.Min(line.start.X, line.end.X) / World.gridLength);
            int startY = (int)Math.Round(Math.Min(line.start.Y, line.end.Y) / World.gridLength);
            int endX = (int)Math.Round(Math.Max(line.start.X, line.end.X) / World.gridLength);
            int endY = (int)Math.Round(Math.Max(line.start.Y, line.end.Y) / World.gridLength);

            List<float> collidingDistances = new List<float>();

            for (int k = startY; k != endY + 1; ++k)
            {
                for (int l = startX; l != endX + 1; ++l)
                {
                    for (int j = 0; j != Game1.world.grid[k, l].Count; ++j)
                    {
                        BoundingRectangle currRect = Game1.world.grid[k, l][j].BoundingRectangle;
                        if (line.IntersectsBox(currRect))
                        {
                            float? dist = line.intersectionDistance(currRect);
                            if (dist != null && !collidingDistances.Contains(dist.Value))
                            {
                                collidingDistances.Add(dist.Value);
                            }
                        }
                    }
                }
            }
            if (collidingDistances.Count == 0)
            {
                return null;
            }
            else
            {
                float s = collidingDistances.Min();
                return s;
            }
        }

        private void wallAvoidance()
        {
            Vector2 leftTestDir = Vector2.Transform(physics.Velocity, Matrix.CreateRotationX(MathHelper.ToRadians(AVOIDANCE_ANGLE)));
            if (leftTestDir.Length() > 0)
            {
                leftTestDir.Normalize();
                leftTestDir *= RAY_LENGTH;
            }
            
            float? leftTest = closestWallCollide(new LineSegment(pos,pos+leftTestDir));

            Vector2 rightTestDir = Vector2.Transform(physics.Velocity, Matrix.CreateRotationX(MathHelper.ToRadians(-AVOIDANCE_ANGLE)));
            if (rightTestDir.Length() > 0)
            {
                rightTestDir.Normalize();
                rightTestDir *= RAY_LENGTH;
            }
            float? rightTest = closestWallCollide(new LineSegment(pos, pos + rightTestDir));

            if (rightTest == null && leftTest == null)
            {
                movement.SetTarget(pos + rightTestDir);
                movement.Seek(ref physics);
            }
            else if (rightTest == null)
            {
                movement.SetTarget(pos + rightTestDir);
                movement.Seek(ref physics);
            }
            else if (leftTest == null)
            {
                movement.SetTarget(pos + leftTestDir);
                movement.Seek(ref physics);
            }
            else if (leftTest > rightTest)
            {
                movement.SetTarget(pos + leftTestDir);
                movement.Seek(ref physics);
            }
            else
            {
                movement.SetTarget(pos + rightTestDir);
                movement.Seek(ref physics);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
