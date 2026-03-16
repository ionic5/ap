using System;

namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 대상 객체가 파괴될 때 발생하는 이벤트의 인자를 제공하는 클래스.
    /// </summary>
    public class DestroyEventArgs : EventArgs
    {
        /// <summary>파괴 대상이 되는 객체.</summary>
        public readonly IDestroyable TargetObject;

        /// <summary>
        /// <see cref="DestroyEventArgs"/>의 새 인스턴스를 생성한다.
        /// </summary>
        /// <param name="targetObject">파괴될 대상 객체.</param>
        public DestroyEventArgs(IDestroyable targetObject)
        {
            TargetObject = targetObject;
        }
    }
}
