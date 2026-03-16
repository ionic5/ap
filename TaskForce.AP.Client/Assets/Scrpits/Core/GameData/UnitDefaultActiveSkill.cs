namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 유닛의 기본 액티브 스킬 매핑을 정의하는 클래스.
    /// 특정 유닛이 기본으로 보유하는 액티브(수동 발동) 스킬을 지정한다.
    /// </summary>
    public class UnitDefaultActiveSkill
    {
        /// <summary>
        /// 대상 유닛의 식별자
        /// </summary>
        public string UnitID;

        /// <summary>
        /// 유닛에 기본 장착되는 액티브 스킬의 식별자
        /// </summary>
        public string SkillID;
    }
}
