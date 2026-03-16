using System;
using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 화약통 설치 스킬 클래스.
    /// 사용 시 유닛 위치에 화약통을 생성하여 설치하는 즉시 시전 스킬이다.
    /// </summary>
    public class PowderKegSkill : InstantSkill, ISkill
    {
        /// <summary>
        /// 쿨다운 관리용 타이머
        /// </summary>
        private readonly Core.Timer _timer;

        /// <summary>
        /// 화약통 객체 생성 팩토리 함수
        /// </summary>
        private readonly Func<IUnit, int, int, float, float, float, PowderKeg> _createPowderKeg;

        /// <summary>
        /// PowderKegSkill의 생성자.
        /// </summary>
        /// <param name="skillEntity">스킬 데이터 엔티티</param>
        /// <param name="timer">쿨다운 타이머</param>
        /// <param name="createPowderKeg">화약통 생성 팩토리 함수</param>
        public PowderKegSkill(Core.Entity.IActiveSkill skillEntity,
            Timer timer, Func<IUnit, int, int, float, float, float, PowderKeg> createPowderKeg) : base(skillEntity)
        {
            _timer = timer;
            _createPowderKeg = createPowderKeg;
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
        /// 화약통 설치 스킬을 사용한다. 쿨다운을 시작하고 유닛 위치에 화약통을 생성하여 설치한다.
        /// </summary>
        /// <param name="args">스킬 사용 인자</param>
        public override void Use(UseSkillArgs args)
        {
            var user = GetOwner();

            _timer.Start(0, GetAttribute(AttributeID.CooldownTime).AsFloat());

            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MaxDamage).AsInt();
            var watchRadius = GetAttribute(AttributeID.WatchRadius).AsFloat();
            var explosionRadius = GetAttribute(AttributeID.ExplosionRadius).AsFloat();
            var expireTime = GetAttribute(AttributeID.ExpireTime).AsFloat();

            var keg = _createPowderKeg(user, minDmg, maxDmg, watchRadius, explosionRadius, expireTime);
            keg.Plant(user.GetPosition());
        }

        /// <summary>
        /// 대상이 유닛의 폭발 반경 내에 있는지 판정한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <param name="target">판정 대상</param>
        /// <returns>대상이 폭발 반경 내에 있으면 true</returns>
        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            var range = GetAttribute(AttributeID.ExplosionRadius).AsFloat();
            var distSq = Vector2.DistanceSquared(unit.GetPosition(), target.GetPosition());

            return distSq <= range * range;
        }

        /// <summary>
        /// 유닛의 폭발 반경 내에 있는 모든 대상을 반환한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <returns>폭발 반경 내 대상 목록</returns>
        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            return unit.FindTargets(GetAttribute(AttributeID.ExplosionRadius).AsFloat());
        }
    }
}
