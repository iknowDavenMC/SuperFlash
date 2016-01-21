using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;

namespace COMP476Proj
{
    /// <summary>
    /// Node for use with pathfinding
    /// </summary>
    public class Node
    {
        // IDs should be unique and are only actually used by the map loader for attaching edges
        private static int nextID = 0;
        private int id;
        public int ID { get { return id; } }
        private Vector2 position;
        private bool isKey;
        public bool IsKey { get { return isKey; } }
        public Vector2 Position { get { return position; } }
        public List<Edge> Edges; // All associated edges

        public Node(float x, float y, bool isKey=false, int id=0)
        {
            if (id < 1)
            {
                id = ++nextID;
            }
            else
            {
                this.id = id;
                if (nextID <= id)
                    nextID = id + 1;
            }
            position = new Vector2(x, y);
            Edges = new List<Edge>();
            this.isKey = isKey;
        }

        public void Update(GameTime gameTime)
        {
            // Make sure the edges erode
            foreach (Edge edge in Edges)
            {
                edge.Update(gameTime);
            }
        }
    }
}
