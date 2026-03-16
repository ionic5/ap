using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 유닛 사망 후 남는 영혼 오브젝트를 표현하는 뷰 인터페이스
    /// </summary>
    public interface ISoul : IDestroyable
    {
        /// <summary>
        /// 영혼의 현재 위치를 반환한다.
        /// </summary>
        /// <returns>영혼의 현재 2D 위치 좌표</returns>
        Vector2 GetPosition();

        /// <summary>
        /// 영혼을 지정한 추적 대상을 향해 이동시킨다.
        /// </summary>
        /// <param name="followTarget">추적할 대상 오브젝트</param>
        /// <param name="speed">이동 속도</param>
        void MoveTo(Core.BattleFieldScene.IFollowable followTarget, float speed);

        /// <summary>
        /// 영혼의 위치를 설정한다.
        /// </summary>
        /// <param name="position">설정할 2D 위치 좌표</param>
        void SetPosition(Vector2 position);

        /// <summary>
        /// 영혼의 이동을 정지시킨다.
        /// </summary>
        void Stop();
    }
}
