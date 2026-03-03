namespace TaskForce.AP.Client.Core.Entity
{
    public class MeleeAttackSkill : ActiveSkill, IActiveSkill
    {
        public MeleeAttackSkill(string skillID, GameData.Skill skillData,
            TextStore textStore) : base(skillID, skillData, textStore)
        {
        }

        public override Attribute GetAttribute(string attributeID)
        {
            return GetUserAttribute(attributeID);
        }
    }
}
