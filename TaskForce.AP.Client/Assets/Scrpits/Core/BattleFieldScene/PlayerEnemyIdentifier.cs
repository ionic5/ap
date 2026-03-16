namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 플레이어 진영의 적/아군 판별 클래스.
    /// 비플레이어 진영을 적으로, 플레이어 진영을 아군으로 판별한다.
    /// </summary>
    public class PlayerEnemyIdentifier : ITargetIdentifier
    {
        /// <summary>
        /// 대상이 아군인지 판별한다.
        /// </summary>
        /// <param name="target">판별할 대상</param>
        /// <returns>플레이어 진영이면 true</returns>
        public bool IsAlly(ITarget target)
        {
            return target.IsPlayerSide();
        }

        /// <summary>
        /// 대상이 적인지 판별한다.
        /// </summary>
        /// <param name="target">판별할 대상</param>
        /// <returns>비플레이어 진영이면 true</returns>
        public bool IsEnemy(ITarget target)
        {
            return !target.IsPlayerSide();
        }
    }
}
