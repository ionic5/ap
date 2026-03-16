namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 활성 스킬의 추상 기본 클래스. Skill을 상속하고 IActiveSkill 인터페이스를 구현하여
    /// 소유자에게 추가되고 속성을 조회할 수 있는 기능을 제공한다.
    /// </summary>
    public abstract class ActiveSkill : Skill, IActiveSkill
    {
        /// <summary>
        /// ActiveSkill의 생성자.
        /// </summary>
        /// <param name="skillID">스킬 고유 식별자.</param>
        /// <param name="skillData">게임 데이터에서 가져온 스킬 정보.</param>
        /// <param name="textStore">텍스트 리소스 저장소.</param>
        protected ActiveSkill(string skillID, GameData.Skill skillData, TextStore textStore) : base(skillID, skillData, textStore)
        {
        }

        /// <summary>
        /// 이 스킬을 소유 유닛에 활성 스킬로 추가한다.
        /// </summary>
        public override void AddToOwner()
        {
            GetOwner().AddSkill(this);
        }

        /// <summary>
        /// 지정된 속성 ID에 해당하는 속성 값을 반환한다.
        /// </summary>
        /// <param name="attributeID">조회할 속성의 고유 식별자.</param>
        /// <returns>해당 속성 값을 담은 <see cref="Attribute"/> 객체.</returns>
        public abstract Attribute GetAttribute(string attributeID);
    }
}
