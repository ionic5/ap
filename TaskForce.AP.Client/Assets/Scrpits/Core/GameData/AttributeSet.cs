namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 속성 집합을 정의하는 게임 데이터 클래스.
    /// 특정 속성 집합 ID와 해당 속성 ID 간의 매핑을 나타낸다.
    /// </summary>
    public class AttributeSet
    {
        /// <summary>
        /// 속성 집합의 고유 식별자
        /// </summary>
        public string ID;

        /// <summary>
        /// 이 집합에 포함된 속성의 식별자
        /// </summary>
        public string AttributeID;
    }
}
