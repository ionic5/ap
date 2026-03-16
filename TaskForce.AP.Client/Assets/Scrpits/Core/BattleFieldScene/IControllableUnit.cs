using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 유닛 로직에 의해 제어 가능한 유닛 인터페이스.
    /// 이동, 공격 사거리 판정, 경험치 획득, 스킬 목록 조회 등 제어에 필요한 기능을 제공한다.
    /// </summary>
    public interface IControllableUnit : IUnit, IFollowable
    {
        /// <summary>
        /// 이 유닛의 마스터(소환자) 유닛을 가져온다.
        /// </summary>
        /// <returns>마스터 유닛</returns>
        IUnit GetMaster();

        /// <summary>
        /// 마스터를 따라갈 때의 추적 범위를 가져온다.
        /// </summary>
        /// <returns>추적 범위</returns>
        float GetFollowRange();

        /// <summary>
        /// 경험치를 추가한다.
        /// </summary>
        /// <param name="exp">추가할 경험치 양</param>
        void AddExp(int exp);

        /// <summary>
        /// 아이템 줍기 범위를 가져온다.
        /// </summary>
        /// <returns>줍기 범위</returns>
        float GetPickUpRange();

        /// <summary>
        /// 대상이 공격 사거리 내에 있는지 판정한다.
        /// </summary>
        /// <param name="mainTarget">판정할 대상</param>
        /// <returns>사거리 내이면 true</returns>
        bool IsTargetInAttackRange(ITarget mainTarget);

        /// <summary>
        /// 지정된 방향으로 이동한다.
        /// </summary>
        /// <param name="direction">이동 방향 벡터</param>
        void Move(Vector2 direction);

        /// <summary>
        /// 지정된 목적지로 이동한다.
        /// </summary>
        /// <param name="vector2">목적지 좌표</param>
        void MoveTo(Vector2 vector2);

        /// <summary>
        /// 보유 중인 스킬 목록을 가져온다.
        /// </summary>
        /// <returns>스킬 목록</returns>
        IEnumerable<ISkill> GetSkills();
    }
}
