namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 유닛의 행동 로직을 정의하는 인터페이스.
    /// 유닛에 대한 제어를 시작하는 기능을 제공한다.
    /// </summary>
    public interface IUnitLogic : IDestroyable
    {
        /// <summary>
        /// 지정된 유닛에 대한 제어를 시작한다.
        /// </summary>
        /// <param name="unit">제어할 유닛</param>
        void StartControl(IControllableUnit unit);
    }
}
