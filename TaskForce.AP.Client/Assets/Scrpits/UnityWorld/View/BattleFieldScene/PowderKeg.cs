using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 화약통 오브젝트를 나타내는 풀링 가능한 클래스.
    /// 주기적으로 주변 오브젝트를 감지하고, 점화 시 폭발 이벤트를 발생시킨다.
    /// 대기 중에는 임의 간격으로 깜빡임 애니메이션을 재생한다.
    /// </summary>
    public class PowderKeg : PoolableObject, IPowderKeg
    {
        /// <summary>주변 오브젝트 감지 시 발생하는 이벤트</summary>
        public event EventHandler<BatchObjectDetectedEventArgs> BatchObjectDetectedEvent;
        /// <summary>점화 완료(폭발) 시 발생하는 이벤트</summary>
        public event EventHandler ExplosionEvent;

        /// <summary>화약통 애니메이션 컨트롤러</summary>
        [SerializeField]
        private Animator _animator;
        /// <summary>주변 감지 체크 간격 (초)</summary>
        [SerializeField]
        private float _watchInterval;
        /// <summary>깜빡임 최소 간격 (초)</summary>
        [SerializeField]
        private float _minBlinkInterval;
        /// <summary>깜빡임 최대 간격 (초)</summary>
        [SerializeField]
        private float _maxBlinkInterval;

        /// <summary>현재 감시 모드 활성화 여부</summary>
        private bool _isWatching;
        /// <summary>감시 반경</summary>
        private float _watchRadius;
        /// <summary>마지막 감지 체크 시각</summary>
        private float _lastCheckTime;
        /// <summary>다음 깜빡임 예정 시각</summary>
        private float _nextBlinkTime;

        /// <summary>OverlapSphere 결과를 저장하는 재사용 배열</summary>
        private readonly Collider[] _overlapResults = new Collider[20];

        /// <summary>
        /// 오브젝트 풀 반환 시 이벤트 구독을 초기화한다.
        /// </summary>
        protected override void CleanUp()
        {
            base.CleanUp();

            BatchObjectDetectedEvent = null;
            ExplosionEvent = null;
        }

        /// <summary>
        /// 화약통의 위치를 2D 좌표로 설정한다.
        /// </summary>
        /// <param name="position">설정할 위치 (X, Y를 월드 X, Z로 변환)</param>
        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, 0, position.Y);
        }

        /// <summary>
        /// 화약통의 현재 위치를 2D 좌표로 반환한다.
        /// </summary>
        /// <returns>현재 월드 위치의 2D 좌표 (X, Z를 X, Y로 변환)</returns>
        public System.Numerics.Vector2 GetPosition()
        {
            var pos = transform.position;
            return new System.Numerics.Vector2(pos.x, pos.z);
        }

        /// <summary>
        /// 감시 모드를 시작한다. 지정된 반경 내 오브젝트를 주기적으로 감지한다.
        /// </summary>
        /// <param name="watchRadius">감시 반경</param>
        public void Watch(float watchRadius)
        {
            _isWatching = true;
            _watchRadius = watchRadius;
            _lastCheckTime = 0.0f;

            _animator.Play("watch");
            DetermineNextBlinkTime();
        }

        /// <summary>
        /// 화약통을 점화한다. 감시를 중단하고 점화 애니메이션을 재생한다.
        /// </summary>
        public void Ignite()
        {
            _isWatching = false;

            _animator.Play("ignite");
        }

        /// <summary>
        /// 점화 애니메이션 완료 시 호출되어 폭발 이벤트를 발생시킨다.
        /// </summary>
        public void OnIgniteFinished()
        {
            ExplosionEvent?.Invoke(this, EventArgs.Empty);
        }

        private void FixedUpdate()
        {
            if (!_isWatching)
                return;

            if (UnityEngine.Time.time >= _nextBlinkTime)
            {
                _animator.SetTrigger("blink");
                DetermineNextBlinkTime();
            }

            if (UnityEngine.Time.fixedTime - _lastCheckTime < _watchInterval) return;
            _lastCheckTime = UnityEngine.Time.fixedTime;

            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _watchRadius, _overlapResults);
            if (hitCount > 0)
            {
                var detectedNames = _overlapResults.Take(hitCount).Select(c => c.gameObject.name);
                BatchObjectDetectedEvent?.Invoke(this, new BatchObjectDetectedEventArgs(detectedNames));
                
                Array.Clear(_overlapResults, 0, _overlapResults.Length);
            }
        }

        /// <summary>
        /// 다음 깜빡임 시각을 최소/최대 간격 사이의 랜덤 값으로 결정한다.
        /// </summary>
        private void DetermineNextBlinkTime()
        {
            _nextBlinkTime = UnityEngine.Time.time + UnityEngine.Random.Range(_minBlinkInterval, _maxBlinkInterval);
        }
    }
}
