namespace TaskForce.AP.Client.Core.Entity
{
    public abstract class Skill : ISkill
    {
        private readonly string _skillID;
        private readonly GameData.Skill _skillData;
        private readonly TextStore _textStore;
        private Unit _owner;
        private int _level;

        public Skill(string skillID, GameData.Skill skillData, TextStore textStore)
        {
            _skillID = skillID;
            _skillData = skillData;
            _textStore = textStore;
        }

        public string GetSkillID()
        {
            return _skillID;
        }

        public virtual void SetLevel(int value)
        {
            _level = value;
        }

        public void SetOwner(Unit owner)
        {
            _owner = owner;
        }

        protected Unit GetOwner()
        {
            return _owner;
        }

        protected Attribute GetUserAttribute(string attributeID)
        {
            return _owner.GetAttribute(attributeID);
        }

        public string GetIconID()
        {
            return "";
        }

        public string GetName()
        {
            return _textStore.GetText(_skillData.NameTextID);
        }

        public int GetLevel()
        {
            return _level;
        }

        public abstract void AddToOwner();

        public virtual void LevelUp()
        {
            SetLevel(_level++);
        }
    }
}
