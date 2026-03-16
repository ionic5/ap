namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 유닛의 기본 속성값을 정의하는 클래스.
    /// 특정 유닛에 대해 속성 ID와 해당 속성의 초기값을 지정한다.
    /// </summary>
    public class UnitBaseAttribute
    {
        /// <summary>
        /// 기본 속성 데이터의 고유 식별자
        /// </summary>
        public string ID;

        /// <summary>
        /// 유닛에 연결된 속성의 식별자
        /// </summary>
        public string AttributeID;

        /// <summary>
        /// 유닛 속성의 기본값
        /// </summary>
        public Attribute Value;
    }
}
