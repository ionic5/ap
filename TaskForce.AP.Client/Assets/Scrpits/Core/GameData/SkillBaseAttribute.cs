namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 스킬의 기본 속성값을 정의하는 클래스.
    /// 특정 스킬에 대해 속성 ID와 해당 속성의 초기값을 지정한다.
    /// </summary>
    public class SkillBaseAttribute
    {
        /// <summary>
        /// 대상 스킬의 식별자
        /// </summary>
        public string SkillID;

        /// <summary>
        /// 스킬에 연결된 속성의 식별자
        /// </summary>
        public string AttributeID;

        /// <summary>
        /// 스킬 속성의 기본값
        /// </summary>
        public Attribute Value;
    }
}
