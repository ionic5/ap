namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 스킬과 성장 수식 간의 연결을 정의하는 클래스.
    /// 특정 스킬의 레벨업 시 적용되는 성장 수식을 매핑한다.
    /// </summary>
    public class SkillGrowthFormula
    {
        /// <summary>
        /// 대상 스킬의 식별자
        /// </summary>
        public string SkillID;

        /// <summary>
        /// 스킬에 적용할 성장 수식의 식별자
        /// </summary>
        public string GrowthFormulaID;
    }
}
