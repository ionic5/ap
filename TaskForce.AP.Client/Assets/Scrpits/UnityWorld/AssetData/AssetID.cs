namespace TaskForce.AP.Client.UnityWorld.AssetData
{
    /// <summary>
    /// 게임 데이터 에셋의 식별자 상수를 정의하는 클래스.
    /// CSV 파일 기반 에셋을 로드할 때 사용되는 고유 키 문자열을 제공한다.
    /// </summary>
    public class AssetID
    {
        /// <summary>게임 상수 데이터 에셋 식별자</summary>
        public const string Constants = "CONSTANTS";
        /// <summary>텍스트(다국어) 데이터 에셋 식별자</summary>
        public const string Text = "TEXT";
        /// <summary>계수(Coefficient) 데이터 에셋 식별자</summary>
        public const string Coefficient = "COEFFICIENT";
        /// <summary>스킬 기본 속성 데이터 에셋 식별자</summary>
        public const string SkillBaseAttribute = "SKILL_BASE_ATTRIBUTE";

        /// <summary>속성 변경 효과 데이터 에셋 식별자</summary>
        public const string ModifyAttributeEffect = "MODIFY_ATTRIBUTE_EFFECT";
        /// <summary>스테이지별 적 유닛 데이터 에셋 식별자</summary>
        public const string StageEnemyUnit = "STAGE_ENEMY_UNIT";
        /// <summary>스테이지 데이터 에셋 식별자</summary>
        public const string Stage = "STAGE";
        /// <summary>유닛 기본 속성 데이터 에셋 식별자</summary>
        public const string UnitBaseAttribute = "UNIT_BASE_ATTRIBUTE";
        /// <summary>유닛 데이터 에셋 식별자</summary>
        public const string Unit = "UNIT";

        /// <summary>비플레이어 유닛 로직 데이터 에셋 식별자</summary>
        public const string NonPlayerUnitLogic = "NON_PLAYER_UNIT_LOGIC";
        /// <summary>성장 공식 데이터 에셋 식별자</summary>
        public const string GrowthFormula = "GROWTH_FORMULA";
        /// <summary>공식(Formula) 데이터 에셋 식별자</summary>
        public const string Formula = "FORMULA";
        /// <summary>유닛 기본 스킬 데이터 에셋 식별자</summary>
        public const string UnitDefaultSkill = "UNIT_DEFAULT_SKILL";
        /// <summary>스킬 데이터 에셋 식별자</summary>
        public const string Skill = "SKILL";

        /// <summary>스킬 성장 공식 데이터 에셋 식별자</summary>
        public const string SkillGrowthFormula = "SKILL_GROWTH_FORMULA";
        /// <summary>레벨업 보상 스킬 데이터 에셋 식별자</summary>
        public const string LevelUpRewardSkill = "LEVEL_UP_REWARD_SKILL";
        /// <summary>속성 집합 데이터 에셋 식별자</summary>
        public const string AttributeSet = "ATTRIBUTE_SET";
        /// <summary>속성 변경 스킬 데이터 에셋 식별자</summary>
        public const string ModifyAttributeSkill = "MODIFY_ATTRIBUTE_SKILL";
        /// <summary>계수 공식 집합 데이터 에셋 식별자</summary>
        public const string CoefficientFormulaSet = "COEFFICIENT_FORMULA_SET";

        /// <summary>유닛 기본 액티브 스킬 데이터 에셋 식별자</summary>
        public const string UnitDefaultActiveSkill = "UNIT_DEFAULT_ACTIVE_SKILL";
    }
}
