using System;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 다이너마이트 투사체를 나타내는 클래스.
    /// 목적지에 도달하면 폭발을 생성하고 스스로 파괴된다.
    /// </summary>
    public class Dynamite
    {
        /// <summary>
        /// 파괴 여부 플래그 (중복 파괴 방지용)
        /// </summary>
        private bool _isDestroyed;

        /// <summary>
        /// 다이너마이트의 뷰 미사일 객체
        /// </summary>
        private readonly IMissile _dynamite;

        /// <summary>
        /// 다이너마이트를 던진 유닛
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
        /// 폭발 객체 생성 팩토리 함수
        /// </summary>
        private readonly Func<IUnit, int, int, float, Explosion> _createExplosion;

        /// <summary>
        /// Dynamite의 생성자. 뷰 미사일에 도착 및 파괴 이벤트를 구독한다.
        /// </summary>
        /// <param name="dynamite">뷰 미사일 객체</param>
        /// <param name="caster">다이너마이트를 던진 유닛</param>
        /// <param name="minDamage">폭발 최소 데미지</param>
        /// <param name="maxDamage">폭발 최대 데미지</param>
        /// <param name="explosionRadius">폭발 반경</param>
        /// <param name="createExplosion">폭발 객체 생성 함수</param>
        public Dynamite(IMissile dynamite, IUnit caster,
            int minDamage, int maxDamage, float explosionRadius,
            Func<IUnit, int, int, float, Explosion> createExplosion)
        {
            _isDestroyed = false;
            _dynamite = dynamite;
            _caster = caster;
            _dynamite.ArrivedDestinationEvent += OnArrivedDestinationEvent;
            _dynamite.DestroyEvent += OnDestroyEvent;

            _minDamage = minDamage;
            _maxDamage = maxDamage;
            _explosionRadius = explosionRadius;
            _createExplosion = createExplosion;
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        private void OnArrivedDestinationEvent(object sender, EventArgs e)
        {
            var explosion = _createExplosion(_caster, _minDamage, _maxDamage, _explosionRadius);
            explosion.Start(_dynamite.GetPosition());

            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _dynamite.Destroy();
            _dynamite.ArrivedDestinationEvent -= OnArrivedDestinationEvent;
            _dynamite.DestroyEvent -= OnDestroyEvent;
        }

        /// <summary>
        /// 다이너마이트를 지정한 목적지로 이동시킨다.
        /// </summary>
        /// <param name="destination">이동 목적지 좌표</param>
        /// <param name="speed">이동 속도</param>
        public void MoveTo(Vector2 destination, float speed)
        {
            _dynamite.MoveTo(destination, speed);
        }

        /// <summary>
        /// 다이너마이트의 위치를 설정한다.
        /// </summary>
        /// <param name="position">설정할 좌표</param>
        public void SetPosition(Vector2 position)
        {
            _dynamite.SetPosition(position);
        }
    }
}
