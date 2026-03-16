using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 폭발 효과를 표현하는 뷰 인터페이스
    /// </summary>
    public interface IExplosion : IDestroyable
    {
        /// <summary>
        /// 폭발 범위 내의 복수 오브젝트에 충돌이 발생했을 때 호출되는 이벤트
        /// </summary>
        event EventHandler<BatchObjectHitEventArgs> BatchObjectHitEvent;

        /// <summary>
        /// 폭발 연출이 완료되었을 때 호출되는 이벤트
        /// </summary>
        event EventHandler ExplosionFinishedEvent;

        /// <summary>
        /// 지정한 반경으로 폭발을 시작한다.
        /// </summary>
        /// <param name="explosionRadius">폭발 반경</param>
        void Start(float explosionRadius);

        /// <summary>
        /// 폭발의 위치를 설정한다.
        /// </summary>
        /// <param name="vector2">설정할 2D 위치 좌표</param>
        void SetPosition(Vector2 vector2);
    }
}
