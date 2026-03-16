using System;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 폭발 효과를 나타내는 클래스.
    /// 지정한 위치에서 폭발을 시작하고, 범위 내 대상에게 랜덤 데미지를 가한 뒤 스스로 파괴된다.
    /// </summary>
    public class Explosion
    {
        /// <summary>
        /// 폭발 뷰 객체
        /// </summary>
        private readonly IExplosion _explosion;

        /// <summary>
        /// 폭발을 발생시킨 유닛
        /// </summary>
        private readonly IUnit _caster;

        /// <summary>
        /// 데미지 계산에 사용되는 난수 생성기
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// 폭발 최소 데미지
        /// </summary>
        private readonly int _minDamage;

        /// <summary>
        /// 폭발 최대 데미지
        /// </summary>
        private readonly int _maxDamage;

        /// <summary>
        /// 폭발 반경
        /// </summary>
        private readonly float _explosionRadius;

        /// <summary>
        /// 파괴 여부 플래그 (중복 파괴 방지용)
        /// </summary>
        private bool _isDestroyed;

        /// <summary>
        /// Explosion의 생성자. 뷰 폭발 객체의 피격, 종료, 파괴 이벤트를 구독한다.
        /// </summary>
        /// <param name="explosion">폭발 뷰 객체</param>
        /// <param name="caster">폭발을 발생시킨 유닛</param>
        /// <param name="random">난수 생성기</param>
        /// <param name="minDamage">최소 데미지</param>
        /// <param name="maxDamage">최대 데미지</param>
        /// <param name="explosionRadius">폭발 반경</param>
        public Explosion(IExplosion explosion, IUnit caster, Random random, int minDamage, int maxDamage, float explosionRadius)
        {
            _explosion = explosion;
            _caster = caster;
            _random = random;
            _minDamage = minDamage;
            _maxDamage = maxDamage;
            _explosionRadius = explosionRadius;

            _explosion.BatchObjectHitEvent += OnBatchObjectHitEvent;
            _explosion.ExplosionFinishedEvent += OnExplosionFinishedEvent;
            _explosion.DestroyEvent += OnDestroyEvent;
        }

        /// <summary>
        /// 지정한 위치에서 폭발을 시작한다.
        /// </summary>
        /// <param name="position">폭발이 발생할 좌표</param>
        public void Start(Vector2 position)
        {
            _explosion.SetPosition(position);
            _explosion.Start(_explosionRadius);
        }

        private void OnBatchObjectHitEvent(object sender, BatchObjectHitEventArgs args)
        {
            var targets = _caster.FindTargets(args.ObjectIDs);
            for (int i = targets.Count() - 1; i >= 0; i--)
                targets.ElementAt(i).Hit(_random.Next(_minDamage, _maxDamage));
        }

        private void OnExplosionFinishedEvent(object sender, EventArgs e)
        {
            Destroy();
        }

        private void OnDestroyEvent(object sender, EventArgs args)
        {
            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _explosion.Destroy();
            _explosion.DestroyEvent -= OnDestroyEvent;
            _explosion.BatchObjectHitEvent -= OnBatchObjectHitEvent;
            _explosion.ExplosionFinishedEvent -= OnExplosionFinishedEvent;
        }
    }
}
