using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 전투 필드의 월드 환경 정보를 제공하는 뷰 인터페이스
    /// </summary>
    public interface IWorld
    {
        /// <summary>
        /// 플레이어 유닛의 스폰 위치를 반환한다.
        /// </summary>
        /// <returns>플레이어 유닛 스폰 지점의 2D 좌표</returns>
        Vector2 GetPlayerUnitSpawnPosition();

        /// <summary>
        /// 워프 포인트의 위치를 반환한다.
        /// </summary>
        /// <returns>워프 지점의 2D 좌표</returns>
        Vector2 GetWarpPoint();

        /// <summary>
        /// 지정한 위치가 카메라 뷰 밖에 있는지 여부를 판별한다.
        /// </summary>
        /// <param name="vector2">판별할 2D 위치 좌표</param>
        /// <returns>카메라 뷰 밖이면 true, 아니면 false</returns>
        bool IsOutOfCameraView(Vector2 vector2);
    }
}
