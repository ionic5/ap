namespace TaskForce.AP.Client.Core.Entity
{
    public abstract class ActiveSkill : Skill, IActiveSkill
    {
        protected ActiveSkill(string skillID, GameData.Skill skillData, TextStore textStore) : base(skillID, skillData, textStore)
        {
        }

        public override void AddToOwner()
        {
            GetOwner().AddSkill(this);
        }

        public abstract Attribute GetAttribute(string attributeID);
    }
}
