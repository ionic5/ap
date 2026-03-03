namespace TaskForce.AP.Client.Core.Entity
{
    public class CommonSkill : ActiveSkill, IActiveSkill
    {
        private readonly AttributeBlock _attributeBlock;

        public CommonSkill(string skillID, GameData.Skill skillData,
            TextStore textStore, AttributeBlock attributeBlock) : base(skillID, skillData, textStore)
        {
            _attributeBlock = attributeBlock;
        }

        public override void SetLevel(int value)
        {
            base.SetLevel(value);

            _attributeBlock.SetLevel(value);
        }

        public override Attribute GetAttribute(string attributeID)
        {
            return _attributeBlock.GetAttribute(attributeID);
        }
    }
}
