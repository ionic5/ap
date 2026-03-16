using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 스킬의 공통 동작을 정의하는 인터페이스.
    /// 모든 액티브 스킬은 이 인터페이스를 구현하여 사용, 쿨다운, 대상 판정 등의 기능을 제공한다.
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// 즉시 시전 스킬 여부를 반환한다.
        /// </summary>
        /// <returns>즉시 시전 스킬이면 true, 아니면 false</returns>
        bool IsInstantSkill();

        /// <summary>
        /// 힐(치유) 효과를 가진 스킬인지 여부를 반환한다.
        /// </summary>
        /// <returns>힐 효과가 있으면 true, 아니면 false</returns>
        bool HasHealEffect();

        /// <summary>
        /// 지정한 대상이 유닛의 스킬 사거리 내에 있는지 판정한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <param name="target">사거리 판정 대상</param>
        /// <returns>대상이 사거리 내에 있으면 true, 아니면 false</returns>
        bool IsTargetInRange(IUnit unit, ITarget target);

        /// <summary>
        /// 유닛의 스킬 사거리 내에 있는 모든 대상을 반환한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <returns>사거리 내 대상 목록</returns>
        IEnumerable<ITarget> GetTargetsInRange(IUnit unit);

        /// <summary>
        /// 스킬의 고유 식별자를 반환한다.
        /// </summary>
        /// <returns>스킬 ID 문자열</returns>
        string GetSkillID();

        /// <summary>
        /// 스킬의 소유자(사용 유닛)를 설정한다.
        /// </summary>
        /// <param name="owner">스킬을 소유할 유닛</param>
        void SetOwner(IUnit owner);

        /// <summary>
        /// 스킬의 쿨다운이 완료되었는지 여부를 반환한다.
        /// </summary>
        /// <returns>쿨다운이 끝났으면 true, 아니면 false</returns>
        bool IsCooldownFinished();

        /// <summary>
        /// 스킬을 사용한다.
        /// </summary>
        /// <param name="args">스킬 사용에 필요한 인자 (대상, 완료 콜백 등)</param>
        void Use(UseSkillArgs args);

        /// <summary>
        /// 스킬 사용이 완료되었는지 여부를 반환한다.
        /// </summary>
        /// <returns>완료되었으면 true, 아니면 false</returns>
        bool IsCompleted();

        /// <summary>
        /// 진행 중인 스킬 사용을 취소한다.
        /// </summary>
        void Cancel();
    }
}
