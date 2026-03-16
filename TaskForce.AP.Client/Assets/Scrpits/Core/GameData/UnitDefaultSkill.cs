namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 유닛의 기본 스킬 매핑을 정의하는 클래스.
    /// 특정 유닛이 기본으로 보유하는 패시브 또는 일반 스킬을 지정한다.
    /// </summary>
    public class UnitDefaultSkill
    {
        /// <summary>
        /// 대상 유닛의 식별자
        /// </summary>
        public string UnitID;

        /// <summary>
        /// 유닛에 기본 장착되는 스킬의 식별자
        /// </summary>
        public string SkillID;
    }
}
