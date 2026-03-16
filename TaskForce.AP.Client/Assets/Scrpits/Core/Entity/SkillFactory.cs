using System.Linq;

namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 스킬 엔티티를 생성하는 팩토리 클래스. 스킬 ID에 따라 적절한 스킬 타입
    /// (MeleeAttackSkill, ModifyAttributeSkill, CommonSkill)을 생성한다.
    /// </summary>
    public class SkillFactory
    {
        /// <summary>게임 데이터 저장소.</summary>
        private readonly GameDataStore _gameDataStore;

        /// <summary>텍스트 리소스 저장소.</summary>
        private readonly TextStore _textStore;

        /// <summary>로거 인스턴스.</summary>
        private readonly ILogger _logger;

        /// <summary>속성 변경 효과 팩토리.</summary>
        private readonly ModifyAttributeEffectFactory _modifyAttributeEffectFactory;

        /// <summary>
        /// SkillFactory의 생성자.
        /// </summary>
        /// <param name="gameDataStore">게임 데이터 저장소.</param>
        /// <param name="logger">로거 인스턴스.</param>
        /// <param name="textStore">텍스트 리소스 저장소.</param>
        /// <param name="modifyAttributeEffectFactory">속성 변경 효과 팩토리.</param>
        public SkillFactory(GameDataStore gameDataStore, ILogger logger,
            TextStore textStore, ModifyAttributeEffectFactory modifyAttributeEffectFactory)
        {
            _gameDataStore = gameDataStore;
            _logger = logger;
            _textStore = textStore;
            _modifyAttributeEffectFactory = modifyAttributeEffectFactory;
        }

        /// <summary>
        /// 소유자와 레벨이 설정된 스킬 엔티티를 생성한다.
        /// </summary>
        /// <param name="owner">스킬을 소유할 유닛.</param>
        /// <param name="skillID">생성할 스킬의 고유 식별자.</param>
        /// <param name="level">스킬의 초기 레벨.</param>
        /// <returns>생성된 <see cref="ISkill"/> 인스턴스.</returns>
        public ISkill CreateSkillEntity(Entity.Unit owner, string skillID, int level)
        {
            var skill = CreateSkill(skillID);
            skill.SetOwner(owner);
            skill.SetLevel(level);

            return skill;
        }

        /// <summary>
        /// 스킬 ID에 따라 적절한 타입의 스킬을 생성한다. 소유자와 레벨은 설정되지 않는다.
        /// </summary>
        /// <param name="skillID">생성할 스킬의 고유 식별자.</param>
        /// <returns>생성된 <see cref="ISkill"/> 인스턴스. 스킬 데이터를 찾지 못하면 null.</returns>
        public ISkill CreateSkill(string skillID)
        {
            var skillData = _gameDataStore.GetSkills().Where(entry => entry.ID == skillID).FirstOrDefault();
            if (skillData == null)
            {
                _logger.Fatal($"Failed to find skill data for {skillID}");
                return null;
            }

            if (skillID == SkillID.MeleeAttack)
                return new Entity.MeleeAttackSkill(skillID, skillData, _textStore);

            if (skillID == SkillID.CleavingAttack)
            {
                var effects = _gameDataStore.GetModifyAttributeSkillEffects(skillID);
                var skill = new Entity.ModifyAttributeSkill(skillID, skillData,
                    _textStore, effects, _modifyAttributeEffectFactory.Create);
                skill.SetLevel(1);
                return skill;
            }

            var attributeMediator = new AttributeMediator(_gameDataStore, _logger);

            attributeMediator.SetBaseAttributes(_gameDataStore.GetSkillBaseAttributes(skillID));
            attributeMediator.SetGrowthFormulas(_gameDataStore.GetSkillGrowthFormulas(skillID));

            return new Entity.CommonSkill(skillID, skillData, _textStore, attributeMediator);
        }
    }
}
