using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    /// <summary>
    /// Edge for use with pathfinding. Note: when attaching an edge to a node, be sure to add the reverse to the end node
    /// </summary>
    public class Edge
    {
        public Node start;
        public Node end;
        private float cost;
        public float Cost { get { return cost + Weight; } }
        public float Weight; // Option weight to add to the cost
        private static float degradeRate = 0.5f; // (per second)

        public Edge(Node s, Node e)
        {
            start = s;
            end = e;
            cost = (e.Position - s.Position).magnitude;
            Weight = 0;
        }

        public void Update(GameTime gameTime)
        {
            // Over time if the node has a weight, erode it to zero.
            if (Weight != 0)
            {
                float time = Time.deltaTime * 1000f;
                if (Weight > 0)
                {
                    Weight -= degradeRate * time/1000;
                    if (Weight < 0)
                        Weight = 0;
                }
                else if (Weight < 0)
                {
                    Weight += degradeRate * time / 1000;
                    if (Weight > 0)
                        Weight = 0;
                }
            }
        }

    }
}
