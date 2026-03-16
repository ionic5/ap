namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 대상의 적/아군 관계를 식별하는 인터페이스.
    /// 진영 정보를 기반으로 적과 아군을 판별한다.
    /// </summary>
    public interface ITargetIdentifier
    {
        /// <summary>
        /// 대상이 적인지 판별한다.
        /// </summary>
        /// <param name="target">판별할 대상</param>
        /// <returns>적이면 true</returns>
        bool IsEnemy(ITarget target);

        /// <summary>
        /// 대상이 아군인지 판별한다.
        /// </summary>
        /// <param name="target">판별할 대상</param>
        /// <returns>아군이면 true</returns>
        bool IsAlly(ITarget target);
    }
}
