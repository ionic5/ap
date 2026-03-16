using TaskForce.AP.Client.Core.View.BattleFieldScene;
using System;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 양(Sheep) 형태의 투사체를 나타내는 풀링 가능한 오브젝트.
    /// 목표 지점으로 이동하며, 도착 시 및 충돌 시 이벤트를 발생시킨다.
    /// </summary>
    public class Sheep : PoolableObject, IMissile
    {
        /// <summary>목표 지점 도착 시 발생하는 이벤트</summary>
        public event EventHandler ArrivedDestinationEvent;
        /// <summary>다른 오브젝트와 충돌 시 발생하는 이벤트</summary>
        public event EventHandler<Core.View.HitEventArgs> HitEvent;

        /// <summary>목표 지점 월드 좌표</summary>
        private Vector3 _destination;
        /// <summary>목표 지점 설정 여부</summary>
        private bool _hasDestination;

        /// <summary>물리 이동을 위한 Rigidbody 컴포넌트</summary>
        [SerializeField]
        private Rigidbody _rigidbody;
        /// <summary>도착 판정 거리 임계값</summary>
        [SerializeField]
        private float _arrivalThreshold;

        private void Awake()
        {
            _isDestroyed = false;
            _hasDestination = false;
            _destination = Vector3.zero;
        }

        /// <summary>
        /// 오브젝트 풀 반환 시 이벤트 구독을 초기화한다.
        /// </summary>
        protected override void CleanUp()
        {
            base.CleanUp();
            ArrivedDestinationEvent = null;
            HitEvent = null;
        }

        /// <summary>
        /// 지정된 목표 지점으로 일정 속도로 이동을 시작한다.
        /// </summary>
        /// <param name="destination">목표 지점 2D 좌표</param>
        /// <param name="speed">이동 속도</param>
        public void MoveTo(System.Numerics.Vector2 destination, float speed)
        {
            _hasDestination = true;
            _destination = new Vector3(destination.X, 0, destination.Y);

            Vector3 direction = (_destination - transform.position).normalized;
            _rigidbody.linearVelocity = direction * speed;
        }

        /// <summary>
        /// 투사체의 위치를 2D 좌표로 설정한다.
        /// </summary>
        /// <param name="position">설정할 위치 (X, Y를 월드 X, Z로 변환)</param>
        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, 0, position.Y);
        }

        /// <summary>
        /// 매 프레임 목표 도착 여부를 확인하고, 도착 시 이벤트를 발생시킨다.
        /// </summary>
        private void Update()
        {
            if (!_hasDestination)
                return;

            if (!IsArrived())
                return;

            _hasDestination = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _destination = Vector3.zero;

            ArrivedDestinationEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 현재 위치가 목표 지점에 도착했는지 판정한다.
        /// </summary>
        /// <returns>도착 임계값 이내이면 true</returns>
        private bool IsArrived()
        {
            Vector3 diff = _destination - _rigidbody.position;
            float sqrDist = diff.sqrMagnitude;
            return sqrDist < _arrivalThreshold * _arrivalThreshold;
        }

        /// <summary>
        /// 트리거 충돌 시 충돌 대상의 이름으로 HitEvent를 발생시킨다.
        /// </summary>
        /// <param name="other">충돌한 콜라이더</param>
        private void OnTriggerEnter(Collider other)
        {
            HitEvent?.Invoke(this, new Core.View.HitEventArgs { ObjectID = other.gameObject.name });
        }

        /// <summary>
        /// 투사체의 현재 위치를 2D 좌표로 반환한다.
        /// </summary>
        /// <returns>현재 월드 위치의 2D 좌표 (X, Z를 X, Y로 변환)</returns>
        public System.Numerics.Vector2 GetPosition()
        {
            var pos = transform.position;
            return new System.Numerics.Vector2(pos.x, pos.z);
        }
    }
}