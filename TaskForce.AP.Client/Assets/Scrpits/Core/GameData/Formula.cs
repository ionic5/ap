namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 수식 정보를 정의하는 게임 데이터 클래스.
    /// 수식의 고유 식별자와 계산 방식(고정값/퍼센트)을 지정한다.
    /// </summary>
    public class Formula
    {
        /// <summary>
        /// 수식의 고유 식별자
        /// </summary>
        public string ID;

        /// <summary>
        /// 수식의 계산 방식 (예: 고정값, 퍼센트)
        /// </summary>
        public string CalculationType;
    }
}
