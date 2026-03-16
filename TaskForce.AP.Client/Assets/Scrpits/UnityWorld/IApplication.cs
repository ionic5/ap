namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 애플리케이션의 생명주기를 관리하는 인터페이스.
    /// 플랫폼별 종료 동작을 추상화한다.
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// 애플리케이션을 종료한다.
        /// </summary>
        void Shutdown();
    }
}
