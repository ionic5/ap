using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class World : MonoBehaviour, Core.View.BattleFieldScene.IWorld
    {
        [SerializeField]
        private GameObject _playerUnitSpawnPosition;
        [SerializeField]
        private GameObject _spawnPositionSet;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private float _spawnAreaWidth;
        [SerializeField]
        private float _spawnRadius;
        [SerializeField]
        private float _unitMaxRadius;
        [SerializeField]
        private float _maxSpawnPositionAttempts;
        [SerializeField]
        private LayerMask _collisionTileLayerMask;

        public Core.Random Random;

        private List<Vector2> _spawnPositions;

        private void Awake()
        {
            _spawnPositions = new List<Vector2>();
            for (int i = 0; i < _spawnPositionSet.transform.childCount; i++)
            {
                var transform = _spawnPositionSet.transform.GetChild(i);
                for (int j = 0; j < transform.childCount; j++)
                    _spawnPositions.Add(transform.GetChild(j).position);
            }
        }

        public System.Numerics.Vector2 GetWarpPoint()
        {
            return GetRandomSpawnPosition();
        }

        public bool IsOutOfCameraView(System.Numerics.Vector2 position)
        {
            Rect viewPortRect = GetCameraViewportRect();
            return !viewPortRect.Contains(new Vector2(position.X, position.Y));
        }

        private System.Numerics.Vector2 GetRandomSpawnPosition()
        {
            var centerPosList = GetSpawnPositions();

            var centerPos = centerPosList.ElementAt(Random.Next(centerPosList.Count()));
            for (int i = 0; i < _maxSpawnPositionAttempts; i++)
            {
                var pos = Random.NextPosition(new System.Numerics.Vector2(centerPos.x, centerPos.y), _spawnRadius);
                if (Physics2D.OverlapCircle(new Vector2(pos.X, pos.Y), _unitMaxRadius, _collisionTileLayerMask) == null)
                    return pos;
                centerPos = centerPosList.ElementAt(Random.Next(centerPosList.Count()));
            }

            return new System.Numerics.Vector2(centerPos.x, centerPos.y);
        }


        private Rect GetCameraViewportRect()
        {
            Vector3 bottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
            Vector3 topRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));

            var viewPortRect = new Rect(
                bottomLeft.x,
                bottomLeft.y,
                topRight.x - bottomLeft.x,
                topRight.y - bottomLeft.y
            );
            return viewPortRect;
        }

        private IEnumerable<Vector2> GetSpawnPositions()
        {
            Rect viewPortRect = GetCameraViewportRect();

            var areaRect = new Rect(viewPortRect.x - _spawnAreaWidth, viewPortRect.y - _spawnAreaWidth,
                viewPortRect.width + _spawnAreaWidth * 2, viewPortRect.height + _spawnAreaWidth * 2);

            var positions = new List<Vector2>();

            foreach (var position in _spawnPositions)
            {
                bool insideArea = areaRect.Contains(position);
                bool insideRect = viewPortRect.Contains(position);

                if (insideArea && !insideRect)
                    positions.Add(position);
            }

            return positions;
        }

        public System.Numerics.Vector2 GetPlayerUnitSpawnPosition()
        {
            var pos = _playerUnitSpawnPosition.transform.position;
            return new System.Numerics.Vector2(pos.x, pos.y);
        }
    }
}
