namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 일반 활성 스킬 클래스. AttributeMediator를 통해 자체 속성을 관리하며,
    /// 레벨 변경 시 속성 중재자의 레벨도 함께 갱신한다.
    /// </summary>
    public class CommonSkill : ActiveSkill, IActiveSkill
    {
        /// <summary>이 스킬의 속성을 관리하는 속성 중재자.</summary>
        private readonly AttributeMediator _attributeMediator;

        /// <summary>
        /// CommonSkill의 생성자.
        /// </summary>
        /// <param name="skillID">스킬 고유 식별자.</param>
        /// <param name="skillData">게임 데이터에서 가져온 스킬 정보.</param>
        /// <param name="textStore">텍스트 리소스 저장소.</param>
        /// <param name="attributeMediator">스킬 속성을 관리할 중재자.</param>
        public CommonSkill(string skillID, GameData.Skill skillData,
            TextStore textStore, AttributeMediator attributeMediator) : base(skillID, skillData, textStore)
        {
            _attributeMediator = attributeMediator;
        }

        /// <summary>
        /// 스킬 레벨을 설정하고 속성 중재자의 레벨도 함께 갱신한다.
        /// </summary>
        /// <param name="value">설정할 레벨 값.</param>
        public override void SetLevel(int value)
        {
            base.SetLevel(value);

            _attributeMediator.SetLevel(value);
        }

        /// <summary>
        /// 속성 중재자를 통해 지정된 속성 ID에 해당하는 속성 값을 반환한다.
        /// </summary>
        /// <param name="attributeID">조회할 속성의 고유 식별자.</param>
        /// <returns>해당 속성 값을 담은 <see cref="Attribute"/> 객체.</returns>
        public override Attribute GetAttribute(string attributeID)
        {
            return _attributeMediator.GetAttribute(attributeID);
        }
    }
}
