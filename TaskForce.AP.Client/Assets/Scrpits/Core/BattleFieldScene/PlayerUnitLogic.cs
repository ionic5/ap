using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 플레이어 유닛의 로직 클래스.
    /// 조이스틱 입력에 따라 이동하고, 범위 내 적에게 자동으로 기본 스킬을 사용하며,
    /// 근처 소울을 흡수하여 경험치를 획득한다.
    /// </summary>
    public class PlayerUnitLogic : UnitLogic
    {
        /// <summary>조이스틱 입력 인터페이스</summary>
        private readonly IJoystick _joystick;
        /// <summary>소울 검색 인터페이스</summary>
        private readonly ISoulFinder _soulFinder;
        /// <summary>검색된 소울 임시 저장 리스트</summary>
        private readonly List<Soul> _souls;
        /// <summary>게임 데이터 저장소</summary>
        private readonly GameDataStore _gameDataStore;

        /// <summary>현재 유닛 상태</summary>
        private UnitState _state = UnitState.Initial;
        /// <summary>현재 사용 중인 스킬</summary>
        private Skills.ISkill _usingSkill;
        /// <summary>마지막으로 공격한 대상</summary>
        private ITarget _lastTarget;

        /// <summary>
        /// PlayerUnitLogic의 생성자.
        /// </summary>
        /// <param name="loop">게임 루프</param>
        /// <param name="joystick">조이스틱 입력 인터페이스</param>
        /// <param name="soulFinder">소울 검색 인터페이스</param>
        /// <param name="gameDataStore">게임 데이터 저장소</param>
        public PlayerUnitLogic(ILoop loop, IJoystick joystick, ISoulFinder soulFinder,
            GameDataStore gameDataStore) : base(loop)
        {
            _joystick = joystick;
            _soulFinder = soulFinder;
            _gameDataStore = gameDataStore;
            _souls = new List<Soul>();
        }

        /// <summary>
        /// 플레이어 유닛의 상태를 나타내는 열거형.
        /// </summary>
        private enum UnitState
        {
            /// <summary>초기 상태</summary>
            Initial,
            /// <summary>대기 상태</summary>
            Wait,
            /// <summary>이동 상태</summary>
            Move,
            /// <summary>스킬 사용 중 상태</summary>
            UsingSkill
        }

        protected override void OnUpdate()
        {
            UpdateState();
            HandleInput();
            UseInstantSkills();
            TryAbsorbSouls();
        }

        private void UseInstantSkills()
        {
            foreach (var skill in GetControlTarget().GetSkills())
                if (skill.IsInstantSkill() && skill.IsCooldownFinished())
                    skill.Use(new UseSkillArgs());
        }

        private void HandleInput()
        {
            if (_joystick.IsOnControl())
                Move(Vector2.Normalize(_joystick.GetInputVector()));
            else if (_state == UnitState.Move)
                Wait();
        }

        private void UpdateState()
        {
            switch (_state)
            {
                case UnitState.Initial:
                    Wait();
                    break;

                case UnitState.Wait:
                    HandleWaitState();
                    break;
            }
        }

        private void TryUseDefaultSkill(ITarget target)
        {
            if (GetControlTarget().GetDefaultSkill() == null || !GetControlTarget().GetDefaultSkill().IsCooldownFinished())
                return;

            SetState(UnitState.UsingSkill);

            _usingSkill = GetControlTarget().GetDefaultSkill();
            SetLastTarget(target);

            _usingSkill.Use(new UseSkillArgs
            {
                Target = _lastTarget,
                OnCompleted = OnSkillCompleted
            });
        }

        private void OnSkillCompleted()
        {
            if (IsLastTargetExist() && _lastTarget.IsAlive())
                TryUseDefaultSkill(_lastTarget);
            else
                Wait();
        }

        private void HandleWaitState()
        {
            var skill = GetControlTarget().GetDefaultSkill();
            var targets = skill?.GetTargetsInRange(GetControlTarget());

            if (targets != null && targets.Any())
            {
                var target = targets.OrderBy(t => Vector2.DistanceSquared(GetControlTarget().GetPosition(), t.GetPosition())).First();
                TryUseDefaultSkill(target);
            }
        }

        private void TryAbsorbSouls()
        {
            int count = _soulFinder.FindRadius(GetControlTarget().GetPosition(), GetControlTarget().GetAttribute(AttributeID.DetectRange).AsFloat(), _souls);
            if (count <= 0) return;

            foreach (var soul in _souls)
            {
                if (!soul.IsMovingTo(GetControlTarget()))
                    soul.MoveTo(GetControlTarget(), 0.5f);
                else if (Vector2.DistanceSquared(GetControlTarget().GetPosition(), soul.GetPosition()) < GetControlTarget().GetPickUpRange() * GetControlTarget().GetPickUpRange())
                    Absorb(soul);
            }
            _souls.Clear();
        }

        private void Move(Vector2 direction)
        {
            SetState(UnitState.Move);
            GetControlTarget().Move(direction);
        }

        private void Wait()
        {
            SetState(UnitState.Wait);
            GetControlTarget().Wait();
        }

        private void SetState(UnitState state)
        {
            var oldState = _state;
            _state = state;

            if (oldState == UnitState.UsingSkill &&
                _usingSkill != null && !_usingSkill.IsCompleted())
            {
                _usingSkill?.Cancel();
                _usingSkill = null;

                ClearLastTarget();
            }
        }

        private void SetLastTarget(ITarget target)
        {
            ClearLastTarget();

            _lastTarget = target;
            _lastTarget.DestroyEvent += OnDestroyLastTargetEvent;
        }

        private void ClearLastTarget()
        {
            if (_lastTarget == null)
                return;

            _lastTarget.DestroyEvent -= OnDestroyLastTargetEvent;
            _lastTarget = null;
        }

        private void OnDestroyLastTargetEvent(object sender, EventArgs e)
        {
            ClearLastTarget();
        }

        private bool IsLastTargetExist()
        {
            return _lastTarget != null;
        }

        private void Absorb(Soul soul)
        {
            GetControlTarget().AddExp(_gameDataStore.GetSoulExp(soul.GetLevel()));
            soul.Destroy();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearLastTarget();
            _usingSkill = null;
        }
    }
}
