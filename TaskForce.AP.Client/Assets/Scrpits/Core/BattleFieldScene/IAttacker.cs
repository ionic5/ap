using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 공격 능력을 가진 객체를 나타내는 인터페이스.
    /// 스킬 조회, 방향/위치 확인, 적 판별, 공격 사거리 판정 기능을 제공한다.
    /// </summary>
    public interface IAttacker
    {
        /// <summary>
        /// 지정된 ID의 액티브 스킬을 가져온다.
        /// </summary>
        /// <param name="skillID">스킬 ID</param>
        /// <returns>해당 ID의 액티브 스킬</returns>
        Core.Entity.IActiveSkill GetSkill(string skillID);

        /// <summary>
        /// 현재 바라보는 방향 벡터를 가져온다.
        /// </summary>
        /// <returns>방향 벡터</returns>
        Vector2 GetDirection();

        /// <summary>
        /// 현재 위치를 가져온다.
        /// </summary>
        /// <returns>위치 벡터</returns>
        Vector2 GetPosition();

        /// <summary>
        /// 대상이 적인지 판별한다.
        /// </summary>
        /// <param name="entry">판별할 대상</param>
        /// <returns>적이면 true</returns>
        bool IsEnemy(ITarget entry);

        /// <summary>
        /// 대상이 공격 사거리 내에 있는지 판정한다.
        /// </summary>
        /// <param name="mainTarget">판정할 대상</param>
        /// <returns>사거리 내이면 true</returns>
        bool IsTargetInAttackRange(ITarget mainTarget);
    }
}
