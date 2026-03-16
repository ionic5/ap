using System;
using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 전투 필드의 유닛 구현 클래스.
    /// 이동, 공격, 피격, 사망, 스킬 사용, 대상 검색 등 유닛의 모든 핵심 동작을 구현한다.
    /// ITarget, IUnit, IAttacker, IControllableUnit 등 다수의 인터페이스를 구현한다.
    /// </summary>
    public class Unit : ITarget, IUnit, IAttacker, IDestroyable, IFollowable, IMortal,
        IPositionable, IControllableUnit
    {
        /// <summary>경험치 변경 시 발생하는 이벤트</summary>
        public event EventHandler ExpChangedEvent;
        /// <summary>필요 경험치 변경 시 발생하는 이벤트</summary>
        public event EventHandler RequireExpChangedEvent;
        /// <summary>레벨업 시 발생하는 이벤트</summary>
        public event EventHandler LevelUpEvent;
        /// <summary>유닛 파괴 시 발생하는 이벤트</summary>
        public event EventHandler<DestroyEventArgs> DestroyEvent;
        /// <summary>유닛 사망 시 발생하는 이벤트</summary>
        public event EventHandler<DiedEventArgs> DiedEvent;

        /// <summary>유닛 뷰 객체</summary>
        private readonly Core.View.BattleFieldScene.IUnit _unitView;
        /// <summary>유닛 엔티티 (데이터 모델)</summary>
        private readonly Entity.Unit _unitEntity;
        /// <summary>적/아군 판별기</summary>
        private readonly ITargetIdentifier _targetIdentifier;
        /// <summary>로거</summary>
        private readonly Core.ILogger _logger;
        /// <summary>보유 스킬 딕셔너리 (스킬 ID -> 스킬)</summary>
        private readonly Dictionary<string, Skills.ISkill> _skills;
        /// <summary>대상 검색기</summary>
        private readonly ITargetFinder _targetFinder;
        /// <summary>스킬 생성 팩토리 함수</summary>
        private readonly Func<Entity.IActiveSkill, Skills.ISkill> _createSkill;
        /// <summary>유닛 로직 생성 팩토리 함수</summary>
        private readonly Func<IControllableUnit, string, IUnitLogic> _createUnitLogic;

        /// <summary>파괴 여부 플래그</summary>
        private bool _isDestroyed;
        /// <summary>마스터(소환자) 유닛</summary>
        private IUnit _master;
        /// <summary>현재 유닛 로직</summary>
        private IUnitLogic _unitLogic;

        /// <summary>
        /// Unit의 생성자.
        /// </summary>
        /// <param name="unitView">유닛 뷰 객체</param>
        /// <param name="unitEntity">유닛 엔티티</param>
        /// <param name="targetFinder">대상 검색기</param>
        /// <param name="targetIdentifier">적/아군 판별기</param>
        /// <param name="logger">로거</param>
        /// <param name="createSkill">스킬 생성 팩토리 함수</param>
        /// <param name="createUnitLogic">유닛 로직 생성 팩토리 함수</param>
        public Unit(Core.View.BattleFieldScene.IUnit unitView, Entity.Unit unitEntity,
            ITargetFinder targetFinder, ITargetIdentifier targetIdentifier,
            ILogger logger, Func<Core.Entity.IActiveSkill, Skills.ISkill> createSkill,
            Func<IControllableUnit, string, IUnitLogic> createUnitLogic)
        {
            _unitView = unitView;
            _unitEntity = unitEntity;
            _isDestroyed = false;
            _skills = new Dictionary<string, Skills.ISkill>();
            _targetFinder = targetFinder;

            _unitEntity.ExpChangedEvent += OnExpChangedEvent;
            _unitEntity.RequireExpChangedEvent += OnRequireExpChangedEvent;
            _unitEntity.LevelUpEvent += OnLevelUpEvent;
            _unitEntity.SkillAddedEvent += OnSkillAddedEvent;

            _unitView.MoveDirectionChangedEvent += OnMoveDirectionChangedEvent;
            _unitView.DieAnimationFinishedEvent += OnDieAnimationFinishedEvent;
            _unitView.DestroyEvent += OnDestroyEvent;

            _targetIdentifier = targetIdentifier;
            _logger = logger;
            _createSkill = createSkill;
            _createUnitLogic = createUnitLogic;
        }

        /// <summary>
        /// 유닛이 사망 상태인지 확인한다.
        /// </summary>
        /// <returns>사망했으면 true</returns>
        public bool IsDead()
        {
            return _unitEntity.IsDead();
        }

        /// <summary>
        /// 유닛을 대기 상태로 전환한다. 이동을 멈추고 대기 모션을 재생한다.
        /// </summary>
        public void Wait()
        {
            if (IsDead())
                return;

            _unitView.Stop();
            _unitView.PlayMotion(UnitMotionID.Stand);
        }

        /// <summary>
        /// 지정된 방향으로 유닛을 이동시킨다. 이동 속도 속성을 적용한다.
        /// </summary>
        /// <param name="direction">이동 방향 벡터</param>
        public void Move(Vector2 direction)
        {
            if (IsDead())
                return;

            _unitView.PlayMotion(UnitMotionID.Move);
            _unitView.Move(direction * _unitEntity.GetAttribute(AttributeID.MoveSpeed).AsFloat());
            _unitView.SetDirection(_unitView.GetMoveDirection());
        }

        /// <summary>
        /// 지정된 목적지로 유닛을 이동시킨다. 이동 속도 속성을 적용한다.
        /// </summary>
        /// <param name="destination">목적지 좌표</param>
        public void MoveTo(Vector2 destination)
        {
            if (IsDead())
                return;

            _unitView.PlayMotion(UnitMotionID.Move);
            _unitView.MoveTo(destination, _unitEntity.GetAttribute(AttributeID.MoveSpeed).AsFloat());
            _unitView.SetDirection(_unitView.GetMoveDirection());
        }

        private void OnMoveDirectionChangedEvent(object sender, EventArgs args)
        {
            _unitView.SetDirection(_unitView.GetMoveDirection());
        }

        private void OnDieAnimationFinishedEvent(object sender, EventArgs e)
        {
            Destroy();
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        /// <summary>
        /// 유닛을 사망 처리한다. 사망 상태로 전환하고 사망 모션을 재생한다.
        /// </summary>
        public void Die()
        {
            if (IsDead())
                return;

            _unitEntity.SetDead();

            OnDie();
        }

        private void OnDie()
        {
            _unitView.Stop();
            _unitView.PlayMotion(UnitMotionID.Die);

            DiedEvent?.Invoke(this, new DiedEventArgs(this));
        }

        /// <summary>
        /// 유닛에게 데미지를 적용한다. HP가 0 이하가 되면 사망 처리한다.
        /// </summary>
        /// <param name="damage">적용할 데미지 양</param>
        public void Hit(int damage)
        {
            if (IsDead())
                return;

            _unitEntity.ApplyDamage(damage);

            _unitView.PlayDamageAnimation(damage);

            if (IsDead())
                OnDie();
        }

        /// <summary>
        /// 유닛이 바라보는 방향 벡터를 가져온다.
        /// </summary>
        /// <returns>방향 벡터</returns>
        public Vector2 GetDirection()
        {
            return _unitView.GetDirection();
        }

        /// <summary>
        /// 유닛의 현재 위치를 가져온다.
        /// </summary>
        /// <returns>현재 위치 벡터</returns>
        public Vector2 GetPosition()
        {
            return _unitView.GetPosition();
        }

        /// <summary>
        /// 유닛이 생존 상태인지 확인한다.
        /// </summary>
        /// <returns>생존 중이면 true</returns>
        public bool IsAlive()
        {
            return _unitEntity.IsAlive();
        }

        /// <summary>
        /// 대상이 공격 사거리 내에 있는지 판정한다.
        /// </summary>
        /// <param name="target">판정할 대상</param>
        /// <returns>사거리 내이면 true</returns>
        public bool IsTargetInAttackRange(ITarget target)
        {
            Vector2 pos = _unitView.GetPosition();
            Vector2 targetPos = target.GetPosition();

            return Vector2.Distance(pos, targetPos) <= _unitEntity.GetAttribute(AttributeID.AttackRange).AsFloat();
        }

        /// <summary>
        /// 유닛을 파괴한다. 모든 이벤트 구독을 해제하고 뷰와 로직을 정리한다.
        /// </summary>
        public void Destroy()
        {
            if (_isDestroyed)
                return;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            _isDestroyed = true;

            _unitEntity.ExpChangedEvent -= OnExpChangedEvent;
            _unitEntity.RequireExpChangedEvent -= OnRequireExpChangedEvent;
            _unitEntity.LevelUpEvent -= OnLevelUpEvent;
            _unitEntity.SkillAddedEvent -= OnSkillAddedEvent;

            _unitView.MoveDirectionChangedEvent -= OnMoveDirectionChangedEvent;
            _unitView.DieAnimationFinishedEvent -= OnDieAnimationFinishedEvent;
            _unitView.DestroyEvent -= OnDestroyEvent;

            _unitView.Destroy();

            DestroyUnitLogic();
        }

        /// <summary>
        /// 유닛의 위치를 설정한다.
        /// </summary>
        /// <param name="position">설정할 위치</param>
        public void SetPosition(Vector2 position)
        {
            _unitView.SetPosition(position);
        }

        /// <summary>
        /// 플레이어 진영에 속하는지 확인한다.
        /// </summary>
        /// <returns>플레이어 진영이면 true</returns>
        public bool IsPlayerSide()
        {
            return _unitEntity.IsPlayerSide();
        }

        /// <summary>
        /// 지정된 ID의 속성 값을 가져온다.
        /// </summary>
        /// <param name="id">속성 ID</param>
        /// <returns>속성 값</returns>
        public Attribute GetAttribute(string id)
        {
            return _unitEntity.GetAttribute(id);
        }

        /// <summary>
        /// 대상이 적인지 판별한다. 자기 자신은 적으로 판별하지 않는다.
        /// </summary>
        /// <param name="target">판별할 대상</param>
        /// <returns>적이면 true</returns>
        public bool IsEnemy(ITarget target)
        {
            if (target == this)
                return false;
            return _targetIdentifier.IsEnemy(target);
        }

        /// <summary>
        /// 아이템 줍기 범위를 가져온다.
        /// </summary>
        /// <returns>줍기 범위</returns>
        public float GetPickUpRange()
        {
            return 0.8f;
        }

        /// <summary>
        /// 유닛에 경험치를 추가한다.
        /// </summary>
        /// <param name="exp">추가할 경험치 양</param>
        public void AddExp(int exp)
        {
            _unitEntity.AddExp(exp);
        }

        private void OnExpChangedEvent(object sender, EventArgs args)
        {
            ExpChangedEvent?.Invoke(this, args);
        }

        private void OnRequireExpChangedEvent(object sender, EventArgs args)
        {
            RequireExpChangedEvent?.Invoke(this, args);
        }

        private void OnLevelUpEvent(object sender, EventArgs args)
        {
            LevelUpEvent?.Invoke(this, args);
        }

        private void OnSkillAddedEvent(object sender, SkillAddedEventArgs args)
        {
            AddSkill(_unitEntity.GetSkill(args.SkillID));
        }

        /// <summary>
        /// 유닛에 스킬을 추가한다. 스킬 엔티티를 기반으로 스킬 객체를 생성하여 등록한다.
        /// </summary>
        /// <param name="skillEntity">추가할 스킬 엔티티</param>
        public void AddSkill(Entity.IActiveSkill skillEntity)
        {
            var skill = _createSkill(skillEntity);
            if (skill == null)
            {
                _logger.Warn($"Failed to add skill : {skillEntity.GetSkillID()}");
                return;
            }
            skill.SetOwner(this);

            _skills.Add(skill.GetSkillID(), skill);
        }

        /// <summary>
        /// 레벨업에 필요한 경험치를 가져온다.
        /// </summary>
        /// <returns>필요 경험치</returns>
        public int GetRequireExp()
        {
            return _unitEntity.GetRequireExp();
        }

        /// <summary>
        /// 현재 경험치를 가져온다.
        /// </summary>
        /// <returns>현재 경험치</returns>
        public int GetExp()
        {
            return _unitEntity.GetExp();
        }

        /// <summary>
        /// 현재 레벨을 가져온다.
        /// </summary>
        /// <returns>현재 레벨</returns>
        public int GetLevel()
        {
            return _unitEntity.GetLevel();
        }

        /// <summary>
        /// 지정된 ID의 액티브 스킬 엔티티를 가져온다.
        /// </summary>
        /// <param name="id">스킬 ID</param>
        /// <returns>해당 스킬 엔티티</returns>
        public Core.Entity.IActiveSkill GetSkill(string id)
        {
            return _unitEntity.GetSkill(id);
        }

        /// <summary>
        /// 유닛의 뷰 객체 ID를 가져온다.
        /// </summary>
        /// <returns>뷰 객체 ID</returns>
        public string GetViewID()
        {
            return _unitView.GetObjectID();
        }

        /// <summary>
        /// 보유 중인 모든 스킬을 가져온다.
        /// </summary>
        /// <returns>스킬 목록</returns>
        public IEnumerable<Skills.ISkill> GetSkills()
        {
            return _skills.Values;
        }

        /// <summary>
        /// 최소~최대 범위 내 적을 검색한다.
        /// </summary>
        /// <param name="minRange">최소 범위</param>
        /// <param name="maxRange">최대 범위</param>
        /// <returns>범위 내 적 목록</returns>
        public IEnumerable<ITarget> FindTargets(float minRange, float maxRange)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 유닛 위치 기준으로 지정 범위 내 적을 검색한다.
        /// </summary>
        /// <param name="range">검색 범위</param>
        /// <returns>범위 내 적 목록</returns>
        public IEnumerable<ITarget> FindTargets(float range)
        {
            return _targetFinder.FindInRadius(GetPosition(), range, entry => _targetIdentifier.IsEnemy(entry));
        }

        /// <summary>
        /// 지정된 중심점과 범위 내 적을 검색한다.
        /// </summary>
        /// <param name="center">검색 중심 좌표</param>
        /// <param name="range">검색 범위</param>
        /// <returns>범위 내 적 목록</returns>
        public IEnumerable<ITarget> FindTargets(Vector2 center, float range)
        {
            return _targetFinder.FindInRadius(center, range, entry => _targetIdentifier.IsEnemy(entry));
        }

        /// <summary>
        /// 뷰 ID 목록에 해당하는 적을 검색한다.
        /// </summary>
        /// <param name="objectIDs">뷰 ID 목록</param>
        /// <returns>해당하는 적 목록</returns>
        public IEnumerable<ITarget> FindTargets(IEnumerable<string> objectIDs)
        {
            return _targetFinder.FindByViewIDs(objectIDs, entry => _targetIdentifier.IsEnemy(entry));
        }

        /// <summary>
        /// 마스터(소환자) 유닛을 가져온다.
        /// </summary>
        /// <returns>마스터 유닛</returns>
        public IUnit GetMaster()
        {
            return _master;
        }

        /// <summary>
        /// 마스터 추적 범위를 가져온다.
        /// </summary>
        /// <returns>추적 범위</returns>
        public float GetFollowRange()
        {
            return 3.0f;
        }

        /// <summary>
        /// 마스터(소환자) 유닛을 설정한다. 마스터의 파괴/사망 이벤트를 구독한다.
        /// </summary>
        /// <param name="unit">마스터 유닛</param>
        public void SetMaster(IUnit unit)
        {
            _master = unit;
            _master.DestroyEvent += OnMasterDestroyEvent;
            _master.DiedEvent += OnMasterDiedEvent;
        }

        /// <summary>
        /// 마스터 유닛 설정을 해제한다. 이벤트 구독을 해제한다.
        /// </summary>
        public void UnsetMaster()
        {
            if (_master == null)
                return;
            _master.DestroyEvent -= OnMasterDestroyEvent;
            _master.DiedEvent -= OnMasterDiedEvent;
            _master = null;
        }

        private void OnMasterDestroyEvent(object sender, DestroyEventArgs args)
        {
            UnsetMaster();
        }

        private void OnMasterDiedEvent(object sender, DiedEventArgs args)
        {
            Die();
        }

        /// <summary>
        /// 남은 HP를 가져온다.
        /// </summary>
        /// <returns>남은 HP</returns>
        public int GetRemainHP()
        {
            return _unitEntity.GetHP();
        }

        /// <summary>
        /// HP가 최대인지 확인한다.
        /// </summary>
        /// <returns>최대 HP이면 true</returns>
        public bool IsFullHP()
        {
            return _unitEntity.IsFullHP();
        }

        /// <summary>
        /// 지정된 범위 내 아군을 검색한다.
        /// </summary>
        /// <param name="range">검색 범위</param>
        /// <returns>범위 내 아군 목록</returns>
        public IEnumerable<ITarget> FindAllies(float range)
        {
            return _targetFinder.FindInRadius(GetPosition(), range, entry => _targetIdentifier.IsAlly(entry));
        }

        /// <summary>
        /// 유닛을 회복한다. 회복 애니메이션을 재생한다.
        /// </summary>
        /// <param name="healAmount">회복량</param>
        public void Heal(int healAmount)
        {
            _unitEntity.ApplyHeal(healAmount);

            _unitView.PlayHealAnimation(healAmount);
        }

        /// <summary>
        /// 부채꼴 범위 내 적을 검색한다.
        /// </summary>
        /// <param name="center">검색 중심 좌표</param>
        /// <param name="direction">부채꼴 중심 방향</param>
        /// <param name="angle">부채꼴 각도</param>
        /// <param name="range">부채꼴 반경</param>
        /// <returns>범위 내 적 목록</returns>
        public IEnumerable<ITarget> FindTargetsInSector(Vector2 center, Vector2 direction, float angle, float range)
        {
            return _targetFinder.FindInSector(center, direction, angle, range, entry => _targetIdentifier.IsEnemy(entry));
        }

        /// <summary>
        /// 기본 스킬을 가져온다. 기본 스킬이 설정되지 않았거나 해당 스킬이 없으면 null을 반환한다.
        /// </summary>
        /// <returns>기본 스킬. 없으면 null</returns>
        public Skills.ISkill GetDefaultSkill()
        {
            var defaultSkillID = _unitEntity.GetDefaultSkillID();
            if (!string.IsNullOrEmpty(defaultSkillID) &&
                _skills.TryGetValue(defaultSkillID, out var skill))
                return skill;
            return null;
        }

        /// <summary>
        /// 시전 모션을 재생한다. 이동을 멈추고 시전 애니메이션을 시작한다.
        /// </summary>
        /// <param name="castTime">시전 시간</param>
        public void Cast(float castTime)
        {
            _unitView.Stop();
            _unitView.PlayMotion(UnitMotionID.Cast);
        }

        /// <summary>
        /// 공격 모션을 재생한다. 이동을 멈추고 지정 방향으로 공격 애니메이션을 시작한다.
        /// </summary>
        /// <param name="direction">공격 방향</param>
        /// <param name="attackTime">공격 시간</param>
        public void Attack(Vector2 direction, float attackTime)
        {
            _unitView.Stop();
            _unitView.PlayMotion(UnitMotionID.Attack, direction, attackTime, true);
        }

        /// <summary>
        /// 지정된 ID의 유닛 로직을 설정한다. 기존 로직이 있으면 파괴 후 교체한다.
        /// </summary>
        /// <param name="logicID">유닛 로직 ID</param>
        public void SetUnitLogic(string logicID)
        {
            DestroyUnitLogic();

            _unitLogic = _createUnitLogic.Invoke(this, logicID);
            _unitLogic.StartControl(this);
        }

        private void DestroyUnitLogic()
        {
            if (_unitLogic == null)
                return;

            _unitLogic?.Destroy();
            _unitLogic = null;
        }
    }
}
