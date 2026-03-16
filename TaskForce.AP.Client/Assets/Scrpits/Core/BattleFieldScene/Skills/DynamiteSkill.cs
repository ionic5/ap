using System;
using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 다이너마이트 투척 스킬 클래스.
    /// 연속 투척(버스트) 방식으로 랜덤 위치에 다이너마이트를 던지는 즉시 시전 스킬이다.
    /// </summary>
    public class DynamiteSkill : InstantSkill, ISkill
    {
        /// <summary>
        /// 랜덤 위치 계산에 사용되는 난수 생성기
        /// </summary>
        private readonly Core.Random _random;

        /// <summary>
        /// 연속 투척을 위한 반복 타이머
        /// </summary>
        private readonly RepeatTimer _repeatTimer;

        /// <summary>
        /// 쿨다운 관리용 타이머
        /// </summary>
        private readonly Core.Timer _timer;

        /// <summary>
        /// 다이너마이트 객체 생성 팩토리 함수
        /// </summary>
        private readonly Func<IUnit, int, int, float, Dynamite> _createDynamite;

        /// <summary>
        /// DynamiteSkill의 생성자.
        /// </summary>
        /// <param name="random">난수 생성기</param>
        /// <param name="repeatTimer">연속 투척용 반복 타이머</param>
        /// <param name="timer">쿨다운 타이머</param>
        /// <param name="createDynamite">다이너마이트 생성 팩토리 함수</param>
        /// <param name="skillEntity">스킬 데이터 엔티티</param>
        public DynamiteSkill(Random random, RepeatTimer repeatTimer, Timer timer,
            Func<IUnit, int, int, float, Dynamite> createDynamite, Core.Entity.IActiveSkill skillEntity) : base(skillEntity)
        {
            _random = random;
            _repeatTimer = repeatTimer;
            _timer = timer;
            _createDynamite = createDynamite;
        }

        /// <summary>
        /// 쿨다운이 완료되었는지 확인한다.
        /// </summary>
        /// <returns>쿨다운 타이머가 동작 중이 아니면 true</returns>
        public override bool IsCooldownFinished()
        {
            return !_timer.IsRunning(0);
        }

        /// <summary>
        /// 스킬을 사용한다. 쿨다운을 시작하고 버스트 간격으로 다이너마이트를 연속 투척한다.
        /// </summary>
        /// <param name="args">스킬 사용 인자</param>
        public override void Use(UseSkillArgs args)
        {
            _timer.Start(0, GetAttribute(AttributeID.CooldownTime).AsFloat());

            _repeatTimer.Start(() => ThrowDynamite(GetOwner()),
                GetAttribute(AttributeID.BurstInterval).AsFloat(),
                GetAttribute(AttributeID.BurstCount).AsInt());
        }

        /// <summary>
        /// 다이너마이트 한 개를 생성하여 유닛 위치에서 랜덤 목적지로 투척한다.
        /// </summary>
        /// <param name="user">다이너마이트를 던지는 유닛</param>
        public void ThrowDynamite(IUnit user)
        {
            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MaxDamage).AsInt();
            var explosionRadius = GetAttribute(AttributeID.ExplosionRadius).AsFloat();
            var missile = _createDynamite.Invoke(user, minDmg, maxDmg, explosionRadius);

            var casterPos = user.GetPosition();
            missile.SetPosition(casterPos);

            var minRange = GetAttribute(AttributeID.MinMissileRange).AsFloat();
            var maxRange = GetAttribute(AttributeID.MaxMissileRange).AsFloat();
            var missleSpd = GetAttribute(AttributeID.MissileSpeed).AsFloat();
            var targetPos = _random.NextPosition(casterPos, minRange, maxRange);
            missile.MoveTo(targetPos, missleSpd);
        }

        /// <summary>
        /// 대상이 유닛의 미사일 사거리 내에 있는지 판정한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <param name="target">판정 대상</param>
        /// <returns>최소/최대 사거리 조건을 만족하면 true</returns>
        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            var minRange = GetAttribute(AttributeID.MinMissileRange).AsFloat();
            var maxRange = GetAttribute(AttributeID.MaxMissileRange).AsFloat();
            var distSq = Vector2.DistanceSquared(unit.GetPosition(), target.GetPosition());

            return distSq >= minRange * minRange && distSq >= maxRange * maxRange;
        }

        /// <summary>
        /// 유닛의 미사일 사거리 내에 있는 모든 대상을 반환한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <returns>최소~최대 사거리 범위 내 대상 목록</returns>
        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            var minRange = GetAttribute(AttributeID.MinMissileRange).AsFloat();
            var maxRange = GetAttribute(AttributeID.MaxMissileRange).AsFloat();

            return unit.FindTargets(minRange, maxRange);
        }
    }
}
