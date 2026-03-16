using System;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 양(Sheep) 미사일 투사체를 나타내는 클래스.
    /// 발사 후 이동하면서 충돌한 대상에게 데미지를 가하고, 목적지 도달 또는 파괴 시 소멸한다.
    /// </summary>
    public class SheepMissile
    {
        /// <summary>
        /// 데미지 계산에 사용되는 난수 생성기
        /// </summary>
        private readonly Core.Random _random;

        /// <summary>
        /// 양 미사일의 뷰 미사일 객체
        /// </summary>
        private readonly IMissile _sheep;

        /// <summary>
        /// 미사일 최소 데미지
        /// </summary>
        private readonly int _minDamage;

        /// <summary>
        /// 미사일 최대 데미지
        /// </summary>
        private readonly int _maxDamage;

        /// <summary>
        /// 뷰 ID로 대상을 검색하기 위한 타겟 파인더
        /// </summary>
        private readonly ITargetFinder _targetFinder;

        /// <summary>
        /// 파괴 여부 플래그 (중복 파괴 방지용)
        /// </summary>
        private bool _isDestroyed;

        /// <summary>
        /// SheepMissile의 생성자. 뷰 미사일의 도착, 파괴, 피격 이벤트를 구독한다.
        /// </summary>
        /// <param name="random">난수 생성기</param>
        /// <param name="sheep">뷰 미사일 객체</param>
        /// <param name="minDamage">최소 데미지</param>
        /// <param name="maxDamage">최대 데미지</param>
        /// <param name="targetFinder">뷰 ID 기반 타겟 검색기</param>
        public SheepMissile(Random random, IMissile sheep, int minDamage, int maxDamage, ITargetFinder targetFinder)
        {
            _isDestroyed = false;
            _random = random;
            _sheep = sheep;
            _sheep.ArrivedDestinationEvent += OnArrivedDestinationEvent;
            _sheep.DestroyEvent += OnDestroyEvent;
            _sheep.HitEvent += OnHitEvent;
            _minDamage = minDamage;
            _maxDamage = maxDamage;
            _targetFinder = targetFinder;
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        private void OnArrivedDestinationEvent(object sender, EventArgs e)
        {
            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _sheep.Destroy();
            _sheep.ArrivedDestinationEvent -= OnArrivedDestinationEvent;
            _sheep.DestroyEvent -= OnDestroyEvent;
        }

        /// <summary>
        /// 양 미사일을 지정한 목적지로 이동시킨다.
        /// </summary>
        /// <param name="destination">이동 목적지 좌표</param>
        /// <param name="speed">이동 속도</param>
        public void MoveTo(Vector2 destination, float speed)
        {
            _sheep.MoveTo(destination, speed);
        }

        /// <summary>
        /// 양 미사일의 위치를 설정한다.
        /// </summary>
        /// <param name="position">설정할 좌표</param>
        public void SetPosition(Vector2 position)
        {
            _sheep.SetPosition(position);
        }

        private void OnHitEvent(object sender, View.HitEventArgs args)
        {
            var target = _targetFinder.FindByViewID(args.ObjectID);
            if (target == null)
                return;

            target.Hit(_random.Next(_minDamage, _maxDamage));
        }
    }
}
