using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class SkillFactory
    {
        private readonly Dictionary<string, Func<Entity.IActiveSkill, ISkill>> _creators;

        public SkillFactory()
        {
            _creators = new Dictionary<string, Func<Core.Entity.IActiveSkill, ISkill>>();
        }

        public void AddCreator(string key, Func<Entity.IActiveSkill, ISkill> tmp)
        {
            _creators.Add(key, tmp);
        }

        public ISkill Create(Entity.IActiveSkill skill)
        {
            if (_creators.TryGetValue(skill.GetSkillID(), out Func<Entity.IActiveSkill, ISkill> func))
                return func.Invoke(skill);
            return null;
        }
    }
}
