namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// Unity 에디터 환경에서의 애플리케이션 구현 클래스.
    /// 종료 시 에디터의 플레이 모드를 중지한다.
    /// </summary>
    public class EditorApplication : IApplication
    {
        /// <summary>
        /// 애플리케이션을 종료한다. Unity 에디터의 플레이 모드를 중지시킨다.
        /// </summary>
        public void Shutdown()
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
