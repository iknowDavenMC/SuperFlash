using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace COMP476Proj
{
    public class Node
    {
        #region Fields
        public static int nodeCount = 0;
        private int nodeID;
        private Vector2 position;
        public List<Node> friendList = new List<Node>();
        #endregion

        #region Properties
        public Vector2 Position
        {
            get { return position; }
        }
        public int NodeID
        {
            get { return nodeID; }
        }
        #endregion

        #region Constructor
        public Node(int id, float xPos, float yPos)
        {
            nodeID = id;
            position.X = xPos;
            position.Y = yPos;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return "Node ID: " + nodeID;
        }
        #endregion
    }
}
