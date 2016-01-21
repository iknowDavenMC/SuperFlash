using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;

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

        public override void Update()
        {
            rect.Update(physics.Position);
        }

        /*
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
         * */

        public override void ResolveCollision(Entity other)
        {
            Rectanglef overlap = Rectanglef.Intersect(rect.Bounds, other.BoundingRectangle.Bounds);

            if (overlap.X == 0 && overlap.Y == 0 && overlap.Width == 0 && overlap.Height == 0)
            {
                // No collision
                return;
            }

            isColliding = true;
            other.isColliding = true;

            // Handle collision
            if (other is EntityMoveable)
            {
                float mass1 = ComponentPhysics.Mass;
                float mass2 = ((EntityMoveable)other).ComponentPhysics.Mass;

                if (mass1 > mass2)
                {
                    if (other is Pedestrian && this is Streaker && ((Pedestrian)other).State != PedestrianState.FALL)
                    {
                        DataManager.GetInstance().IncreaseScore(DataManager.Points.KnockDownPed, true,
                            ((EntityMoveable)other).ComponentPhysics.Position.x,
                            ((EntityMoveable)other).ComponentPhysics.Position.y - 64);
                        DataManager.GetInstance().numberPedestriansKnockedOver++;
                    }
                    if (other is DumbCop && this is Streaker && ((DumbCop)other).State != DumbCopState.FALL)
                    {
                        DataManager.GetInstance().IncreaseScore(DataManager.Points.KnockDownCop, true,
                            ((EntityMoveable)other).ComponentPhysics.Position.x,
                            ((EntityMoveable)other).ComponentPhysics.Position.y - 64);
                        DataManager.GetInstance().numberCopsKnockedOver++;
                    }
                    if (other is SmartCop && this is Streaker && ((SmartCop)other).State != SmartCopState.FALL)
                    {
                        DataManager.GetInstance().IncreaseScore(DataManager.Points.KnockDownCop, true,
                            ((EntityMoveable)other).ComponentPhysics.Position.x,
                            ((EntityMoveable)other).ComponentPhysics.Position.y - 64);
                        DataManager.GetInstance().numberCopsKnockedOver++;
                    }

                    if (!(other is Streaker && ((Streaker)other).IsGhost))
                    {
                        ((EntityMoveable)other).Fall(false);
                    }
                }
                if (mass1 < mass2)
                {
                    if (other is Streaker && this is Pedestrian && ((Pedestrian)this).State != PedestrianState.FALL)
                    {
                        DataManager.GetInstance().IncreaseScore(DataManager.Points.KnockDownPed, true, physics.Position.x, physics.Position.y - 64);
                        DataManager.GetInstance().numberPedestriansKnockedOver++;
                    }
                    if (other is Streaker && this is DumbCop && ((DumbCop)this).State != DumbCopState.FALL)
                    {
                        DataManager.GetInstance().IncreaseScore(DataManager.Points.KnockDownCop, true, physics.Position.x, physics.Position.y - 64);
                        DataManager.GetInstance().numberCopsKnockedOver++;
                    }
                    if (other is Streaker && this is SmartCop && ((SmartCop)this).State != SmartCopState.FALL)
                    {
                        DataManager.GetInstance().IncreaseScore(DataManager.Points.KnockDownCop, true, physics.Position.x, physics.Position.y - 64);
                        DataManager.GetInstance().numberCopsKnockedOver++;
                    }


                    if (!(this is Streaker && ((Streaker)this).IsGhost))
                    {
                        ((EntityMoveable)this).Fall(false);
                    }
                }

                if (!(this is Streaker && ((Streaker)this).IsGhost))
                {
                    physics.ResolveCollision(((EntityMoveable)other).physics, overlap);
                }

                if (!(other is Streaker && ((Streaker)other).IsGhost))
                {
                    ((EntityMoveable)other).ComponentPhysics.ResolveCollision(physics, overlap);
                }

                if (!(this is Streaker && ((Streaker)this).IsGhost))
                {
                    physics.ResolveInterPenetration(overlap, rect.Bounds);
                }

                if (!(other is Streaker && ((Streaker)other).IsGhost))
                {
                    ((EntityMoveable)other).ComponentPhysics.ResolveInterPenetration(overlap, other.BoundingRectangle.Bounds);
                }
            }
            else if (other is Wall)
            {
                physics.ResolveWallCollision(overlap);
                physics.ResolveInterPenetrationWall(overlap, rect.Bounds);
            }
        }

        /// <summary>
        /// Fall
        /// </summary>
        public virtual void Fall(bool isSuperFlash) { }

        public virtual void IsVisible(Vector2 position, out bool canSee, out bool canReach)
        {
            LineSegment lineTest = new LineSegment(Position, position);

            bool obstacle;
            bool isSeeThrough;

            canReach = true;
            canSee = true;

            // Check the grid for walls
            int startX = (int)Math.Round(Math.Min(Position.x, position.x) / World.gridLength);
            int startY = (int)Math.Round(Math.Min(Position.y, position.y) / World.gridLength);
            int endX = (int)Math.Round(Math.Max(Position.x, position.x) / World.gridLength);
            int endY = (int)Math.Round(Math.Max(Position.y, position.y) / World.gridLength);

            try
            {
                for (int k = startY; k != endY + 1; ++k)
                {
                    for (int l = startX; l != endX + 1; ++l)
                    {
                        for (int j = 0; j != SuperFlashGame.world.grid[k, l].Count; ++j)
                        {
                            obstacle = lineTest.IntersectsBox(SuperFlashGame.world.grid[k, l][j].BoundingRectangle);
                            isSeeThrough = SuperFlashGame.world.grid[k, l][j].IsSeeThrough;

                            if (obstacle && isSeeThrough)
                            {
                                canReach = false;
                            }
                            else if (obstacle && !isSeeThrough)
                            {
                                canReach = false;
                                canSee = false;
                            }

                            if (!canReach && !canSee)
                            {
                                return;
                            }
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                //Console.WriteLine("BAD POSITION: " + physics.Position.X + ", " + physics.Position.Y);
            }
        }

        public void OptimizePath(ref List<Node> path)
        {
            bool canSee;
						bool canReach = false;

            if (path.Count > 2)
            {
                IsVisible(path[1].Position, out canSee, out canReach);
            }
            else
            {
                return;
            }

            // Optimize
            while (path.Count > 2 && canReach)
            {
                path.RemoveAt(0);
                IsVisible(path[1].Position, out canSee, out canReach);
            }
        }

        public Vector2 GetKeyNode()
        {
            float distance = float.MaxValue;
            Vector2 position = Vector2.zero;

            foreach (Node node in SuperFlashGame.world.map.nodes)
            {
                if (!node.IsKey)
                {
                    continue;
                }

                bool canReach;
                bool canSee;

                IsVisible(node.Position, out canSee, out canReach);

                if (canReach && (Position - Position).magnitude < distance)
                {
										distance = (Position - Position).magnitude;
                    position = node.Position;
                }
            }

            return position;
        }

        #endregion
    }
}
