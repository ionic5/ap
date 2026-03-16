namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 스킬의 추상 기본 클래스. ISkill 인터페이스를 구현하며,
    /// 스킬 ID, 레벨, 소유자 관리 등 공통 기능을 제공한다.
    /// </summary>
    public abstract class Skill : ISkill
    {
        /// <summary>스킬 고유 식별자.</summary>
        private readonly string _skillID;

        /// <summary>게임 데이터에서 가져온 스킬 정보.</summary>
        private readonly GameData.Skill _skillData;

        /// <summary>텍스트 리소스 저장소.</summary>
        private readonly TextStore _textStore;

        /// <summary>이 스킬을 소유한 유닛.</summary>
        private Unit _owner;

        /// <summary>스킬의 현재 레벨.</summary>
        private int _level;

        /// <summary>
        /// Skill의 생성자.
        /// </summary>
        /// <param name="skillID">스킬 고유 식별자.</param>
        /// <param name="skillData">게임 데이터에서 가져온 스킬 정보.</param>
        /// <param name="textStore">텍스트 리소스 저장소.</param>
        public Skill(string skillID, GameData.Skill skillData, TextStore textStore)
        {
            _skillID = skillID;
            _skillData = skillData;
            _textStore = textStore;
        }

        /// <summary>
        /// 스킬의 고유 식별자를 반환한다.
        /// </summary>
        /// <returns>스킬 ID 문자열.</returns>
        public string GetSkillID()
        {
            return _skillID;
        }

        /// <summary>
        /// 스킬의 레벨을 설정한다.
        /// </summary>
        /// <param name="value">설정할 레벨 값.</param>
        public virtual void SetLevel(int value)
        {
            _level = value;
        }

        /// <summary>
        /// 스킬의 소유 유닛을 설정한다.
        /// </summary>
        /// <param name="owner">소유자로 설정할 유닛.</param>
        public void SetOwner(Unit owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// 소유 유닛을 반환한다.
        /// </summary>
        /// <returns>소유 유닛 또는 null.</returns>
        protected Unit GetOwner()
        {
            return _owner;
        }

        /// <summary>
        /// 소유 유닛의 지정된 속성 값을 반환한다.
        /// </summary>
        /// <param name="attributeID">조회할 속성의 고유 식별자.</param>
        /// <returns>소유 유닛의 해당 속성 값.</returns>
        protected Attribute GetUserAttribute(string attributeID)
        {
            return _owner.GetAttribute(attributeID);
        }

        /// <summary>
        /// 스킬 아이콘의 리소스 ID를 반환한다.
        /// </summary>
        /// <returns>아이콘 ID 문자열 (현재 빈 문자열).</returns>
        public string GetIconID()
        {
            return "";
        }

        /// <summary>
        /// 스킬의 표시 이름을 텍스트 저장소에서 조회하여 반환한다.
        /// </summary>
        /// <returns>스킬 이름 문자열.</returns>
        public string GetName()
        {
            return _textStore.GetText(_skillData.NameTextID);
        }

        /// <summary>
        /// 스킬의 현재 레벨을 반환한다.
        /// </summary>
        /// <returns>현재 레벨 값.</returns>
        public int GetLevel()
        {
            return _level;
        }

        /// <summary>
        /// 스킬을 소유 유닛에 추가한다. 하위 클래스에서 구현해야 한다.
        /// </summary>
        public abstract void AddToOwner();

        /// <summary>
        /// 스킬의 레벨을 1 올린다.
        /// </summary>
        public virtual void LevelUp()
        {
            SetLevel(_level++);
        }
    }
}
