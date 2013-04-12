using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public static class AStar
    {
        private class AStarNode : IComparable<AStarNode>
        {
            public Node node;
            public AStarNode cameFrom;
            public float totalCost;
            public float costSoFar;

            public AStarNode(Node n)
            {
                node = n;
                cameFrom = null;
                totalCost = 0;
                costSoFar = 0;
            }

            public int CompareTo(AStarNode obj)
            {
                return Math.Sign(totalCost - obj.totalCost);
            }

            public static bool operator ==(AStarNode l, AStarNode r)
            {
                if ((object)l == null && (object)r == null)
                    return true;
                if ((object)l == null || (object)r == null)
                    return false;
                return l.node == r.node;
            }

            public static bool operator !=(AStarNode l, AStarNode r)
            {
                if ((object)l == null && (object)r == null)
                    return false;
                if ((object)l == null || (object)r == null)
                    return true;
                return l.node != r.node;
            }
        }
        public static List<Node> GetPath(Vector2 start, Vector2 end, List<Node> graph, bool ignoreWeight = false)
        {
            List<Node> path = new List<Node>();

            AStarNode startNode = new AStarNode(FindClosestNode(start, graph));
            AStarNode endNode = new AStarNode(FindClosestNode(end, graph));

            // If either the start or the end is null for some reason, return an empty list
            if (startNode.node == null || endNode.node == null)
                return new List<Node>();

            List<AStarNode> closedList = new List<AStarNode>();
            MinHeap<AStarNode> openList = new MinHeap<AStarNode>();

            startNode.totalCost = getHeuristic(startNode, endNode);
            openList.Enqueue(startNode);

            while (!openList.IsEmpty())
            {
                AStarNode current = openList.Top;
                if (current == endNode)
                    if (ignoreWeight)
                        return constructPath(current, 0, 0);
                    else
                        return constructPath(current, 3, 0.2f);

                openList.Dequeue();
                closedList.Add(current);

                foreach (Edge e in current.node.Edges)
                {
                    AStarNode neighbour = closedList.Find(n => n.node == e.end);
                    float costSoFar = current.costSoFar + e.Cost - (ignoreWeight ? 0 : e.Weight);
                    if (neighbour != null && costSoFar >= neighbour.costSoFar)
                        continue;
                    else neighbour = new AStarNode(e.end);
                    List<AStarNode> openData = openList.data;
                    if (openData.Contains(neighbour))
                    {
                        neighbour = openData.Find(n => n == neighbour);
                        if (costSoFar < neighbour.costSoFar)
                        {
                            neighbour.cameFrom = current;
                            neighbour.costSoFar = costSoFar;
                            neighbour.totalCost = costSoFar + getHeuristic(neighbour, endNode);
                            openList.Clear();
                            foreach (AStarNode n in openData)
                            {
                                openList.Enqueue(n);
                            }
                        }
                    }
                    else
                    {
                        neighbour.cameFrom = current;
                        neighbour.costSoFar = costSoFar;
                        neighbour.totalCost = costSoFar + getHeuristic(neighbour, endNode);
                        openList.Enqueue(neighbour);
                    }
                }
            }

            throw new Exception("AStar is returning in an impossible way");
        }

        private static float getHeuristic(AStarNode n1, AStarNode n2)
        {
            return (n1.node.Position - n2.node.Position).Length();
        }

        private static List<Node> constructPath(AStarNode n, float weight, float weightPer, AStarNode next = null)
        {
            List<Node> path;
            if (n.cameFrom == null)
            {
                if (next != null)
                {
                    n.node.Edges.Find(e => e.end == next.node).Weight += weight;
                    next.node.Edges.Find(e => e.end == n.node).Weight += weight;
                }
                path = new List<Node>();
                path.Add(n.node);
                return path;
            }
            path = constructPath(n.cameFrom, weight - weightPer, weightPer, n);
            if (next != null)
            {
                n.node.Edges.Find(e => e.end == next.node).Weight += weight;
                next.node.Edges.Find(e => e.end == n.node).Weight += weight;
            }
            path.Add(n.node);
            return path;
        }

        private static Node FindClosestNode(Vector2 searchPos, List<Node> nodes)
        {
            float minDist = float.MaxValue;
            Node closest = null;
            foreach (Node n in nodes)
            {
                float dist = (n.Position - searchPos).LengthSquared();
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = n;
                }
            }

            return closest;
        }
    }
}
