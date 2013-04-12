using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public class Edge
    {
        public Node start;
        public Node end;
        private float cost;
        public float Cost { get { return cost + Weight; } }
        public float Weight;
        private static float degradeRate = 0.5f; // (per second)

        public Edge(Node s, Node e)
        {
            start = s;
            end = e;
            cost = (e.Position - s.Position).Length();
            Weight = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (Weight != 0)
            {
                float time = gameTime.ElapsedGameTime.Milliseconds;
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
