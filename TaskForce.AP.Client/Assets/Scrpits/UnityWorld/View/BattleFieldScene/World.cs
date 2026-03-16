using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 전장 월드를 관리하는 MonoBehaviour 클래스.
    /// 유닛 스폰 위치 관리, 카메라 뷰 판정, 랜덤 스폰 위치 제공 등을 담당한다.
    /// </summary>
    public class World : MonoBehaviour, Core.View.BattleFieldScene.IWorld
    {
        /// <summary>플레이어 유닛 스폰 기본 위치 오브젝트</summary>
        [SerializeField]
        private GameObject _playerUnitSpawnPosition;
        /// <summary>스폰 위치 그룹 부모 오브젝트</summary>
        [SerializeField]
        private GameObject _spawnPositionSet;
        /// <summary>카메라 뷰 판정용 카메라</summary>
        [SerializeField]
        private Camera _camera;
        /// <summary>스폰 영역 너비</summary>
        [SerializeField]
        private float _spawnAreaWidth;
        /// <summary>스폰 중심으로부터의 랜덤 오프셋 반경</summary>
        [SerializeField]
        private float _spawnRadius;
        /// <summary>유닛 최대 반경</summary>
        [SerializeField]
        private float _unitMaxRadius;

        /// <summary>랜덤 값 생성기</summary>
        public Core.Random Random;

        /// <summary>모든 스폰 위치 목록</summary>
        private List<Vector3> _spawnPositions;

        private void Awake()
        {
            _spawnPositions = new List<Vector3>();
            for (int i = 0; i < _spawnPositionSet.transform.childCount; i++)
            {
                var parentTransform = _spawnPositionSet.transform.GetChild(i);
                for (int j = 0; j < parentTransform.childCount; j++)
                    _spawnPositions.Add(parentTransform.GetChild(j).position);
            }
        }

        /// <summary>
        /// 카메라 밖의 랜덤 스폰 위치를 2D 좌표로 반환한다.
        /// </summary>
        /// <returns>랜덤 워프 위치 2D 좌표</returns>
        public System.Numerics.Vector2 GetWarpPoint()
        {
            Vector3 pos = GetRandomSpawnPosition();
            return new System.Numerics.Vector2(pos.x, pos.z);
        }

        /// <summary>
        /// 지정된 2D 위치가 카메라 뷰 밖에 있는지 확인한다.
        /// </summary>
        /// <param name="position">확인할 2D 좌표</param>
        /// <returns>카메라 뷰 밖이면 true</returns>
        public bool IsOutOfCameraView(System.Numerics.Vector2 position)
        {
            return IsOutOfCameraView(new Vector3(position.X, 0, position.Y));
        }

        /// <summary>
        /// 지정된 월드 좌표가 카메라 뷰포트 밖에 있는지 확인한다.
        /// </summary>
        /// <param name="worldPos">확인할 월드 좌표</param>
        /// <returns>뷰포트 밖이면 true</returns>
        private bool IsOutOfCameraView(Vector3 worldPos)
        {
            Vector3 viewportPos = _camera.WorldToViewportPoint(worldPos);

            return viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1 || viewportPos.z < 0;
        }

        /// <summary>
        /// 카메라 밖 스폰 위치 중 하나를 랜덤으로 선택하고, 반경 내 오프셋을 적용하여 반환한다.
        /// 사용 가능한 위치가 없으면 플레이어 스폰 위치를 반환한다.
        /// </summary>
        /// <returns>랜덤 스폰 월드 좌표</returns>
        private Vector3 GetRandomSpawnPosition()
        {
            var centerPosList = GetAvailableSpawnPositions().ToList();
            if (centerPosList.Count == 0) return _playerUnitSpawnPosition.transform.position;

            Vector3 centerPos = centerPosList[Random.Next(centerPosList.Count)];

            var randomOffset = Random.NextPosition(System.Numerics.Vector2.Zero, _spawnRadius);
            Vector3 spawnPos = new Vector3(centerPos.x + randomOffset.X, centerPos.y, centerPos.z + randomOffset.Y);

            return spawnPos;
        }

        /// <summary>
        /// 카메라 뷰 밖에 있는 사용 가능한 스폰 위치 목록을 반환한다.
        /// </summary>
        /// <returns>카메라 밖 스폰 위치 컬렉션</returns>
        private IEnumerable<Vector3> GetAvailableSpawnPositions()
        {
            var positions = new List<Vector3>();
            foreach (var pos in _spawnPositions)
                if (IsOutOfCameraView(pos))
                    positions.Add(pos);

            return positions;
        }

        /// <summary>
        /// 플레이어 유닛의 기본 스폰 위치를 2D 좌표로 반환한다.
        /// </summary>
        /// <returns>플레이어 스폰 위치 2D 좌표</returns>
        public System.Numerics.Vector2 GetPlayerUnitSpawnPosition()
        {
            var pos = _playerUnitSpawnPosition.transform.position;
            return new System.Numerics.Vector2(pos.x, pos.z);
        }
    }
}
