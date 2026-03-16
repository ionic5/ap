namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 계수와 수식 간의 연결을 정의하는 클래스.
    /// 대상 계수 키와 적용할 수식을 매핑한다.
    /// </summary>
    public class CoefficientFormulaSet
    {
        /// <summary>
        /// 계수-수식 집합의 고유 식별자
        /// </summary>
        public string ID;

        /// <summary>
        /// 대상 계수를 식별하는 키 문자열
        /// </summary>
        public string TargetCoefficientKey;

        /// <summary>
        /// 적용할 수식의 식별자
        /// </summary>
        public string FormulaID;
    }
}
