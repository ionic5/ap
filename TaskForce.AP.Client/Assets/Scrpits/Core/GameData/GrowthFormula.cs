namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 성장 수식을 정의하는 게임 데이터 클래스.
    /// 특정 대상에 적용되는 레벨업 기반 성장 수식을 나타낸다.
    /// </summary>
    public class GrowthFormula
    {
        /// <summary>
        /// 성장 수식의 고유 식별자
        /// </summary>
        public string ID;

        /// <summary>
        /// 성장 수식이 적용되는 대상의 식별자
        /// </summary>
        public string TargetID;

        /// <summary>
        /// 성장 계산에 사용할 수식의 식별자
        /// </summary>
        public string FormulaID;
    }
}
