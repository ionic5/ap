namespace TaskForce.AP.Client.Core.Entity
{
    public interface IActiveSkill : ISkill
    {
        Attribute GetAttribute(string attributeID);
    }
}
