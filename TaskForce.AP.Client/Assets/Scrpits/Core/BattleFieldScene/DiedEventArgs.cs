using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 유닛 사망 시 발생하는 이벤트의 인자를 담는 클래스.
    /// 사망한 대상 정보를 이벤트 수신자에게 전달한다.
    /// </summary>
    public class DiedEventArgs : EventArgs
    {
        /// <summary>사망한 대상</summary>
        public readonly IMortal DiedTarget;

        /// <summary>
        /// DiedEventArgs의 생성자.
        /// </summary>
        /// <param name="diedTarget">사망한 대상</param>
        public DiedEventArgs(IMortal diedTarget)
        {
            DiedTarget = diedTarget;
        }
    }
}
