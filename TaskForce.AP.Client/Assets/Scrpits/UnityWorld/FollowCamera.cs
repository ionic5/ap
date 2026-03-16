using TaskForce.AP.Client.Core.View.BattleFieldScene;
using Unity.Cinemachine;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 대상 오브젝트를 추적하는 카메라 컴포넌트.
    /// Cinemachine 카메라와 프록시 오브젝트를 사용하여 대상의 위치를 따라간다.
    /// </summary>
    public class FollowCamera : MonoBehaviour, IFollowCamera
    {
        /// <summary>Cinemachine 카메라 인스턴스</summary>
        [SerializeField]
        private CinemachineCamera _camera;
        /// <summary>카메라가 추적할 프록시 게임오브젝트</summary>
        [SerializeField]
        private GameObject _targetProxy;

        /// <summary>프록시의 현재 위치</summary>
        private Vector3 _position;
        /// <summary>카메라가 추적 중인 대상</summary>
        private Core.BattleFieldScene.IFollowable _target;

        private void Awake()
        {
            _position = new Vector3();
            _target = null;
        }

        /// <summary>
        /// 카메라가 추적할 대상을 설정한다.
        /// </summary>
        /// <param name="_unit">추적할 대상 오브젝트</param>
        public void SetTarget(Core.BattleFieldScene.IFollowable _unit)
        {
            _target = _unit;

            UpdateTargetProxy();
        }

        /// <summary>
        /// 카메라의 추적 대상을 해제한다.
        /// </summary>
        public void UnsetTarget()
        {
            _target = null;
        }

        void FixedUpdate()
        {
            UpdateTargetProxy();
        }

        /// <summary>
        /// 추적 대상의 위치를 프록시 오브젝트에 반영하여 카메라가 따라가도록 한다.
        /// </summary>
        private void UpdateTargetProxy()
        {
            if (_target != null)
            {
                System.Numerics.Vector2 position = _target.GetPosition();
                _position.x = position.X;
                _position.z = position.Y;
            }

            _targetProxy.transform.position = _position;
        }
    }
}