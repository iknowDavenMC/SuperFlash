using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;

namespace COMP476Proj
{
    /// <summary>
    /// A line segment with a start and end point
    /// </summary>
    public class LineSegment
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
                return Vector2.zero;

            float denom = A * line.B - B * line.A;
            float pX = (B * line.C - line.B * C) / denom;
            float pY = (A * line.C - line.A * C) / denom;

            return new Vector2(pX, pY);
        }

        public float? intersectionDistance(BoundingRectangle rect)
        {
            LineSegment[] side = new LineSegment[4]; 
            side[0] = new LineSegment(rect.Bounds.Left, rect.Bounds.Top, rect.Bounds.Right, rect.Bounds.Top);
            side[1] = new LineSegment(rect.Bounds.Left, rect.Bounds.Bottom, rect.Bounds.Right, rect.Bounds.Bottom);
            side[2] = new LineSegment(rect.Bounds.Left, rect.Bounds.Top, rect.Bounds.Left, rect.Bounds.Bottom);
            side[3] = new LineSegment(rect.Bounds.Right, rect.Bounds.Top, rect.Bounds.Right, rect.Bounds.Bottom);

            Vector2 intersectPt = Vector2.zero;
            float? shortestDist = null;
            float currDist;
            for (int i = 0; i < 4; i++)
            {
                intersectPt = intersection(side[i]);
                currDist = distance(intersectPt);
                if (!intersectPt.Equals(Vector2.zero) && (shortestDist == null || currDist < shortestDist))
                {
                    shortestDist = currDist;
                }
            }

            return shortestDist;
        }

        public float getXfromY(float y)
        {
            return (-C - B * y) / A;
        }

        public float getYfromX(float x)
        {
            return (-C - A * x) / B;
        }

        /// <summary>
        ///  Check collision with a box
        /// </summary>
        /// <param name="rect">Bounding rectangle</param>
        /// <returns>True if there is a collision</returns>
        public bool IntersectsBox(BoundingRectangle box)
        {
            float left = box.Bounds.Left;
            float right = box.Bounds.Right;
            float top = box.Bounds.Top;
            float bottom = box.Bounds.Bottom;

            // if the line's endpoints are on the same side of the rectangle, there can't be an intersection
            if ((left > start.X && left > end.X) ||
                (right< start.X && right < end.X) ||
                (top > start.Y && top > end.Y) ||
                (bottom < start.Y && bottom < end.Y))
                return false;

            // Find the line's X and Y positions at the box's edges
            float xTop = getXfromY(top);
            float xBottom = getXfromY(bottom);
            float yLeft = getYfromX(left);
            float yRight = getYfromX(right);

            // If all of the points are outside the rectangle, there is no intersection
            if (
                (xTop < left || xTop > right) &&
                (xBottom < left || xBottom > right) &&
                (yLeft < top || yLeft > bottom) &&
                (yRight < top || yRight > bottom)
                )
            {
                return false;
            }

            return true;
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
