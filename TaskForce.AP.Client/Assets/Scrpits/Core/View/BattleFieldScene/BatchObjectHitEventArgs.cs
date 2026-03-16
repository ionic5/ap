using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 복수의 오브젝트에 일괄 충돌(히트)이 발생했을 때 전달되는 이벤트 인자 클래스
    /// </summary>
    public class BatchObjectHitEventArgs : EventArgs
    {
        /// <summary>
        /// 충돌이 발생한 오브젝트들의 고유 식별자 목록
        /// </summary>
        public readonly IEnumerable<string> ObjectIDs;

        /// <summary>
        /// <see cref="BatchObjectHitEventArgs"/> 클래스의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="objectIDs">충돌이 발생한 오브젝트들의 고유 식별자 목록</param>
        public BatchObjectHitEventArgs(IEnumerable<string> objectIDs)
        {
            ObjectIDs = objectIDs;
        }
    }
}
