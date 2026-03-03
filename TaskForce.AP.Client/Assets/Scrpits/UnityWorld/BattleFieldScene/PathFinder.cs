using System.Collections.Generic;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    public class PathFinder
    {
        private readonly TilemapGrid _grid;
        public LayerMask _collisionTileMask;

        public PathFinder(TilemapGrid grid, LayerMask collisionTileMask)
        {
            _grid = grid;
            _collisionTileMask = collisionTileMask;
        }

        public List<Vector2> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = _grid.NodeFromWorldPoint(startPos);
            Node targetNode = _grid.NodeFromWorldPoint(targetPos);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);

                    if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null;
        }

        List<Vector2> RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Vector2>();

            var currentNode = endNode;
            var nodeSize = _grid.GetNodeSize();
            var halfSize = nodeSize * 0.5f;
            var pivot = new Vector2(halfSize, halfSize);
            while (currentNode != startNode)
            {
                path.Add(new Vector2(currentNode.GridX, currentNode.GridY) * nodeSize + pivot);
                currentNode = currentNode.Parent;
            }

            path.Reverse();

            OptimizePath(path);

            return path;
        }

        List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.GridX + x;
                    int checkY = node.GridY + y;

                    if (checkX >= 0 && checkX < _grid.GetSize().x && checkY >= 0 && checkY < _grid.GetSize().y)
                    {
                        neighbours.Add(_grid.GetNode(checkX, checkY));
                    }
                }
            }
            return neighbours;
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        public List<Vector2> OptimizePath(List<Vector2> originalPath)
        {
            List<Vector2> optimized = new List<Vector2>();

            if (originalPath == null || originalPath.Count == 0)
                return optimized;

            optimized.Add(originalPath[0]);

            int currentIndex = 0;

            while (currentIndex < originalPath.Count - 1)
            {
                int nextIndex = currentIndex + 1;

                for (int i = originalPath.Count - 1; i > nextIndex; i--)
                {
                    Vector2 start = originalPath[currentIndex];
                    Vector2 end = originalPath[i];

                    RaycastHit2D hit = Physics2D.Linecast(start, end, _collisionTileMask);
                    if (hit.collider == null)
                    {
                        nextIndex = i;
                        break;
                    }
                }

                optimized.Add(originalPath[nextIndex]);
                currentIndex = nextIndex;
            }

            return optimized;
        }

    }
}
