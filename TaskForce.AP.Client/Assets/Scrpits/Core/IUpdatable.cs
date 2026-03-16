namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 매 프레임 갱신이 필요한 객체가 구현해야 하는 인터페이스.
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// 매 프레임 호출되어 객체의 상태를 갱신한다.
        /// </summary>
        void Update();
    }
}
