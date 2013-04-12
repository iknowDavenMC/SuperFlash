using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public class Node
    {
        private static int nextID = 0;
        private int id;
        public int ID { get { return id; } }
        private Vector2 position;
        public Vector2 Position { get { return position; } }
        public List<Edge> Edges;

        public Node(float x, float y, int id=0)
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
        }

        public void Update(GameTime gameTime)
        {
            foreach (Edge edge in Edges)
            {
                edge.Update(gameTime);
            }
        }
    }
}
