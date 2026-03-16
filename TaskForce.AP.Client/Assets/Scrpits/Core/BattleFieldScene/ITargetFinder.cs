using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 다양한 조건으로 대상(ITarget)을 검색하는 인터페이스.
    /// 뷰 ID, 원형 범위, 부채꼴 범위 등의 방식으로 대상을 찾을 수 있다.
    /// </summary>
    public interface ITargetFinder
    {
        /// <summary>
        /// 뷰 ID로 대상을 검색한다.
        /// </summary>
        /// <param name="objectID">뷰 객체 ID</param>
        /// <returns>해당 ID의 대상</returns>
        ITarget FindByViewID(string objectID);

        /// <summary>
        /// 여러 뷰 ID 중 조건에 맞는 대상들을 검색한다.
        /// </summary>
        /// <param name="objectIDs">검색할 뷰 ID 목록</param>
        /// <param name="predicate">필터 조건</param>
        /// <returns>조건에 맞는 대상 목록</returns>
        IEnumerable<ITarget> FindByViewIDs(IEnumerable<string> objectIDs, Func<ITarget, bool> predicate);

        /// <summary>
        /// 도넛형(최소 반경~최대 반경) 범위 내에서 조건에 맞는 대상들을 검색한다.
        /// </summary>
        /// <param name="center">검색 중심 좌표</param>
        /// <param name="minRadius">최소 반경</param>
        /// <param name="maxRadius">최대 반경</param>
        /// <param name="predicate">필터 조건</param>
        /// <returns>조건에 맞는 대상 목록</returns>
        IEnumerable<ITarget> FindInRadius(System.Numerics.Vector2 center, float minRadius, float maxRadius, Func<ITarget, bool> predicate);

        /// <summary>
        /// 원형 범위 내에서 조건에 맞는 대상들을 검색한다.
        /// </summary>
        /// <param name="center">검색 중심 좌표</param>
        /// <param name="radius">검색 반경</param>
        /// <param name="predicate">필터 조건</param>
        /// <returns>조건에 맞는 대상 목록</returns>
        IEnumerable<ITarget> FindInRadius(System.Numerics.Vector2 center, float radius, Func<ITarget, bool> predicate);

        /// <summary>
        /// 부채꼴 범위 내에서 조건에 맞는 대상들을 검색한다.
        /// </summary>
        /// <param name="center">검색 중심 좌표</param>
        /// <param name="direction">부채꼴 중심 방향 벡터</param>
        /// <param name="degree">부채꼴 각도 (도 단위)</param>
        /// <param name="radius">부채꼴 반경</param>
        /// <param name="predicate">필터 조건</param>
        /// <returns>조건에 맞는 대상 목록</returns>
        IEnumerable<ITarget> FindInSector(System.Numerics.Vector2 center, System.Numerics.Vector2 direction, float degree, float radius, Func<ITarget, bool> predicate);
    }
}
