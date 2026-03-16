using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 폭발 이펙트를 처리하는 풀링 가능한 오브젝트.
    /// 폭발 애니메이션을 재생하고, 폭발 반경 내 오브젝트를 감지하여 이벤트로 알린다.
    /// </summary>
    public class Explosion : PoolableObject, IExplosion
    {
        /// <summary>폭발 애니메이션 컨트롤러</summary>
        [SerializeField]
        private Animator _animator;

        /// <summary>폭발 애니메이션 완료 시 발생하는 이벤트</summary>
        public event EventHandler ExplosionFinishedEvent;
        /// <summary>폭발 반경 내 오브젝트 감지 시 발생하는 이벤트</summary>
        public event EventHandler<BatchObjectHitEventArgs> BatchObjectHitEvent;
        /// <summary>OverlapSphere 결과를 저장하는 재사용 배열</summary>
        private readonly Collider[] _hitResults = new Collider[50];

        /// <summary>
        /// 폭발을 시작한다. 애니메이션을 재생하고 지정된 반경 내 오브젝트를 감지한다.
        /// </summary>
        /// <param name="explosionRadius">폭발 감지 반경</param>
        void IExplosion.Start(float explosionRadius)
        {
            _animator.Play("explosion");

            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                explosionRadius,
                _hitResults
            );

            if (hitCount > 0)
            {
                var hitNames = _hitResults.Take(hitCount).Select(c => c.gameObject.name);
                BatchObjectHitEvent?.Invoke(this, new BatchObjectHitEventArgs(hitNames));

                Array.Clear(_hitResults, 0, _hitResults.Length);
            }
        }

        /// <summary>
        /// 폭발 위치를 2D 좌표로 설정한다.
        /// </summary>
        /// <param name="position">폭발 위치 (X, Y를 월드 X, Z로 변환)</param>
        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, 0.0f, position.Y);
        }

        /// <summary>
        /// 폭발 애니메이션 완료 시 애니메이션 이벤트로부터 호출된다.
        /// </summary>
        public void OnExplosionFinished()
        {
            ExplosionFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 오브젝트 풀 반환 시 이벤트 구독을 초기화한다.
        /// </summary>
        protected override void CleanUp()
        {
            base.CleanUp();

            ExplosionFinishedEvent = null;
            BatchObjectHitEvent = null;
        }
    }
}
