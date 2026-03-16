namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 속성 수치 계산 방식을 정의하는 상수 클래스.
    /// 고정값(덧셈) 방식과 퍼센트(비율) 방식을 제공한다.
    /// </summary>
    public class CalculateType
    {
        /// <summary>
        /// 고정값(덧셈) 계산 방식
        /// </summary>
        public const string Flat = "ADDITIVE";

        /// <summary>
        /// 퍼센트(비율) 계산 방식
        /// </summary>
        public const string Percent = "PERCENT";
    }
}
