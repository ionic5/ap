using System;
using System.Collections.Generic;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;
using UnityEngine.AI;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 전장의 유닛을 나타내는 풀링 가능한 오브젝트.
    /// NavMeshAgent를 이용한 이동, 애니메이션 재생, 데미지/힐 텍스트 표시 등을 담당한다.
    /// IUnit, IDestroyable, IFollowable 인터페이스를 구현한다.
    /// </summary>
    public class Unit : PoolableObject, Core.View.BattleFieldScene.IUnit, IDestroyable, IFollowable
    {
        /// <summary>사망 애니메이션 완료 시 발생하는 이벤트</summary>
        public event EventHandler DieAnimationFinishedEvent;
        /// <summary>이동 방향 변경 시 발생하는 이벤트</summary>
        public event EventHandler MoveDirectionChangedEvent;

        /// <summary>NavMesh 기반 경로 탐색 및 이동 에이전트</summary>
        [SerializeField]
        private NavMeshAgent _agent;
        /// <summary>유닛 애니메이션 컨트롤러</summary>
        [SerializeField]
        private Animator _animator;
        /// <summary>이펙트 표시 기준점 오브젝트</summary>
        [SerializeField]
        private GameObject _effectAreaCenter;
        /// <summary>경로 재설정 최소 거리 임계값</summary>
        [SerializeField]
        private float _pathUpdateThreshold = 0.2f;

        /// <summary>플로팅 텍스트 애니메이터 생성 팩토리 함수</summary>
        public Func<FloatingTextAnimator> CreateFloatingTextAnimator;
        /// <summary>유닛에 연결된 타이머</summary>
        public Core.Timer Timer;

        /// <summary>현재 위치 캐싱용 2D 벡터</summary>
        private System.Numerics.Vector2 _position;
        /// <summary>현재 바라보는 방향</summary>
        private System.Numerics.Vector2 _direction;
        /// <summary>현재 이동 방향</summary>
        private System.Numerics.Vector2 _moveDirection;

        /// <summary>이동 목표 월드 좌표</summary>
        private Vector3 _destination;
        /// <summary>이동 목표가 설정되었는지 여부</summary>
        private bool _isDestinationSetted;

        /// <summary>모션 ID와 애니메이션 클립 이름의 매핑</summary>
        private IReadOnlyDictionary<UnitMotionID, string> _clipNameMap;
        /// <summary>애니메이션 상태 이름 배열</summary>
        private readonly string[] State = {
            "attack",
            "idle",
            "die",
            "walk",
            "cast"
        };

        private void Awake()
        {
            if (_agent == null)
                _agent = GetComponent<NavMeshAgent>();

            _agent.updateRotation = true;
            _agent.updateUpAxis = true;

            _destination = Vector3.zero;
            _position = new System.Numerics.Vector2();
            _isDestinationSetted = false;

            _clipNameMap = new Dictionary<UnitMotionID, string>
            {
                { UnitMotionID.Attack, State[0] },
                { UnitMotionID.Stand, State[1] },
                { UnitMotionID.Die, State[2] },
                { UnitMotionID.Move, State[3] },
                { UnitMotionID.Cast, State[4] }
            };
        }

        private void Update()
        {
            if (!_isDestinationSetted) return;

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    Stop();
                }
            }
            else
            {
                UpdateMoveDirectionFromAgent();
            }
        }

        /// <summary>
        /// NavMeshAgent를 이용하여 지정된 위치로 이동한다. 임계값 이내 거리 변화는 무시한다.
        /// </summary>
        /// <param name="position">목표 위치 2D 좌표</param>
        /// <param name="speed">이동 속도</param>
        public void MoveTo(System.Numerics.Vector2 position, float speed)
        {
            Vector3 targetPos = new Vector3(position.X, transform.position.y, position.Y);

            if (!_isDestinationSetted || Vector3.Distance(_destination, targetPos) > _pathUpdateThreshold)
            {
                _destination = targetPos;
                _isDestinationSetted = true;
                _agent.isStopped = false;
                _agent.speed = speed;
                _agent.SetDestination(_destination);
            }
        }

        /// <summary>
        /// 속도 벡터를 직접 설정하여 유닛을 이동시킨다. 기존 목적지 이동은 중단된다.
        /// </summary>
        /// <param name="velocity">이동 속도 벡터</param>
        public void Move(System.Numerics.Vector2 velocity)
        {
            StopMoveTo();
            _agent.isStopped = false;
            _agent.velocity = new Vector3(velocity.X, 0f, velocity.Y);
        }

        /// <summary>
        /// 유닛의 이동을 멈추고 대기(Stand) 모션을 재생한다.
        /// </summary>
        public void Stop()
        {
            if (_agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
            }
            _isDestinationSetted = false;
            PlayMotion(UnitMotionID.Stand);
        }

        /// <summary>
        /// 목적지 기반 이동만 중단한다. NavMeshAgent 자체는 멈추지 않는다.
        /// </summary>
        private void StopMoveTo()
        {
            _isDestinationSetted = false;
        }

        /// <summary>
        /// NavMeshAgent의 속도로부터 이동 방향을 갱신하고, 변경 시 이벤트를 발생시킨다.
        /// </summary>
        private void UpdateMoveDirectionFromAgent()
        {
            if (_agent.velocity.sqrMagnitude < 0.01f) return;

            Vector3 vel = _agent.velocity.normalized;
            System.Numerics.Vector2 currentDir = new System.Numerics.Vector2(vel.x, vel.z);

            if (System.Numerics.Vector2.Distance(_moveDirection, currentDir) > 0.1f)
            {
                _moveDirection = currentDir;
                SetDirection(_moveDirection);
                MoveDirectionChangedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 유닛의 현재 위치를 2D 좌표로 반환한다.
        /// </summary>
        /// <returns>현재 월드 위치의 2D 좌표 (X, Z를 X, Y로 변환)</returns>
        public System.Numerics.Vector2 GetPosition()
        {
            _position.X = transform.position.x;
            _position.Y = transform.position.z;
            return _position;
        }

        /// <summary>
        /// 유닛의 위치를 2D 좌표로 설정한다. NavMeshAgent가 활성화된 경우 Warp를 사용한다.
        /// </summary>
        /// <param name="position">설정할 위치 (X, Y를 월드 X, Z로 변환)</param>
        public void SetPosition(System.Numerics.Vector2 position)
        {
            Vector3 newPos = new Vector3(position.X, transform.position.y, position.Y);

            if (_agent.enabled)
                _agent.Warp(newPos);
            else
                transform.position = newPos;
        }

        /// <summary>
        /// 유닛이 바라보는 방향을 반환한다.
        /// </summary>
        /// <returns>바라보는 방향 2D 벡터</returns>
        public System.Numerics.Vector2 GetDirection()
        {
            return _direction;
        }

        /// <summary>
        /// 유닛이 바라보는 방향을 설정한다.
        /// </summary>
        /// <param name="direction">설정할 방향 벡터</param>
        public void SetDirection(System.Numerics.Vector2 direction)
        {
            _direction = direction;
        }

        /// <summary>
        /// 지정된 모션을 방향, 재생 시간, 강제 재시작 옵션과 함께 재생한다.
        /// </summary>
        /// <param name="motionID">재생할 모션 식별자</param>
        /// <param name="direction">모션 재생 방향</param>
        /// <param name="playTime">재생 시간 (초)</param>
        /// <param name="forceRestart">이미 같은 모션 재생 중일 때 강제 재시작 여부</param>
        public void PlayMotion(UnitMotionID motionID, System.Numerics.Vector2 direction, float playTime, bool forceRestart)
        {
            SetDirection(direction);
            PlayAnimation(_clipNameMap[motionID], playTime, forceRestart);
        }

        /// <summary>
        /// 현재 방향으로 지정된 모션을 기본 설정으로 재생한다.
        /// </summary>
        /// <param name="motionID">재생할 모션 식별자</param>
        public void PlayMotion(UnitMotionID motionID)
        {
            PlayMotion(motionID, _direction, 1.0f, false);
        }

        /// <summary>
        /// 애니메이션 클립을 재생한다. 이미 동일 클립이 재생 중이면 강제 재시작이 아닌 한 무시한다.
        /// </summary>
        /// <param name="clipName">재생할 클립 이름</param>
        /// <param name="duration">재생 시간 (초, Animator 속도로 변환)</param>
        /// <param name="forceRestart">강제 재시작 여부</param>
        private void PlayAnimation(string clipName, float duration = 1.0f, bool forceRestart = false)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(clipName) && !forceRestart) return;
            _animator.speed = 1.0f / duration;

            _animator.Play(clipName, 0, 0f);
        }

        /// <summary>
        /// NavMeshAgent의 속도를 기반으로 유닛의 이동 방향을 반환한다.
        /// 이동 중이 아니면 영벡터를 반환한다.
        /// </summary>
        /// <returns>현재 이동 방향 벡터</returns>
        public System.Numerics.Vector2 GetMoveDirection()
        {
            Vector3 velocity = _agent.velocity;
            if (velocity.sqrMagnitude > 0f)
            {
                Vector3 normalized = velocity.normalized;
                _moveDirection.X = normalized.x;
                _moveDirection.Y = normalized.y;
            }
            else
            {
                _moveDirection = System.Numerics.Vector2.Zero;
            }
            return _moveDirection;
        }

        /// <summary>
        /// 데미지 수치를 플로팅 텍스트로 유닛 위에 표시한다.
        /// </summary>
        /// <param name="damage">표시할 데미지 값</param>
        public void PlayDamageAnimation(int damage)
        {
            var animator = CreateFloatingTextAnimator.Invoke();
            var offset = _effectAreaCenter.transform.localPosition;
            animator.Follow(this, new System.Numerics.Vector2(offset.x, offset.y));
            animator.PlayDamageAnimation(damage);
            animator.BringToTop();

            EventHandler hdlr = null;
            hdlr = (sender, args) =>
            {
                animator.AnimationFinishedEvent -= hdlr;
                animator.Destroy();
            };
            animator.AnimationFinishedEvent += hdlr;
        }

        /// <summary>
        /// 회복량을 플로팅 텍스트로 유닛 위에 표시한다.
        /// </summary>
        /// <param name="healAmount">표시할 회복량</param>
        public void PlayHealAnimation(int healAmount)
        {
            var animator = CreateFloatingTextAnimator.Invoke();
            var offset = _effectAreaCenter.transform.localPosition;
            animator.Follow(this, new System.Numerics.Vector2(offset.x, offset.y));
            animator.PlayDamageAnimation(healAmount);
            animator.BringToTop();

            EventHandler hdlr = null;
            hdlr = (sender, args) =>
            {
                animator.AnimationFinishedEvent -= hdlr;
                animator.Destroy();
            };
            animator.AnimationFinishedEvent += hdlr;
        }

        /// <summary>
        /// 오브젝트 풀 반환 시 이벤트 구독 및 리소스를 초기화한다.
        /// </summary>
        protected override void CleanUp()
        {
            base.CleanUp();
            DieAnimationFinishedEvent = null;
            CreateFloatingTextAnimator = null;
            if (Timer != null)
            {
                Timer.Destroy();
                Timer = null;
            }
        }

        /// <summary>
        /// 사망 애니메이션 완료 시 애니메이션 이벤트로부터 호출된다.
        /// </summary>
        public void OnDieAnimationFinished()
        {
            DieAnimationFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 유닛의 오브젝트 ID(게임오브젝트 이름)를 반환한다.
        /// </summary>
        /// <returns>게임오브젝트 이름</returns>
        public string GetObjectID()
        {
            return gameObject.name;
        }
    }
}