using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 유닛 객체를 생성하는 팩토리 클래스.
    /// 게임 데이터를 기반으로 플레이어 유닛, 아군 NPC 유닛, 적 유닛을 생성한다.
    /// </summary>
    public class UnitFactory
    {
        /// <summary>유닛 생성 시 발생하는 이벤트</summary>
        public event EventHandler<CreatedEventArgs<Unit>> UnitCreatedEvent;

        /// <summary>난수 생성기</summary>
        private readonly Core.Random _random;
        /// <summary>타이머 생성 팩토리 함수</summary>
        private readonly Func<Core.Timer> _createTimer;
        /// <summary>대상 검색기</summary>
        private readonly ITargetFinder _targetFinder;
        /// <summary>유닛 뷰 생성 팩토리 함수</summary>
        private readonly Func<string, View.BattleFieldScene.IUnit> _createUnitView;
        /// <summary>로거</summary>
        private readonly Core.ILogger _logger;
        /// <summary>게임 데이터 저장소</summary>
        private readonly GameDataStore _gameDataStore;
        /// <summary>스킬 생성 팩토리 함수</summary>
        private readonly Func<Entity.IActiveSkill, Skills.ISkill> _createSkill;
        /// <summary>스킬 엔티티 생성 팩토리 함수</summary>
        private readonly Func<string, ISkill> _createSkillEntity;
        /// <summary>유닛 로직 생성 팩토리 함수</summary>
        private readonly Func<IControllableUnit, string, IUnitLogic> _createUnitLogic;

        /// <summary>
        /// UnitFactory의 생성자.
        /// </summary>
        /// <param name="random">난수 생성기</param>
        /// <param name="createTimer">타이머 생성 팩토리 함수</param>
        /// <param name="targetFinder">대상 검색기</param>
        /// <param name="createUnitView">유닛 뷰 생성 팩토리 함수</param>
        /// <param name="logger">로거</param>
        /// <param name="createSkill">스킬 생성 팩토리 함수</param>
        /// <param name="gameDataStore">게임 데이터 저장소</param>
        /// <param name="createUnitLogic">유닛 로직 생성 팩토리 함수</param>
        /// <param name="createSkillEntity">스킬 엔티티 생성 팩토리 함수</param>
        public UnitFactory(Random random, Func<Timer> createTimer, ITargetFinder targetFinder,
            Func<string, View.BattleFieldScene.IUnit> createUnitView,
            ILogger logger, Func<Entity.IActiveSkill, Skills.ISkill> createSkill, GameDataStore gameDataStore,
            Func<IControllableUnit, string, IUnitLogic> createUnitLogic, Func<string, ISkill> createSkillEntity)
        {
            _random = random;
            _createTimer = createTimer;
            _targetFinder = targetFinder;
            _createUnitView = createUnitView;
            _logger = logger;
            _createSkill = createSkill;
            _createSkillEntity = createSkillEntity;
            _gameDataStore = gameDataStore;
            _createUnitLogic = createUnitLogic;
        }

        /// <summary>
        /// 유닛 엔티티를 기반으로 Unit 객체를 생성한다.
        /// 진영에 따라 적절한 적/아군 판별기를 할당하고 스킬을 추가한다.
        /// </summary>
        /// <param name="unitEntity">유닛 엔티티</param>
        /// <returns>생성된 Unit 객체</returns>
        public Unit Create(Entity.Unit unitEntity)
        {
            var unitView = _createUnitView(unitEntity.GetUnitBodyID());

            ITargetIdentifier enemyIdentifier = null;
            if (unitEntity.IsPlayerSide())
                enemyIdentifier = new PlayerEnemyIdentifier();
            else
                enemyIdentifier = new NonPlayerEnemyIdentifier();

            var timer = _createTimer();
            var unit = new Unit(unitView, unitEntity,
                _targetFinder, enemyIdentifier, _logger,
                _createSkill, _createUnitLogic);

            UnitCreatedEvent?.Invoke(this, new CreatedEventArgs<Unit>(unit));

            foreach (var skill in unitEntity.GetSkills())
                unit.AddSkill(skill);

            return unit;
        }

        /// <summary>
        /// 플레이어 유닛을 생성한다. 플레이어 진영으로 설정하고 플레이어 로직을 적용한다.
        /// </summary>
        /// <param name="entity">유닛 엔티티</param>
        /// <returns>생성된 플레이어 유닛</returns>
        public IUnit CreatePlayerUnit(Entity.Unit entity)
        {
            //var entity = CreateUnitEntity(unitID, 1);
            entity.SetPlayerSide(true);

            var unit = Create(entity);
            unit.SetUnitLogic("PLAYER");

            return unit;
        }

        /// <summary>
        /// 아군 NPC 유닛을 생성한다. 게임 데이터에서 로직 ID를 조회하여 적용한다.
        /// </summary>
        /// <param name="unitID">유닛 ID</param>
        /// <param name="level">유닛 레벨</param>
        /// <returns>생성된 NPC 유닛. 로직 데이터가 없으면 null</returns>
        public IUnit CreateNonPlayerUnit(string unitID, int level)
        {
            var gdNonPlayerUnitLogic = _gameDataStore.GetNonPlayerUnitLogics().FirstOrDefault(entry => entry.UnitID == unitID);
            if (gdNonPlayerUnitLogic == null)
            {
                _logger.Fatal($"Failed to find unit id ({unitID}) on non player unit logic table.");
                return null;
            }

            var entity = CreateUnitEntity(unitID, level);
            entity.SetPlayerSide(true);

            var unit = Create(entity);
            unit.SetUnitLogic(gdNonPlayerUnitLogic.UnitLogicID);

            return unit;
        }

        /// <summary>
        /// 적 유닛을 생성한다. 비플레이어 진영으로 설정하고 게임 데이터에서 로직 ID를 조회하여 적용한다.
        /// </summary>
        /// <param name="unitID">유닛 ID</param>
        /// <param name="level">유닛 레벨</param>
        /// <returns>생성된 적 유닛. 로직 데이터가 없으면 null</returns>
        public IUnit CreateEnemyUnit(string unitID, int level)
        {
            var gdNonPlayerUnitLogic = _gameDataStore.GetNonPlayerUnitLogics().FirstOrDefault(entry => entry.UnitID == unitID);
            if (gdNonPlayerUnitLogic == null)
            {
                _logger.Fatal($"Failed to find unit id ({unitID}) on non player unit logic table.");
                return null;
            }

            var entity = CreateUnitEntity(unitID, level);
            entity.SetPlayerSide(false);

            var unit = Create(entity);
            unit.SetUnitLogic(gdNonPlayerUnitLogic.UnitLogicID);

            return unit;
        }

        private Entity.Unit CreateUnitEntity(string unitID, int level)
        {
            var gdUnit = _gameDataStore.GetUnits().FirstOrDefault(entry => entry.ID == unitID);
            if (gdUnit == null)
            {
                _logger.Fatal($"Failed to find unit in game data for unit id ({unitID}).");
                return null;
            }

            var attributeMediator = new AttributeMediator(_gameDataStore, _logger);
            var entity = new Entity.Unit(_gameDataStore, attributeMediator);

            var gdBaseAttributes = _gameDataStore.GetUnitBaseAttributes().Where(entry => entry.ID == gdUnit.BaseAttributeID);
            if (gdBaseAttributes.Count() == 0)
                _logger.Warn($"There is no base stat for unit id ({unitID}).");
            var baseAttributes = new Dictionary<string, Attribute>();
            foreach (var entry in gdBaseAttributes)
                baseAttributes[entry.AttributeID] = entry.Value;
            entity.SetBaseAttributes(baseAttributes);

            var attributeGrowthFormulas = _gameDataStore.GetGrowthFormulas().Where(entry => entry.ID == gdUnit.AttributeGrowthFormulaID);
            entity.SetAttributeGrowthFormulas(attributeGrowthFormulas);

            entity.SetUnitBodyID(gdUnit.UnitBodyID);
            entity.SetLevel(level);

            entity.SetHP(entity.GetAttribute(AttributeID.MaxHP).AsInt());

            var gdSkill = _gameDataStore.GetUnitDefaultSkills().FirstOrDefault(entry => entry.UnitID == unitID);
            if (gdSkill != null)
            {
                var skill = _createSkillEntity.Invoke(gdSkill.SkillID);
                skill.SetOwner(entity);
                skill.AddToOwner();

                entity.SetDefaultSkill(skill.GetSkillID());
            }
            else
            {
                _logger.Info($"Failed to find default skill for unit id. ({unitID})");
            }

            return entity;
        }
    }
}
