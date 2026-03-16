using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 스킬 ID에 따라 적절한 ISkill 인스턴스를 생성하는 팩토리 클래스.
    /// 스킬 ID와 생성 함수를 등록해 두고, 엔티티 정보를 기반으로 스킬 객체를 생성한다.
    /// </summary>
    public class SkillFactory
    {
        /// <summary>
        /// 스킬 ID를 키로, 스킬 생성 함수를 값으로 보관하는 딕셔너리
        /// </summary>
        private readonly Dictionary<string, Func<Entity.IActiveSkill, ISkill>> _creators;

        /// <summary>
        /// SkillFactory의 생성자. 빈 생성자 딕셔너리를 초기화한다.
        /// </summary>
        public SkillFactory()
        {
            _creators = new Dictionary<string, Func<Core.Entity.IActiveSkill, ISkill>>();
        }

        /// <summary>
        /// 스킬 ID에 대응하는 생성 함수를 등록한다.
        /// </summary>
        /// <param name="key">스킬 ID</param>
        /// <param name="tmp">스킬 엔티티를 받아 ISkill을 반환하는 생성 함수</param>
        public void AddCreator(string key, Func<Entity.IActiveSkill, ISkill> tmp)
        {
            _creators.Add(key, tmp);
        }

        /// <summary>
        /// 스킬 엔티티의 ID로 등록된 생성 함수를 찾아 스킬 인스턴스를 생성한다.
        /// </summary>
        /// <param name="skill">스킬 데이터 엔티티</param>
        /// <returns>생성된 ISkill 인스턴스. 등록된 생성 함수가 없으면 null</returns>
        public ISkill Create(Entity.IActiveSkill skill)
        {
            if (_creators.TryGetValue(skill.GetSkillID(), out Func<Entity.IActiveSkill, ISkill> func))
                return func.Invoke(skill);
            return null;
        }
    }
}
