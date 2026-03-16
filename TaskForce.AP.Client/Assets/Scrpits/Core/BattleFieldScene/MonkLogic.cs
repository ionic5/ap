using System.Linq;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 승려(Monk) 유닛의 AI 로직 클래스.
    /// 마스터를 따라다니며 범위 내 아군 중 HP가 가장 낮은 대상을 회복시킨다.
    /// </summary>
    public class MonkLogic : UnitLogic
    {
        /// <summary>난수 생성기</summary>
        private readonly Random _random;
        /// <summary>현재 유닛 상태</summary>
        private UnitState _state;

        /// <summary>
        /// MonkLogic의 생성자.
        /// </summary>
        /// <param name="loop">게임 루프</param>
        /// <param name="random">난수 생성기</param>
        public MonkLogic(ILoop loop, Random random) : base(loop)
        {
            _random = random;
            _state = UnitState.Initial;
        }

        /// <summary>
        /// 승려 유닛의 상태를 나타내는 열거형.
        /// </summary>
        private enum UnitState
        {
            /// <summary>초기 상태</summary>
            Initial,
            /// <summary>대기 상태</summary>
            Wait,
            /// <summary>마스터 추적 상태</summary>
            Follow,
            /// <summary>회복 시전 중 상태</summary>
            Healing
        }

        protected override void OnUpdate()
        {
            switch (_state)
            {
                case UnitState.Initial:
                    Wait();
                    break;

                case UnitState.Wait:
                    HandleWaitState();
                    break;

                case UnitState.Follow:
                    HandleFollowState();
                    break;

                case UnitState.Healing:
                    HandleHealingState();
                    break;
            }
        }


        private void HandleWaitState()
        {
            if (!IsMasterNearby())
                Follow();
            else
                TryHeal();
        }

        private void HandleFollowState()
        {
            if (TryHeal())
                return;

            if (IsMasterNearby())
                Wait();
            else
                Follow();
        }

        private void HandleHealingState()
        {
            if (GetControlTarget().GetDefaultSkill().IsCompleted())
                Wait();
        }

        private void Wait()
        {
            _state = UnitState.Wait;
            GetControlTarget().Wait();
        }

        private void Follow()
        {
            _state = UnitState.Follow;

            var master = GetControlTarget().GetMaster();
            var dest = _random.NextPosition(master.GetPosition(), GetControlTarget().GetFollowRange());
            GetControlTarget().MoveTo(dest);
        }

        private bool TryHeal()
        {
            var skill = GetControlTarget().GetDefaultSkill();
            if (!skill.HasHealEffect() || !skill.IsCooldownFinished())
                return false;

            var target = skill.GetTargetsInRange(GetControlTarget())
                .Where(t => !t.IsFullHP())
                .OrderBy(t => t.GetRemainHP())
                .FirstOrDefault();

            if (target == null) return false;

            skill.Use(new UseSkillArgs { Target = target });

            if (!skill.IsInstantSkill())
                _state = UnitState.Healing;

            return true;
        }

        private bool IsMasterNearby()
        {
            var master = GetControlTarget().GetMaster();
            float range = GetControlTarget().GetFollowRange();
            return System.Numerics.Vector2.DistanceSquared(GetControlTarget().GetPosition(), master.GetPosition()) <= range * range;
        }
    }
}
