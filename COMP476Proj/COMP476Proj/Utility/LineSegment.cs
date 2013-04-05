using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    /// <summary>
    /// A line segment with a start and end point
    /// </summary>
    class LineSegment
    {
        public Vector2 start { get; private set; }
        public Vector2 end { get; private set; }

        // Values of the general equation: Ax + By + C = 0
        public float A { get; private set; }
        public float B { get; private set; }
        public float C { get; private set; }

        /// <summary>
        /// Line Segment constructor
        /// </summary>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        public LineSegment(Vector2 start, Vector2 end)
        {
            this.start = new Vector2(start.X, start.Y);
            this.end = new Vector2(end.X, end.Y);
            init();
        }

        /// <summary>
        /// Line segment constructor
        /// </summary>
        /// <param name="sx">Start X</param>
        /// <param name="sy">Start Y</param>
        /// <param name="ex">End X</param>
        /// <param name="ey">End Y</param>
        public LineSegment(float sx, float sy, float ex, float ey)
        {
            start = new Vector2(sx, sy);
            end = new Vector2(ex, ey);
            init();
        }

        /// <summary>
        /// Initialize the linear equation
        /// </summary>
        private void init()
        {
            A = start.Y - end.Y;
            B = end.X - start.X;
            C = start.X * end.Y - start.Y * end.X;
        }

        /// <summary>
        /// Check if two lines are parallel
        /// </summary>
        /// <param name="line">Line to check against</param>
        /// <returns>True if the lines are parallel</returns>
        public bool isParallel(LineSegment line)
        {
            return (A * line.B == B * line.A);
        }

        /// <summary>
        /// Get the intersection point of two lines
        /// </summary>
        /// <param name="line">Line to intersect</param>
        /// <returns>The intersection point</returns>
        public Vector2 intersection(LineSegment line)
        {
            if (isParallel(line))
                return Vector2.Zero;

            float denom = A * line.B - B * line.A;
            float pX = (B * line.C - line.B * C) / denom;
            float pY = (A * line.C - line.A * C) / denom;

            return new Vector2(pX, pY);
        }

        /// <summary>
        ///  Check collision with a box
        /// </summary>
        /// <param name="rect">Bounding rectangle</param>
        /// <returns>True if there is a collision</returns>
        private bool collidesBL(BoundingRectangle rect)
        {
            Rectanglef box = rect.Bounds;
            // Convert each side into a line
            LineSegment l = new LineSegment(box.Left, box.Top, box.Left, box.Bottom);
            LineSegment r = new LineSegment(box.Right, box.Bottom, box.Right, box.Top);
            LineSegment t = new LineSegment(box.Right, box.Top, box.Left, box.Top);
            LineSegment b = new LineSegment(box.Left, box.Bottom, box.Right, box.Bottom);

            // Only check for collision if the lines are not parallel
            if (!isParallel(l))
            {
                float yPos = (-A * box.Left - C) / B;
                if (yPos > box.Top && yPos < box.Bottom &&
                    yPos > Math.Min(start.Y, end.Y) &&
                    yPos < Math.Max(start.Y, end.Y))
                    return true;
            }
            if (!isParallel(r))
            {
                float yPos = (-A * box.Right - C) / B;
                if (yPos > box.Top && yPos < box.Bottom &&
                    yPos > Math.Min(start.Y, end.Y) &&
                    yPos < Math.Max(start.Y, end.Y))
                    return true;
            }
            if (!isParallel(t))
            {
                float xPos = (-B * box.Top - C) / A;
                if (xPos > box.Left && xPos < box.Right &&
                    xPos > Math.Min(start.X, end.X) &&
                    xPos < Math.Max(start.X, end.X))
                    return true;
            }
            if (!isParallel(b))
            {
                float xPos = (-B * box.Bottom - C) / A;
                if (xPos > box.Left && xPos < box.Right &&
                    xPos > Math.Min(start.X, end.X) &&
                    xPos < Math.Max(start.X, end.X))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get the perpendicular distance of a point to the line
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>The distance of the point</returns>
        public float distance(Vector2 point)
        {
            return (float)(Math.Abs(A * point.X + B * point.Y + C) / Math.Sqrt(A * A + B * B));
        }
    }
}
