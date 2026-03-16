using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 객체 생성 시 발생하는 이벤트의 인자를 담는 제네릭 클래스.
    /// 생성된 객체를 이벤트 수신자에게 전달한다.
    /// </summary>
    /// <typeparam name="T">생성된 객체의 타입</typeparam>
    public class CreatedEventArgs<T> : EventArgs
    {
        /// <summary>생성된 객체</summary>
        public readonly T CreatedObject;

        /// <summary>
        /// CreatedEventArgs의 생성자.
        /// </summary>
        /// <param name="createdObject">생성된 객체</param>
        public CreatedEventArgs(T createdObject)
        {
            CreatedObject = createdObject;
        }
    }
}
