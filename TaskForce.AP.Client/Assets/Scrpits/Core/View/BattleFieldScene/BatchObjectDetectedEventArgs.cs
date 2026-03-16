using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 복수의 오브젝트가 일괄 감지되었을 때 전달되는 이벤트 인자 클래스
    /// </summary>
    public class BatchObjectDetectedEventArgs
    {
        /// <summary>
        /// 감지된 오브젝트들의 고유 식별자 목록
        /// </summary>
        public readonly IEnumerable<string> ObjectIDs;

        /// <summary>
        /// <see cref="BatchObjectDetectedEventArgs"/> 클래스의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="objectIDs">감지된 오브젝트들의 고유 식별자 목록</param>
        public BatchObjectDetectedEventArgs(IEnumerable<string> objectIDs)
        {
            ObjectIDs = objectIDs;
        }
    }
}
