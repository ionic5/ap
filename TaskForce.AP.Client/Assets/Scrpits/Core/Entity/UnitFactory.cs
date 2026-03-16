using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 유닛 엔티티를 생성하는 팩토리 클래스. 게임 데이터에서 유닛 정보를 조회하여
    /// 기본 속성, 성장 공식, 기본 스킬이 설정된 유닛 인스턴스를 생성한다.
    /// </summary>
    public class UnitFactory
    {
        /// <summary>로거 인스턴스.</summary>
        private readonly Core.ILogger _logger;

        /// <summary>게임 데이터 저장소.</summary>
        private readonly GameDataStore _gameDataStore;

        /// <summary>스킬 ID로 스킬을 생성하는 팩토리 함수.</summary>
        private readonly Func<string, Entity.ISkill> _createSkill;

        /// <summary>
        /// UnitFactory의 생성자.
        /// </summary>
        /// <param name="logger">로거 인스턴스.</param>
        /// <param name="gameDataStore">게임 데이터 저장소.</param>
        /// <param name="createSkill">스킬 ID를 받아 스킬을 생성하는 함수.</param>
        public UnitFactory(ILogger logger, GameDataStore gameDataStore, Func<string, ISkill> createSkill)
        {
            _logger = logger;
            _gameDataStore = gameDataStore;
            _createSkill = createSkill;
        }

        /// <summary>
        /// 지정된 유닛 ID를 기반으로 유닛 엔티티를 생성한다.
        /// 기본 속성, 성장 공식, 체력, 기본 스킬을 설정하여 완전한 유닛을 반환한다.
        /// </summary>
        /// <param name="unitID">생성할 유닛의 고유 식별자.</param>
        /// <returns>생성된 <see cref="Unit"/> 인스턴스. 유닛 데이터를 찾지 못하면 null.</returns>
        public Entity.Unit CreateUnitEntity(string unitID)
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
            entity.SetLevel(1);

            entity.SetHP(entity.GetAttribute(AttributeID.MaxHP).AsInt());

            var skillDatas = _gameDataStore.GetUnitDefaultSkills().Where(entry => entry.UnitID == unitID);
            foreach (var data in skillDatas)
            {
                var skill = _createSkill.Invoke(data.SkillID);
                skill.SetOwner(entity);
                skill.AddToOwner();
            }

            var defaultSkillData = _gameDataStore.GetUnitDefaultActiveSkillByUnitID(unitID);
            if (defaultSkillData != null)
                entity.SetDefaultSkill(defaultSkillData.SkillID);
            else
                _logger.Info($"Failed to find default skill for unit id. ({unitID})");

            return entity;
        }
    }
}
