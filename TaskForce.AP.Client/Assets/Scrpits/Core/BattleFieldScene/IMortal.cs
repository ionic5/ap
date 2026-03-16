using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 사망 가능한 객체를 나타내는 인터페이스.
    /// 사망 이벤트 발생과 사망 상태 확인 기능을 제공한다.
    /// </summary>
    public interface IMortal : IPositionable
    {
        /// <summary>사망 시 발생하는 이벤트</summary>
        event EventHandler<DiedEventArgs> DiedEvent;

        /// <summary>
        /// 사망 상태인지 확인한다.
        /// </summary>
        /// <returns>사망했으면 true</returns>
        bool IsDead();
    }
}
