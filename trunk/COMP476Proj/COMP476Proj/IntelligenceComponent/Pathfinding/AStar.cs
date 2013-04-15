using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    /// <summary>
    /// Static class to run the A* algorithm
    /// </summary>
    public static class AStar
    {
        /// <summary>
        /// Helper class for A* noeds
        /// </summary>
        private class AStarNode : IComparable<AStarNode>
        {
            public Node node;           // Represented path node
            public AStarNode cameFrom;  // A* node that leads to this one
            public float totalCost;
            public float costSoFar;

            public AStarNode(Node n)
            {
                node = n;
                cameFrom = null;
                totalCost = 0;
                costSoFar = 0;
            }

            // Used for comparing two nodes by the MinHeap
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

        /// <summary>
        /// Get a path from a start point to an end point using a set list of nodes
        /// </summary>
        /// <param name="start">Position to start at</param>
        /// <param name="end">Position to end at</param>
        /// <param name="graph">List of nodes to use</param>
        /// <param name="ignoreWeight">If true, extra weights will no be considered in the calculation or added to the edges</param>
        /// <returns>A list of nodes representing the path to take</returns>
        public static List<Node> GetPath(Vector2 start, Vector2 end, List<Node> graph, List<Wall>[,] grid, bool checkWalls = true, bool ignoreWeight = true)
        {
            List<Node> path = new List<Node>();
            AStarNode startNode = new AStarNode(FindClosestNode(start, ref graph, ref grid, checkWalls));
            AStarNode endNode = new AStarNode(FindClosestNode(end, ref graph, ref grid, checkWalls));

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
                // When the node being considered is the end node, stop
                if (current == endNode)
                    if (ignoreWeight)
                        return constructPath(current, 0, 0);
                    else
                        return constructPath(current, 3, 0.2f);

                openList.Dequeue();
                closedList.Add(current);

                int edgeC = current.node.Edges.Count;
                for(int i=0; i!= edgeC; ++i) {
                    Edge e = current.node.Edges[i];
                    AStarNode neighbour = closedList.Find(n => n.node == e.end);
                    float costSoFar = current.costSoFar + e.Cost - (ignoreWeight ? 0 : e.Weight);

                    if (neighbour != null)
                    {
                        // If the neighbouring node is already in the closed list and its 
                        // cost is less than this one, skip it
                        if (costSoFar >= neighbour.costSoFar)
                            continue;
                    }
                    else neighbour = new AStarNode(e.end);

                    // If the node is already in the open list, find it and replace it if the new cost is lower
                    if (openList.data.Contains(neighbour))
                    {
                        List<AStarNode> openData = openList.data;
                        neighbour = openData.Find(n => n == neighbour);
                        if (costSoFar < neighbour.costSoFar)
                        {
                            neighbour.cameFrom = current;
                            neighbour.costSoFar = costSoFar;
                            neighbour.totalCost = costSoFar + getHeuristic(neighbour, endNode);

                            // If the node is replaced, the heap has to be rebuilt
                            openList.Clear();
                            int nodeC = openData.Count;
                            for(int j=0; j!=nodeC; ++j)
                            {
                                openList.Enqueue(openData[j]);
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

        /// <summary>
        /// Get the heuristic value between two nodes (ie: Euclidean distance)
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        private static float getHeuristic(AStarNode n1, AStarNode n2)
        {
            return (n1.node.Position - n2.node.Position).Length();
        }

        /// <summary>
        /// Construct the path recursively starting with the last node
        /// </summary>
        /// <param name="n">Current node</param>
        /// <param name="weight">Current weight value</param>
        /// <param name="weightPer">Additional weight per node in the path</param>
        /// <param name="next">Next node in the path</param>
        /// <returns>The final path of nodes</returns>
        private static List<Node> constructPath(AStarNode n, float weight, float weightPer, AStarNode next = null)
        {
            List<Node> path;
            // If this node didn't come from anywhere, it's the start.
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
            // recursion
            path = constructPath(n.cameFrom, weight - weightPer, weightPer, n);
            if (next != null)
            {
                n.node.Edges.Find(e => e.end == next.node).Weight += weight;
                next.node.Edges.Find(e => e.end == n.node).Weight += weight;
            }
            path.Add(n.node);
            return path;
        }

        /// <summary>
        /// Find the closest node to a point
        /// </summary>
        /// <param name="searchPos"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static Node FindClosestNode(Vector2 searchPos, ref List<Node> nodes, ref List<Wall>[,] grid, bool checkWalls)
        {
            float minDist = float.MaxValue;
            Node closest = null;
            int nodeC = nodes.Count;
            for(int n=0; n!= nodeC; ++n)
            {
                Node node = nodes[n];
                if (grid != null && checkWalls)
                {
                    LineSegment line = new LineSegment(searchPos, node.Position);
                    bool blocked = false;

                    // Check every wall
                    //int wallC = grid.Count;
                    //for (int i = 0; i != wallC; ++i)
                    //{
                    //    if (line.IntersectsBox(grid[i].BoundingRectangle))
                    //    {
                    //        blocked = true;
                    //        break;
                    //    }
                    //}

                    // Check the grid for walls
                    int minGX = (int)Math.Min(searchPos.X, node.Position.X) / 200;
                    int minGY = (int)Math.Min(searchPos.Y, node.Position.Y) / 200;
                    int maxGX = (int)Math.Max(searchPos.X, node.Position.X) / 200;
                    int maxGY = (int)Math.Max(searchPos.Y, node.Position.Y) / 200;
                    if (minGX > 0)
                        --minGX;
                    if (minGY > 0)
                        --minGY;
                    if (maxGX > grid.GetUpperBound(1))
                        ++maxGX;
                    if (maxGY > grid.GetUpperBound(0))
                        ++maxGY;
                    for (int i = minGX; i <= maxGX; ++i)
                    {
                        for (int j = minGY; j <= maxGY; ++j)
                        {
                            List<Wall> walls = grid[j, i];
                            int wallC = walls.Count;
                            for (int k = 0; k != wallC; ++k)
                            {
                                Wall wall = walls[k];
                                if (wall.IsSeeThrough)
                                    continue;
                                if (line.IntersectsBox(wall.BoundingRectangle))
                                {
                                    blocked = true;
                                    break;
                                }
                            }
                            if (blocked)
                                break;
                        }
                        if (blocked)
                            break;
                    }

                    if (blocked)
                        continue;
                }
                float dist = (node.Position - searchPos).LengthSquared();
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = node;
                }
            }

            return closest;
        }
    }
}
