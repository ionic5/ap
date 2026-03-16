using System;
using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 아군 대상 치유 스킬 클래스.
    /// 시전 시간이 있으며, 시전 완료 후 대상에게 힐 이펙트와 회복량을 적용한다.
    /// </summary>
    public class HealSkill : ActiveSkill, ISkill
    {
        /// <summary>
        /// 쿨다운, 적용 딜레이, 시전 관리에 사용되는 타이머
        /// </summary>
        private readonly Core.Timer _timer;

        /// <summary>
        /// 힐 이펙트 생성 팩토리 함수
        /// </summary>
        private readonly Func<IOneShotEffect> _createHealEffect;

        /// <summary>
        /// 타이머 타입 상수를 정의하는 구조체
        /// </summary>
        private struct TimerType
        {
            /// <summary>
            /// 쿨다운 타이머 ID
            /// </summary>
            public const int Cooldown = 0;

            /// <summary>
            /// 힐 적용 딜레이 타이머 ID
            /// </summary>
            public const int ApplyDelay = 1;

            /// <summary>
            /// 시전 타이머 ID
            /// </summary>
            public const int Casting = 2;
        }

        /// <summary>
        /// HealSkill의 생성자.
        /// </summary>
        /// <param name="timer">다목적 타이머 (쿨다운, 시전, 적용 딜레이)</param>
        /// <param name="createHealEffect">힐 이펙트 생성 팩토리 함수</param>
        /// <param name="skill">스킬 데이터 엔티티</param>
        public HealSkill(Timer timer, Func<IOneShotEffect> createHealEffect, Core.Entity.IActiveSkill skill) : base(skill)
        {
            _timer = timer;
            _createHealEffect = createHealEffect;
        }

        /// <summary>
        /// 쿨다운이 완료되었는지 확인한다.
        /// </summary>
        /// <returns>쿨다운 타이머가 동작 중이 아니면 true</returns>
        public override bool IsCooldownFinished()
        {
            return !_timer.IsRunning(TimerType.Cooldown);
        }

        /// <summary>
        /// 힐 스킬을 사용한다. 시전 시간 후 대상에게 힐 이펙트와 회복량을 적용한다.
        /// 대상이 사망 상태이면 효과가 적용되지 않는다.
        /// </summary>
        /// <param name="args">스킬 사용 인자 (힐 대상 포함)</param>
        public override void Use(UseSkillArgs args)
        {
            var user = GetOwner();
            user.Cast(GetAttribute(AttributeID.CastTime).AsFloat());
            _timer.Start(TimerType.Casting, GetAttribute(AttributeID.CastTime).AsFloat(), () =>
            {
                var target = args.Target;
                if (target.IsDead())
                    return;

                _timer.Start(TimerType.Cooldown, GetAttribute(AttributeID.CooldownTime).AsFloat());

                var effect = _createHealEffect.Invoke();
                effect.SetPosition(target.GetPosition());
                effect.Follow(target);
                effect.Play();

                _timer.Start(TimerType.ApplyDelay, GetAttribute(AttributeID.ApplyDelayTime).AsFloat(), () =>
                {
                    target.Heal(GetAttribute(AttributeID.HealAmount).AsInt());
                });
            });
        }

        /// <summary>
        /// 힐 효과를 가진 스킬이므로 항상 true를 반환한다.
        /// </summary>
        /// <returns>항상 true</returns>
        public override bool HasHealEffect()
        {
            return true;
        }

        /// <summary>
        /// 대상이 유닛의 힐 사거리 내에 있는지 판정한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <param name="mainTarget">사거리 판정 대상</param>
        /// <returns>대상이 사거리 내에 있으면 true</returns>
        public override bool IsTargetInRange(IUnit unit, ITarget mainTarget)
        {
            var range = GetAttribute(AttributeID.Range).AsFloat();
            return Vector2.DistanceSquared(unit.GetPosition(), mainTarget.GetPosition()) < range * range;
        }

        /// <summary>
        /// 시전 사거리 내에 있는 아군 대상 목록을 반환한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <returns>시전 범위 내 아군 목록</returns>
        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            return unit.FindAllies(GetAttribute(AttributeID.CastRange).AsFloat());
        }

        /// <summary>
        /// 시전이 완료되었는지 확인한다.
        /// </summary>
        /// <returns>시전 타이머가 동작 중이 아니면 true</returns>
        public override bool IsCompleted()
        {
            return !_timer.IsRunning(TimerType.Casting);
        }

        /// <summary>
        /// 시전 중인 힐 스킬을 취소한다.
        /// </summary>
        public override void Cancel()
        {
            if (_timer.IsRunning(TimerType.Casting))
                _timer.Stop(TimerType.Casting);
        }
    }
}
