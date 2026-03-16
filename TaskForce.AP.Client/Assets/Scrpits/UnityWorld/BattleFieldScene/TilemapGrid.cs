using UnityEngine;
using UnityEngine.Tilemaps;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    /// <summary>
    /// 타일맵 기반 그리드 시스템을 관리하는 MonoBehaviour 클래스.
    /// 충돌 레이어의 타일 정보를 기반으로 이동 가능 여부가 설정된 노드 그리드를 생성한다.
    /// </summary>
    public class TilemapGrid : MonoBehaviour
    {
        /// <summary>그리드의 가로/세로 크기</summary>
        [SerializeField]
        private Vector2Int _gridSize;
        /// <summary>충돌 판정용 타일맵 레이어</summary>
        [SerializeField]
        private Tilemap _collisionLayer;
        /// <summary>각 노드의 월드 공간 크기</summary>
        [SerializeField]
        private float _nodeSize = 1f;

        /// <summary>노드 2차원 배열 (그리드)</summary>
        private Node[,] grid;

        /// <summary>
        /// 충돌 타일맵 정보를 기반으로 노드 그리드를 초기화한다.
        /// 타일이 없는 위치는 이동 가능(walkable)으로 설정된다.
        /// </summary>
        public void Awake()
        {
            grid = new Node[_gridSize.x, _gridSize.y];
            Vector3Int tileOrigin = _collisionLayer.origin;

            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    Vector3Int tilePos = new Vector3Int(tileOrigin.x + x, tileOrigin.y + y, 0);

                    TileBase collisionTile = _collisionLayer.GetTile(tilePos);
                    bool isWalkable = collisionTile == null;

                    grid[x, y] = new Node(isWalkable, x, y);
                }
            }
        }

        /// <summary>
        /// 노드 하나의 월드 공간 크기를 반환한다.
        /// </summary>
        /// <returns>노드 크기</returns>
        public float GetNodeSize()
        {
            return _nodeSize;
        }

        /// <summary>
        /// 그리드의 가로/세로 크기를 반환한다.
        /// </summary>
        /// <returns>그리드 크기 (x: 가로, y: 세로)</returns>
        public Vector2Int GetSize()
        {
            return _gridSize;
        }

        /// <summary>
        /// 월드 좌표를 그리드 좌표로 변환하여 해당 노드를 반환한다.
        /// 범위를 벗어난 좌표는 그리드 경계로 클램프된다.
        /// </summary>
        /// <param name="worldPosition">월드 좌표</param>
        /// <returns>해당 위치의 노드</returns>
        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / _nodeSize);
            int y = Mathf.FloorToInt(worldPosition.y / _nodeSize);

            x = Mathf.Clamp(x, 0, _gridSize.x - 1);
            y = Mathf.Clamp(y, 0, _gridSize.y - 1);

            return grid[x, y];
        }

        /// <summary>
        /// 지정된 그리드 좌표의 노드를 반환한다.
        /// </summary>
        /// <param name="x">그리드 X 좌표</param>
        /// <param name="y">그리드 Y 좌표</param>
        /// <returns>해당 좌표의 노드</returns>
        public Node GetNode(int x, int y)
        {
            return grid[x, y];
        }
    }
}
