namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 근접 공격 스킬 클래스. 자체 속성을 갖지 않고,
    /// 소유 유닛의 속성을 그대로 사용하는 활성 스킬이다.
    /// </summary>
    public class MeleeAttackSkill : ActiveSkill, IActiveSkill
    {
        /// <summary>
        /// MeleeAttackSkill의 생성자.
        /// </summary>
        /// <param name="skillID">스킬 고유 식별자.</param>
        /// <param name="skillData">게임 데이터에서 가져온 스킬 정보.</param>
        /// <param name="textStore">텍스트 리소스 저장소.</param>
        public MeleeAttackSkill(string skillID, GameData.Skill skillData,
            TextStore textStore) : base(skillID, skillData, textStore)
        {
        }

        /// <summary>
        /// 소유 유닛의 속성 값을 그대로 반환한다.
        /// </summary>
        /// <param name="attributeID">조회할 속성의 고유 식별자.</param>
        /// <returns>소유 유닛의 해당 속성 값을 담은 <see cref="Attribute"/> 객체.</returns>
        public override Attribute GetAttribute(string attributeID)
        {
            return GetUserAttribute(attributeID);
        }
    }
}
