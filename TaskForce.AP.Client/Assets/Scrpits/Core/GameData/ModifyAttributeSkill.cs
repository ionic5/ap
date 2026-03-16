namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 스킬과 속성 수정 효과 간의 연결을 정의하는 클래스.
    /// 특정 스킬이 어떤 속성 수정 효과를 발동하는지를 매핑한다.
    /// </summary>
    public class ModifyAttributeSkill
    {
        /// <summary>
        /// 속성 수정 효과를 발동하는 스킬의 식별자
        /// </summary>
        public string SkillID;

        /// <summary>
        /// 스킬에 연결된 속성 수정 효과의 식별자
        /// </summary>
        public string ModifyAttributeEffectID;
    }
}
