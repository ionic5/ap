using System;
using System.Collections.Generic;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 전투 필드의 유닛을 나타내는 핵심 인터페이스.
    /// 속성 조회, 경험치/레벨 관리, 스킬 사용, 대상 검색, 이동/공격 동작 등을 제공한다.
    /// </summary>
    public interface IUnit : IDestroyable, IMortal, IPositionable, IFollowable
    {
        /// <summary>경험치 변경 시 발생하는 이벤트</summary>
        event EventHandler ExpChangedEvent;
        /// <summary>필요 경험치 변경 시 발생하는 이벤트</summary>
        event EventHandler RequireExpChangedEvent;
        /// <summary>레벨업 시 발생하는 이벤트</summary>
        event EventHandler LevelUpEvent;

        /// <summary>
        /// 지정된 ID의 속성 값을 가져온다.
        /// </summary>
        /// <param name="attributeID">속성 ID</param>
        /// <returns>속성 값</returns>
        Attribute GetAttribute(string attributeID);

        /// <summary>
        /// 현재 경험치를 가져온다.
        /// </summary>
        /// <returns>현재 경험치</returns>
        int GetExp();

        /// <summary>
        /// 레벨업에 필요한 경험치를 가져온다.
        /// </summary>
        /// <returns>필요 경험치</returns>
        int GetRequireExp();

        /// <summary>
        /// 현재 레벨을 가져온다.
        /// </summary>
        /// <returns>현재 레벨</returns>
        int GetLevel();

        /// <summary>
        /// 현재 바라보는 방향 벡터를 가져온다.
        /// </summary>
        /// <returns>방향 벡터</returns>
        Vector2 GetDirection();

        /// <summary>
        /// 기본 스킬을 가져온다.
        /// </summary>
        /// <returns>기본 스킬. 없으면 null</returns>
        Skills.ISkill GetDefaultSkill();

        /// <summary>
        /// 대상이 적인지 판별한다.
        /// </summary>
        /// <param name="target">판별할 대상</param>
        /// <returns>적이면 true</returns>
        bool IsEnemy(ITarget target);

        /// <summary>
        /// 지정된 범위 내 아군을 검색한다.
        /// </summary>
        /// <param name="range">검색 범위</param>
        /// <returns>범위 내 아군 목록</returns>
        IEnumerable<ITarget> FindAllies(float range);

        /// <summary>
        /// 지정된 범위 내 적을 검색한다.
        /// </summary>
        /// <param name="range">검색 범위</param>
        /// <returns>범위 내 적 목록</returns>
        IEnumerable<ITarget> FindTargets(float range);

        /// <summary>
        /// 최소~최대 범위 내 적을 검색한다.
        /// </summary>
        /// <param name="minRange">최소 범위</param>
        /// <param name="maxRange">최대 범위</param>
        /// <returns>범위 내 적 목록</returns>
        IEnumerable<ITarget> FindTargets(float minRange, float maxRange);

        /// <summary>
        /// 지정된 중심점과 범위 내 적을 검색한다.
        /// </summary>
        /// <param name="center">검색 중심 좌표</param>
        /// <param name="range">검색 범위</param>
        /// <returns>범위 내 적 목록</returns>
        IEnumerable<ITarget> FindTargets(Vector2 center, float range);

        /// <summary>
        /// 부채꼴 범위 내 적을 검색한다.
        /// </summary>
        /// <param name="center">검색 중심 좌표</param>
        /// <param name="direction">부채꼴 중심 방향</param>
        /// <param name="angle">부채꼴 각도</param>
        /// <param name="range">부채꼴 반경</param>
        /// <returns>범위 내 적 목록</returns>
        IEnumerable<ITarget> FindTargetsInSector(Vector2 center, Vector2 direction, float angle, float range);

        /// <summary>
        /// 뷰 ID 목록에 해당하는 적을 검색한다.
        /// </summary>
        /// <param name="objectIDs">뷰 ID 목록</param>
        /// <returns>해당하는 적 목록</returns>
        IEnumerable<ITarget> FindTargets(IEnumerable<string> objectIDs);

        /// <summary>
        /// 시전 모션을 재생한다.
        /// </summary>
        /// <param name="castTime">시전 시간</param>
        void Cast(float castTime);

        /// <summary>
        /// 공격 모션을 재생한다.
        /// </summary>
        /// <param name="direction">공격 방향</param>
        /// <param name="attackTime">공격 시간</param>
        void Attack(Vector2 direction, float attackTime);

        /// <summary>
        /// 대기 상태로 전환한다.
        /// </summary>
        void Wait();

        /// <summary>
        /// 지정된 ID의 유닛 로직을 설정한다.
        /// </summary>
        /// <param name="logicID">유닛 로직 ID</param>
        void SetUnitLogic(string logicID);

        /// <summary>
        /// 마스터(소환자) 유닛을 설정한다.
        /// </summary>
        /// <param name="summoner">마스터 유닛</param>
        void SetMaster(IUnit summoner);
    }
}
