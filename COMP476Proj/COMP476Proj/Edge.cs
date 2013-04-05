using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP476Proj
{
    public class Edge
    {
        public Node prevNode;
        public Node currNode;
        public float costSoFar;
        public float estimCost;
        public Edge(Node prevNode, Node currNode, float costSoFar, float estCost)
        {
            this.prevNode = prevNode;
            this.currNode = currNode;
            this.costSoFar = costSoFar;
            estimCost = estCost;
        }

        public bool Equals(Edge e){
            return prevNode.Equals(e.prevNode) && currNode.Equals(e.currNode) &&
                costSoFar == e.costSoFar && estimCost == e.estimCost;
        }

        public override string ToString()
        {
            return "["+currNode.NodeID+","+costSoFar+","+prevNode.NodeID+"->"+currNode.NodeID+","+estimCost+"]";
        }
    }
}
