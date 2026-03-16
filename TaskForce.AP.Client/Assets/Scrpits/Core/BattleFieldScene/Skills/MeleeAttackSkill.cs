using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 근접 공격 스킬 클래스.
    /// 대상 방향으로 공격 모션을 수행하고, 타격 타이밍에 범위 내 적에게 데미지를 가한다.
    /// 부채꼴(SwingAngle) 범위 판정을 지원한다.
    /// </summary>
    public class MeleeAttackSkill : ActiveSkill, ISkill
    {
        /// <summary>
        /// 쿨다운 및 타격 타이밍 관리용 타이머
        /// </summary>
        private readonly Core.Timer _timer;

        /// <summary>
        /// 데미지 계산에 사용되는 난수 생성기
        /// </summary>
        private readonly Core.Random _random;

        /// <summary>
        /// 현재 스킬 사용에 대한 인자 (대상, 완료 콜백)
        /// </summary>
        private UseSkillArgs _useSkillArgs;

        /// <summary>
        /// 현재 스킬의 상태
        /// </summary>
        private State _state;

        /// <summary>
        /// 근접 공격 스킬의 상태를 나타내는 열거형
        /// </summary>
        private enum State
        {
            /// <summary>
            /// 초기 상태
            /// </summary>
            Initial,

            /// <summary>
            /// 스킬 사용 중
            /// </summary>
            Using,

            /// <summary>
            /// 스킬 사용 완료
            /// </summary>
            Completed,

            /// <summary>
            /// 스킬 사용 취소됨
            /// </summary>
            Canceled
        }

        /// <summary>
        /// 타이머 타입 상수를 정의하는 클래스
        /// </summary>
        private static class TimerType
        {
            /// <summary>
            /// 쿨다운 타이머 ID
            /// </summary>
            public const int Cooldown = 0;

            /// <summary>
            /// 타격(임팩트) 타이머 ID
            /// </summary>
            public const int Impact = 1;
        }

        /// <summary>
        /// MeleeAttackSkill의 생성자.
        /// </summary>
        /// <param name="timer">쿨다운 및 타격 타이밍 타이머</param>
        /// <param name="skillEntity">스킬 데이터 엔티티</param>
        /// <param name="random">난수 생성기</param>
        public MeleeAttackSkill(Timer timer, Entity.IActiveSkill skillEntity, Random random) : base(skillEntity)
        {
            _timer = timer;
            _random = random;
            _state = State.Initial;
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
        /// 근접 공격 스킬을 사용한다. 대상 방향으로 공격 모션을 시작하고,
        /// 타격 타이밍과 쿨다운 타이머를 설정한다.
        /// </summary>
        /// <param name="args">스킬 사용 인자 (공격 대상 포함)</param>
        public override void Use(UseSkillArgs args)
        {
            _state = State.Using;

            var user = GetOwner();
            var target = args.Target;
            var attackTime = GetAttribute(AttributeID.AttackTime).AsFloat();

            var attackDirection = Vector2.Normalize(target.GetPosition() - user.GetPosition());
            user.Attack(attackDirection, attackTime);
            _timer.Start(TimerType.Cooldown, GetAttribute(AttributeID.AttackTime).AsFloat(), OnCooldownFinished);
            _timer.Start(TimerType.Impact, GetAttribute(AttributeID.AttackImpactTime).AsFloat(), OnAttackImpact);

            SetUseSkillArgs(args);
        }

        private void SetUseSkillArgs(UseSkillArgs args)
        {
            _useSkillArgs = args;

            var user = GetOwner();
            user.DiedEvent += OnUserDiedEvent;
        }

        private void UnsetUseSkillArgs()
        {
            var user = GetOwner();
            user.DiedEvent -= OnUserDiedEvent;

            _useSkillArgs = default;
        }

        private void OnUserDiedEvent(object sender, DiedEventArgs e)
        {
            _state = State.Initial;

            _timer.Stop();

            UnsetUseSkillArgs();
        }

        private void OnCooldownFinished()
        {
            if (_state != State.Using)
                return;
            _state = State.Completed;

            var user = GetOwner();
            var onCompleted = _useSkillArgs.OnCompleted;
            UnsetUseSkillArgs();

            user.Wait();
            onCompleted?.Invoke();
        }

        private void OnAttackImpact()
        {
            if (_state != State.Using)
                return;

            var user = GetOwner();
            var target = _useSkillArgs.Target;

            var targets = new HashSet<ITarget>();

            var attackRange = GetAttribute(AttributeID.AttackRange).AsFloat();
            var degree = GetAttribute(AttributeID.SwingAngle).AsFloat();
            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MaxDamage).AsInt();

            if (target.IsAlive() && IsTargetInRange(user, target))
                targets.Add(target);

            if (degree > 0)
            {
                var position = user.GetPosition();
                var direction = user.GetDirection();
                var enemies = user.FindTargetsInSector(position, direction, degree, attackRange);

                targets.UnionWith(enemies);
            }

            foreach (var entry in targets)
            {
                var damage = _random.Next(minDmg, maxDmg);
                entry.Hit(damage);
            }
        }

        /// <summary>
        /// 대상이 유닛의 공격 사거리 내에 있는지 판정한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <param name="target">판정 대상</param>
        /// <returns>대상이 공격 사거리 내에 있으면 true</returns>
        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            var range = GetAttribute(AttributeID.AttackRange).AsFloat();
            var distSq = Vector2.DistanceSquared(unit.GetPosition(), target.GetPosition());
            return distSq <= range * range;
        }

        /// <summary>
        /// 유닛의 공격 사거리 내에 있는 모든 대상을 반환한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <returns>공격 사거리 내 대상 목록</returns>
        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            return unit.FindTargets(GetAttribute(AttributeID.AttackRange).AsFloat());
        }

        /// <summary>
        /// 스킬 사용이 완료되었는지 확인한다.
        /// </summary>
        /// <returns>상태가 Completed이면 true</returns>
        public override bool IsCompleted()
        {
            return _state == State.Completed;
        }

        /// <summary>
        /// 진행 중인 근접 공격을 취소한다.
        /// 사용 중(Using) 상태일 때만 취소가 가능하다.
        /// </summary>
        public override void Cancel()
        {
            if (_state != State.Using)
                return;
            _state = State.Canceled;

            var user = GetOwner();
            user.Wait();

            UnsetUseSkillArgs();

            if (_timer.IsRunning(TimerType.Cooldown) && _timer.IsRunning(TimerType.Impact))
                _timer.Stop(TimerType.Cooldown);
        }
    }
}
