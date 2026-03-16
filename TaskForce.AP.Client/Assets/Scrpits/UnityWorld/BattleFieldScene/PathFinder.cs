using System.Collections.Generic;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    /// <summary>
    /// A* 알고리즘을 사용하여 타일맵 그리드 위에서 최단 경로를 탐색하는 클래스.
    /// 탐색된 경로는 Raycast 기반 최적화를 통해 불필요한 중간 지점을 제거한다.
    /// </summary>
    public class PathFinder
    {
        /// <summary>경로 탐색에 사용되는 타일맵 그리드</summary>
        private readonly TilemapGrid _grid;
        /// <summary>경로 최적화 시 충돌 판정에 사용되는 레이어 마스크</summary>
        public LayerMask _collisionTileMask;

        /// <summary>
        /// PathFinder 인스턴스를 생성한다.
        /// </summary>
        /// <param name="grid">경로 탐색 대상 타일맵 그리드</param>
        /// <param name="collisionTileMask">충돌 타일 레이어 마스크</param>
        public PathFinder(TilemapGrid grid, LayerMask collisionTileMask)
        {
            _grid = grid;
            _collisionTileMask = collisionTileMask;
        }

        /// <summary>
        /// 시작 위치에서 목표 위치까지의 최단 경로를 A* 알고리즘으로 탐색한다.
        /// </summary>
        /// <param name="startPos">시작 월드 좌표</param>
        /// <param name="targetPos">목표 월드 좌표</param>
        /// <returns>경로를 구성하는 2D 좌표 리스트. 경로를 찾지 못하면 null</returns>
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

        /// <summary>
        /// 목표 노드에서 시작 노드까지 부모 노드를 역추적하여 경로를 생성한다.
        /// </summary>
        /// <param name="startNode">시작 노드</param>
        /// <param name="endNode">목표 노드</param>
        /// <returns>최적화된 2D 좌표 경로 리스트</returns>
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

        /// <summary>
        /// 지정된 노드의 주변 8방향 이웃 노드들을 반환한다.
        /// </summary>
        /// <param name="node">중심 노드</param>
        /// <returns>유효한 이웃 노드 리스트</returns>
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

        /// <summary>
        /// 두 노드 사이의 거리를 계산한다. 대각선 이동은 14, 직선 이동은 10의 비용을 사용한다.
        /// </summary>
        /// <param name="nodeA">첫 번째 노드</param>
        /// <param name="nodeB">두 번째 노드</param>
        /// <returns>두 노드 사이의 추정 이동 비용</returns>
        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        /// <summary>
        /// Raycast를 이용하여 직선 이동이 가능한 구간의 중간 경유점을 제거하고 경로를 최적화한다.
        /// </summary>
        /// <param name="originalPath">원본 경로 좌표 리스트</param>
        /// <returns>최적화된 경로 좌표 리스트</returns>
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
