using System;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// NPC(비플레이어) 유닛의 AI 로직 클래스.
    /// 감지 범위 내 적을 탐색하여 접근 및 기본 스킬로 공격하며,
    /// 카메라 밖으로 나가면 워프 포인트로 재배치된다.
    /// </summary>
    public class NonPlayerUnitLogic : UnitLogic
    {
        /// <summary>워프 타이머</summary>
        private readonly Timer _timer;
        /// <summary>월드 인터페이스</summary>
        private readonly Core.View.BattleFieldScene.IWorld _world;

        /// <summary>현재 주 공격 대상</summary>
        private ITarget _mainTarget;
        /// <summary>현재 유닛 상태</summary>
        private UnitState _state = UnitState.Initial;

        /// <summary>
        /// NonPlayerUnitLogic의 생성자.
        /// </summary>
        /// <param name="loop">게임 루프</param>
        /// <param name="timer">타이머</param>
        /// <param name="world">월드 인터페이스</param>
        public NonPlayerUnitLogic(ILoop loop, Timer timer, View.BattleFieldScene.IWorld world) : base(loop)
        {
            _timer = timer;
            _world = world;
        }

        /// <summary>
        /// NPC 유닛의 상태를 나타내는 열거형.
        /// </summary>
        private enum UnitState
        {
            /// <summary>초기 상태</summary>
            Initial,
            /// <summary>대기 상태</summary>
            Wait,
            /// <summary>스킬 사용 중 상태</summary>
            UsingSkill,
            /// <summary>대상에게 이동 중 상태</summary>
            MoveToTarget
        }

        protected override void OnUpdate()
        {
            switch (_state)
            {
                case UnitState.Initial:
                    Wait();
                    SetWarpTimer();
                    break;
                case UnitState.Wait:
                    HandleWaitState();
                    break;
                case UnitState.MoveToTarget:
                    HandleMoveToTargetState();
                    break;
            }
        }

        private void HandleMoveToTargetState()
        {
            TryUseDefaultSkill();
        }

        private void HandleWaitState()
        {
            if (!IsValidTarget(_mainTarget))
                if (!TrySetMainTarget())
                    return;

            TryUseDefaultSkill();
        }

        private void TryUseDefaultSkill()
        {
            var skill = GetControlTarget().GetDefaultSkill();
            if (skill == null || !skill.IsCooldownFinished())
                return;

            if (!IsValidTarget(_mainTarget))
            {
                Wait();
                return;
            }

            if (!skill.IsTargetInRange(GetControlTarget(), _mainTarget))
            {
                MoveTo(_mainTarget);
                return;
            }

            skill.Use(new UseSkillArgs { Target = _mainTarget, OnCompleted = OnSkillCompleted });
            _state = UnitState.UsingSkill;
        }

        private void OnSkillCompleted()
        {
            TryUseDefaultSkill();
        }

        private void Wait()
        {
            _state = UnitState.Wait;
            GetControlTarget().Wait();
        }

        private void MoveTo(ITarget target)
        {
            _state = UnitState.MoveToTarget;
            GetControlTarget().MoveTo(target.GetPosition());
        }

        private bool IsValidTarget(ITarget target)
        {
            return target != null && !target.IsDead();
        }

        private bool TrySetMainTarget()
        {
            var enemies = GetControlTarget().FindTargets(GetControlTarget().GetPosition(), GetControlTarget().GetAttribute(AttributeID.DetectRange).AsFloat());
            var target = enemies.OrderBy(e => Vector2.Distance(GetControlTarget().GetPosition(), e.GetPosition())).FirstOrDefault();

            if (target == null)
                return false;

            SetMainTarget(target);
            return true;
        }

        private void SetMainTarget(ITarget target)
        {
            UnsetMainTarget();
            _mainTarget = target;
            _mainTarget.DestroyEvent += OnDestroyMainTargetEvent;
        }

        private void UnsetMainTarget()
        {
            if (_mainTarget == null)
                return;

            _mainTarget.DestroyEvent -= OnDestroyMainTargetEvent;
            _mainTarget = null;
        }

        private void OnDestroyMainTargetEvent(object sender, EventArgs e)
        {
            UnsetMainTarget();
        }

        private void SetWarpTimer()
        {
            if (IsDestroyed())
                return;

            _timer.Start(0, 3.0f, OnWarpTimeFinished);
        }

        private void OnWarpTimeFinished()
        {
            if (IsDestroyed())
                return;

            if (_world.IsOutOfCameraView(GetControlTarget().GetPosition()))
            {
                Wait();
                GetControlTarget().SetPosition(_world.GetWarpPoint());
            }
            SetWarpTimer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UnsetMainTarget();
            _timer.Destroy();
        }
    }
}
