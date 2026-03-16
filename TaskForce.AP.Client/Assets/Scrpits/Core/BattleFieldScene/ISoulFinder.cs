using System.Collections.Generic;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 범위 내 소울(Soul) 검색 기능을 제공하는 인터페이스.
    /// </summary>
    public interface ISoulFinder
    {
        /// <summary>
        /// 지정된 중심점과 반경 내의 소울을 검색하여 결과 리스트에 담는다.
        /// </summary>
        /// <param name="center">검색 중심 좌표</param>
        /// <param name="radius">검색 반경</param>
        /// <param name="results">검색 결과를 저장할 리스트</param>
        /// <returns>발견된 소울의 수</returns>
        int FindRadius(Vector2 center, float radius, List<Soul> results);
    }
}
