using System;

namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 파괴 가능한 객체가 구현해야 하는 인터페이스.
    /// 파괴 이벤트를 통해 외부에 파괴 시점을 알릴 수 있다.
    /// </summary>
    public interface IDestroyable
    {
        /// <summary>
        /// 객체가 파괴될 때 발생하는 이벤트.
        /// </summary>
        event EventHandler<DestroyEventArgs> DestroyEvent;

        /// <summary>
        /// 객체를 파괴하고 관련 리소스를 정리한다.
        /// </summary>
        void Destroy();
    }
}
