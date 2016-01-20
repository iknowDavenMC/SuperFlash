using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;
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
            if (dir.magnitude > 0)
            {
                dir.Normalize();
                dir *= RAY_LENGTH;
            }

            Vector2 endPoint = pos + dir;
            LineSegment ray = new LineSegment(pos, endPoint);

            // Check the grid for walls
            int startX = (int)Math.Round(Math.Min(Position.x, endPoint.x) / World.gridLength);
            int startY = (int)Math.Round(Math.Min(Position.y, endPoint.y) / World.gridLength);
            int endX = (int)Math.Round(Math.Max(Position.x, endPoint.x) / World.gridLength);
            int endY = (int)Math.Round(Math.Max(Position.y, endPoint.y) / World.gridLength);

            bool collides = false;

            try
            {
                for (int k = startY; k != endY + 1; ++k)
                {
                    for (int l = startX; l != endX + 1; ++l)
                    {
                        for (int j = 0; j != SuperFlashGame.world.grid[k, l].Count; ++j)
                        {
                            if (ray.IntersectsBox(SuperFlashGame.world.grid[k, l][j].BoundingRectangle))
                            {
                                collides = true;
                            }
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {

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
            int startX = (int)Math.Round(Math.Min(line.start.x, line.end.x) / World.gridLength);
            int startY = (int)Math.Round(Math.Min(line.start.y, line.end.y) / World.gridLength);
            int endX = (int)Math.Round(Math.Max(line.start.x, line.end.x) / World.gridLength);
            int endY = (int)Math.Round(Math.Max(line.start.y, line.end.y) / World.gridLength);

            List<float> collidingDistances = new List<float>();

            for (int k = startY; k != endY + 1; ++k)
            {
                for (int l = startX; l != endX + 1; ++l)
                {
                    for (int j = 0; j != SuperFlashGame.world.grid[k, l].Count; ++j)
                    {
                        BoundingRectangle currRect = SuperFlashGame.world.grid[k, l][j].BoundingRectangle;
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
            Vector2 leftTestDir = Quaternion.AngleAxis(AVOIDANCE_ANGLE, Vector3.back) * physics.Velocity;
            if (leftTestDir.magnitude > 0)
            {
                leftTestDir.Normalize();
                leftTestDir *= RAY_LENGTH;
            }
            
            float? leftTest = closestWallCollide(new LineSegment(pos,pos+leftTestDir));

						Vector2 rightTestDir = Quaternion.AngleAxis(AVOIDANCE_ANGLE, Vector3.forward) * physics.Velocity;
            if (rightTestDir.magnitude > 0)
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

        public override void Update()
        {
            base.Update();
        }
    }
}
