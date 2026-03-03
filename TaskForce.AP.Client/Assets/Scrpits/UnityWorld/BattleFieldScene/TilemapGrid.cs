using UnityEngine;
using UnityEngine.Tilemaps;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    public class TilemapGrid : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int _gridSize;
        [SerializeField]
        private Tilemap _collisionLayer;
        [SerializeField]
        private float _nodeSize = 1f;

        private Node[,] grid;

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

        public float GetNodeSize()
        {
            return _nodeSize;
        }

        public Vector2Int GetSize()
        {
            return _gridSize;
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / _nodeSize);
            int y = Mathf.FloorToInt(worldPosition.y / _nodeSize);

            x = Mathf.Clamp(x, 0, _gridSize.x - 1);
            y = Mathf.Clamp(y, 0, _gridSize.y - 1);

            return grid[x, y];
        }

        public Node GetNode(int x, int y)
        {
            return grid[x, y];
        }
    }
}
