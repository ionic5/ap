using System;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 화약통(폭발물)을 표현하는 뷰 인터페이스
    /// </summary>
    public interface IPowderKeg : IPositionable, IDestroyable
    {
        /// <summary>
        /// 감시 범위 내에서 복수의 오브젝트가 감지되었을 때 호출되는 이벤트
        /// </summary>
        event EventHandler<BatchObjectDetectedEventArgs> BatchObjectDetectedEvent;

        /// <summary>
        /// 화약통이 폭발했을 때 호출되는 이벤트
        /// </summary>
        event EventHandler ExplosionEvent;

        /// <summary>
        /// 화약통에 점화하여 폭발을 시작한다.
        /// </summary>
        void Ignite();

        /// <summary>
        /// 지정한 반경으로 주변 오브젝트 감시를 시작한다.
        /// </summary>
        /// <param name="watchRadius">감시 반경</param>
        void Watch(float watchRadius);
    }
}
