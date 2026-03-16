using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 액티브 스킬의 기본 구현을 제공하는 추상 클래스.
    /// ISkill 인터페이스를 구현하며, 스킬 엔티티 정보 접근 및 소유자 관리 기능을 공통으로 제공한다.
    /// </summary>
    public abstract class ActiveSkill : ISkill
    {
        /// <summary>
        /// 스킬의 데이터 엔티티 (속성값 조회에 사용)
        /// </summary>
        private readonly Entity.IActiveSkill _skillEntity;

        /// <summary>
        /// 이 스킬을 보유한 유닛
        /// </summary>
        private IUnit _owner;

        /// <summary>
        /// ActiveSkill의 생성자.
        /// </summary>
        /// <param name="skillEntity">스킬 데이터 엔티티</param>
        public ActiveSkill(Core.Entity.IActiveSkill skillEntity)
        {
            _skillEntity = skillEntity;
        }

        /// <summary>
        /// 스킬의 고유 식별자를 반환한다.
        /// </summary>
        /// <returns>스킬 ID 문자열</returns>
        public string GetSkillID()
        {
            return _skillEntity.GetSkillID();
        }

        /// <summary>
        /// 스킬의 소유자를 설정한다.
        /// </summary>
        /// <param name="owner">스킬을 소유할 유닛</param>
        public void SetOwner(IUnit owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// 스킬 소유 유닛을 반환한다.
        /// </summary>
        /// <returns>소유 유닛</returns>
        protected IUnit GetOwner()
        {
            return _owner;
        }

        /// <summary>
        /// 스킬 엔티티에서 지정한 속성값을 조회한다.
        /// </summary>
        /// <param name="attributeID">조회할 속성 ID</param>
        /// <returns>해당 속성값을 담은 Attribute 객체</returns>
        protected Attribute GetAttribute(string attributeID)
        {
            return _skillEntity.GetAttribute(attributeID);
        }

        /// <summary>
        /// 즉시 시전 스킬 여부를 반환한다. 기본값은 false이다.
        /// </summary>
        /// <returns>즉시 시전이면 true, 아니면 false</returns>
        public virtual bool IsInstantSkill()
        {
            return false;
        }

        /// <summary>
        /// 힐(치유) 효과를 가진 스킬인지 여부를 반환한다. 기본값은 false이다.
        /// </summary>
        /// <returns>힐 효과가 있으면 true, 아니면 false</returns>
        public virtual bool HasHealEffect()
        {
            return false;
        }

        /// <summary>
        /// 지정한 대상이 유닛의 스킬 사거리 내에 있는지 판정한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <param name="target">사거리 판정 대상</param>
        /// <returns>대상이 사거리 내에 있으면 true</returns>
        public abstract bool IsTargetInRange(IUnit unit, ITarget target);

        /// <summary>
        /// 유닛의 스킬 사거리 내에 있는 모든 대상을 반환한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <returns>사거리 내 대상 목록</returns>
        public abstract IEnumerable<ITarget> GetTargetsInRange(IUnit unit);

        /// <summary>
        /// 스킬 사용이 완료되었는지 여부를 반환한다.
        /// </summary>
        /// <returns>완료되었으면 true</returns>
        public abstract bool IsCompleted();

        /// <summary>
        /// 스킬의 쿨다운이 완료되었는지 여부를 반환한다.
        /// </summary>
        /// <returns>쿨다운이 끝났으면 true</returns>
        public abstract bool IsCooldownFinished();

        /// <summary>
        /// 스킬을 사용한다.
        /// </summary>
        /// <param name="args">스킬 사용 인자 (대상, 완료 콜백 등)</param>
        public abstract void Use(UseSkillArgs args);

        /// <summary>
        /// 진행 중인 스킬 사용을 취소한다.
        /// </summary>
        public abstract void Cancel();
    }
}
