namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 수식에서 사용되는 계수(coefficient) 데이터를 나타내는 클래스.
    /// 특정 수식에 대해 키-값 쌍으로 계수를 정의한다.
    /// </summary>
    public class Coefficient
    {
        /// <summary>
        /// 이 계수가 속한 수식의 식별자
        /// </summary>
        public string FormulaID;

        /// <summary>
        /// 계수를 구분하는 키 문자열
        /// </summary>
        public string Key;

        /// <summary>
        /// 계수의 수치 값
        /// </summary>
        public float Value;
    }
}
