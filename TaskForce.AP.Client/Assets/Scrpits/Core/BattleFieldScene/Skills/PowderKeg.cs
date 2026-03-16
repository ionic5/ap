using System;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 화약통 오브젝트를 나타내는 클래스.
    /// 설치 후 감시 범위 내 적 진입 또는 만료 시간 경과 시 점화되어 폭발을 생성한다.
    /// </summary>
    public class PowderKeg
    {
        /// <summary>
        /// 파괴 여부 플래그 (중복 파괴 방지용)
        /// </summary>
        private bool _isDestroyed;

        /// <summary>
        /// 화약통 뷰 객체
        /// </summary>
        private readonly IPowderKeg _powderKeg;

        /// <summary>
        /// 화약통을 설치한 유닛
        /// </summary>
        private readonly IUnit _caster;

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
        /// 적 감지 감시 반경
        /// </summary>
        private readonly float _watchRadius;

        /// <summary>
        /// 자동 폭발까지의 만료 시간
        /// </summary>
        private readonly float _expireTime;

        /// <summary>
        /// 폭발 객체 생성 팩토리 함수
        /// </summary>
        private readonly Func<IUnit, int, int, float, Explosion> _createExplosion;

        /// <summary>
        /// 만료 시간 관리용 타이머
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// PowderKeg의 생성자. 뷰 화약통의 파괴, 적 감지, 폭발 이벤트를 구독한다.
        /// </summary>
        /// <param name="powderKeg">화약통 뷰 객체</param>
        /// <param name="caster">화약통을 설치한 유닛</param>
        /// <param name="expireTimer">만료 타이머</param>
        /// <param name="minDamage">폭발 최소 데미지</param>
        /// <param name="maxDamage">폭발 최대 데미지</param>
        /// <param name="watchRadius">적 감지 감시 반경</param>
        /// <param name="explosionRadius">폭발 반경</param>
        /// <param name="expireTime">자동 폭발 만료 시간</param>
        /// <param name="createExplosion">폭발 객체 생성 함수</param>
        public PowderKeg(IPowderKeg powderKeg, IUnit caster, Timer expireTimer,
            int minDamage, int maxDamage, float watchRadius, float explosionRadius, float expireTime,
            Func<IUnit, int, int, float, Explosion> createExplosion)
        {
            _powderKeg = powderKeg;
            _caster = caster;
            _timer = expireTimer;

            _minDamage = minDamage;
            _maxDamage = maxDamage;
            _watchRadius = watchRadius;
            _explosionRadius = explosionRadius;
            _createExplosion = createExplosion;
            _expireTime = expireTime;

            _powderKeg.DestroyEvent += OnDestroyEvent;
            _powderKeg.BatchObjectDetectedEvent += OnObjectDetectedEvent;
            _powderKeg.ExplosionEvent += OnExplosionEvent;
        }

        private void OnExpireTimeOutEvent(object sender, EventArgs e)
        {
            Ignite();
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        /// <summary>
        /// 지정한 위치에 화약통을 설치하고 감시 및 만료 타이머를 시작한다.
        /// </summary>
        /// <param name="position">설치할 좌표</param>
        public void Plant(Vector2 position)
        {
            _powderKeg.SetPosition(position);
            _powderKeg.Watch(_watchRadius);

            _timer.Start(0, _expireTime, () => { Ignite(); });
        }

        private void OnObjectDetectedEvent(object sender, BatchObjectDetectedEventArgs args)
        {
            var targets = _caster.FindTargets(args.ObjectIDs);
            if (targets.Any())
                Ignite();
        }

        private void Ignite()
        {
            _timer.Stop(0);

            _powderKeg.Ignite();
        }

        private void OnExplosionEvent(object sender, EventArgs eventArgs)
        {
            var explosion = _createExplosion(_caster, _minDamage, _maxDamage, _explosionRadius);
            explosion.Start(_powderKeg.GetPosition());

            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _timer.Destroy();
            _powderKeg.Destroy();
            _powderKeg.DestroyEvent -= OnDestroyEvent;
            _powderKeg.BatchObjectDetectedEvent -= OnObjectDetectedEvent;
            _powderKeg.ExplosionEvent -= OnExplosionEvent;
        }
    }
}
