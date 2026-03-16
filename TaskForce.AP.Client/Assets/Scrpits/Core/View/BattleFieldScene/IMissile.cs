using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 투사체(미사일)를 표현하는 뷰 인터페이스
    /// </summary>
    public interface IMissile : IDestroyable
    {
        /// <summary>
        /// 투사체가 목적지에 도착했을 때 호출되는 이벤트
        /// </summary>
        event EventHandler ArrivedDestinationEvent;

        /// <summary>
        /// 투사체가 오브젝트에 충돌했을 때 호출되는 이벤트
        /// </summary>
        event EventHandler<View.HitEventArgs> HitEvent;

        /// <summary>
        /// 투사체를 지정한 목적지로 이동시킨다.
        /// </summary>
        /// <param name="destination">이동 목적지 좌표</param>
        /// <param name="speed">이동 속도</param>
        void MoveTo(Vector2 destination, float speed);

        /// <summary>
        /// 투사체의 위치를 설정한다.
        /// </summary>
        /// <param name="position">설정할 2D 위치 좌표</param>
        void SetPosition(Vector2 position);

        /// <summary>
        /// 투사체의 현재 위치를 반환한다.
        /// </summary>
        /// <returns>투사체의 현재 2D 위치 좌표</returns>
        Vector2 GetPosition();
    }
}
