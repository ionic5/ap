using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 양 미사일 발사 스킬 클래스.
    /// 사거리 내 랜덤 대상을 선택하여 발사 각도에 랜덤 편차를 적용한 양 미사일을 연속 발사하는 즉시 시전 스킬이다.
    /// </summary>
    public class SheepMissileSkill : InstantSkill, ISkill
    {
        /// <summary>
        /// 대상 선택 및 발사 각도 편차 계산에 사용되는 난수 생성기
        /// </summary>
        private readonly Core.Random _random;

        /// <summary>
        /// 연속 발사를 위한 반복 타이머
        /// </summary>
        private readonly RepeatTimer _repeatTimer;

        /// <summary>
        /// 쿨다운 관리용 타이머
        /// </summary>
        private readonly Core.Timer _timer;

        /// <summary>
        /// 양 미사일 객체 생성 팩토리 함수
        /// </summary>
        private readonly Func<int, int, SheepMissile> _createSheepMissile;

        /// <summary>
        /// SheepMissileSkill의 생성자.
        /// </summary>
        /// <param name="random">난수 생성기</param>
        /// <param name="repeatTimer">연속 발사용 반복 타이머</param>
        /// <param name="timer">쿨다운 타이머</param>
        /// <param name="createSheepMissile">양 미사일 생성 팩토리 함수</param>
        /// <param name="skillEntity">스킬 데이터 엔티티</param>
        public SheepMissileSkill(Random random, RepeatTimer repeatTimer, Timer timer,
            Func<int, int, SheepMissile> createSheepMissile, Core.Entity.IActiveSkill skillEntity) : base(skillEntity)
        {
            _random = random;
            _repeatTimer = repeatTimer;
            _timer = timer;
            _createSheepMissile = createSheepMissile;
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
        /// 양 미사일 스킬을 사용한다. 사거리 내 랜덤 대상을 선택하여 버스트 간격으로 연속 발사한다.
        /// 사거리 내 대상이 없으면 아무 동작도 하지 않는다.
        /// </summary>
        /// <param name="args">스킬 사용 인자</param>
        public override void Use(UseSkillArgs args)
        {
            var user = GetOwner();

            var maxRange = GetAttribute(AttributeID.MaxMissileRange).AsFloat();
            var targets = user.FindTargets(maxRange);

            if (!targets.Any())
                return;

            var target = targets.ElementAt(_random.Next(0, targets.Count()));
            _timer.Start(0, GetAttribute(AttributeID.CooldownTime).AsFloat());

            _repeatTimer.Start(() => { FireMissile(user, target.GetPosition()); },
                GetAttribute(AttributeID.BurstInterval).AsFloat(),
                GetAttribute(AttributeID.BurstCount).AsInt());
        }

        /// <summary>
        /// 양 미사일 한 발을 생성하여 대상 방향에 랜덤 각도 편차를 적용해 발사한다.
        /// </summary>
        /// <param name="user">미사일을 발사하는 유닛</param>
        /// <param name="targetPos">목표 대상의 위치</param>
        protected void FireMissile(IUnit user, Vector2 targetPos)
        {
            var shooterPos = user.GetPosition();
            var direction = Vector2.Normalize(targetPos - shooterPos);

            var launchAngle = GetAttribute(AttributeID.LaunchAngle).AsFloat();
            float deltaAngleDeg = (float)(_random.NextDouble() * 2.0f * launchAngle - launchAngle);

            float deltaAngleRad = deltaAngleDeg * MathF.PI / 180f;

            float cos = MathF.Cos(deltaAngleRad);
            float sin = MathF.Sin(deltaAngleRad);
            var rotatedDir = new Vector2(
                direction.X * cos - direction.Y * sin,
                direction.X * sin + direction.Y * cos
            );

            var range = GetAttribute(AttributeID.MissileRange).AsFloat();
            var destination = shooterPos + rotatedDir * range;

            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var missile = _createSheepMissile.Invoke(minDmg, maxDmg);
            missile.SetPosition(shooterPos);

            var missleSpd = GetAttribute(AttributeID.MissileSpeed).AsFloat();
            missile.MoveTo(destination, missleSpd);
        }

        /// <summary>
        /// 대상이 유닛의 최대 미사일 사거리 내에 있는지 판정한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <param name="target">판정 대상</param>
        /// <returns>대상이 최대 사거리 내에 있으면 true</returns>
        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            var range = GetAttribute(AttributeID.MaxMissileRange).AsFloat();
            var distSq = Vector2.DistanceSquared(unit.GetPosition(), target.GetPosition());
            return distSq <= range * range;
        }

        /// <summary>
        /// 유닛의 최대 미사일 사거리 내에 있는 모든 대상을 반환한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <returns>최대 사거리 내 대상 목록</returns>
        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            return unit.FindTargets(GetAttribute(AttributeID.MaxMissileRange).AsFloat());
        }
    }
}
