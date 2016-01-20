using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public class QuadTree
    {
        private const int TOP_LEFT = 0;
        private const int TOP_RIGHT = 1;
        private const int BOTTOM_LEFT = 3;
        private const int BOTTOM_RIGHT = 2;
        private int x, y, width, height, halfH, halfW, maxDepth;
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public int MaxDepth { get { return maxDepth; } }

        private QuadTree[] cells;
        private List<Wall> objects;

        public QuadTree(int width, int height, int maxDepth)
        {
            x = 0;
            y = 0;
            this.width = width;
            this.height = height;
            this.maxDepth = maxDepth;
            halfH = height / 2;
            halfW = width / 2;
            if (maxDepth > 0)
            {
                cells = new QuadTree[4];
                cells[0] = new QuadTree(0, 0, halfW, halfH, maxDepth - 1);
                cells[1] = new QuadTree(halfW, 0, halfW, halfH, maxDepth - 1);
                cells[2] = new QuadTree(halfW, halfH, halfW, halfH, maxDepth - 1);
                cells[3] = new QuadTree(0, halfH, halfW, halfH, maxDepth - 1);
            }
            objects = new List<Wall>();
        }

        private QuadTree(int x, int y, int width, int height, int maxDepth)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.maxDepth = maxDepth;
            if (maxDepth > 0)
            {
                cells = new QuadTree[4];
                cells[0] = new QuadTree(0, 0, halfW, halfH, maxDepth - 1);
                cells[1] = new QuadTree(halfW, 0, halfW, halfH, maxDepth - 1);
                cells[2] = new QuadTree(halfW, halfH, halfW, halfH, maxDepth - 1);
                cells[3] = new QuadTree(0, halfH, halfW, halfH, maxDepth - 1);
            }
            objects = new List<Wall>();
        }

        public void insert(Wall ent)
        {
            Rectanglef bounds = ent.BoundingRectangle.Bounds;
            if (maxDepth == 0 ||
                (bounds.Left < halfW && bounds.Right >= halfW) ||
                (bounds.Top < halfH && bounds.Bottom >= halfH))
            {
                objects.Add(ent);
                return;
            }
            if (bounds.Left < halfW)
            {
                if (bounds.Top < halfH)
                {
                    cells[TOP_LEFT].insert(ent);
                }
                else
                {
                    cells[BOTTOM_LEFT].insert(ent);
                }
            }
            else
            {
                if (bounds.Top < halfH)
                {
                    cells[TOP_RIGHT].insert(ent);
                }
                else
                {
                    cells[BOTTOM_RIGHT].insert(ent);
                }
            }
        }

        public void getEntities(Vector2 point, ref List<Wall> ret)
        {
            ret.AddRange(objects);
            if (maxDepth > 0)
            {
                if (point.X < halfW)
                {
                    if (point.Y < halfH)
                    {
                        cells[TOP_LEFT].getEntities(point, ref ret);
                    }
                    else
                    {
                        cells[BOTTOM_LEFT].getEntities(point, ref ret);
                    }
                }
                else
                {
                    if (point.Y < halfH)
                    {
                        cells[TOP_RIGHT].getEntities(point, ref ret);
                    }
                    else
                    {
                        cells[BOTTOM_RIGHT].getEntities(point, ref ret);
                    }
                } 
            }
        }

        public void getEntities(BoundingRectangle bounds, ref List<Wall> ret)
        {
            ret.AddRange(objects);
            Rectanglef box = bounds.Bounds;
            if (maxDepth > 0)
            {
                if (box.Left < halfW)
                {
                    if (box.Top < halfH)
                        cells[TOP_LEFT].getEntities(bounds, ref ret);
                    if (box.Bottom >= halfH)
                        cells[BOTTOM_LEFT].getEntities(bounds, ref ret);
                }
                if (box.Right >= halfW)
                {
                    if (box.Top < halfH)
                        cells[TOP_RIGHT].getEntities(bounds, ref ret);
                    if (box.Bottom >= halfH)
                        cells[BOTTOM_RIGHT].getEntities(bounds, ref ret);
                }
            }
        }
    }
}
