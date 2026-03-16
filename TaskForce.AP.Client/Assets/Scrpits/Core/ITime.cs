namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 시간 정보를 제공하는 인터페이스.
    /// 현재 시각, 델타 타임, 오늘 날짜 등의 시간 관련 값을 조회할 수 있다.
    /// </summary>
    public interface ITime
    {
        /// <summary>
        /// 현재 시각을 밀리초 단위로 반환한다.
        /// </summary>
        /// <returns>밀리초 단위의 현재 시각.</returns>
        long GetCurrentTimeMilliseconds();

        /// <summary>
        /// 현재 시각을 반환한다.
        /// </summary>
        /// <returns>현재 시각 값.</returns>
        long GetCurrentTime();

        /// <summary>
        /// 이전 프레임과 현재 프레임 사이의 경과 시간(델타 타임)을 초 단위로 반환한다.
        /// </summary>
        /// <returns>프레임 간 경과 시간(초).</returns>
        float GetDeltaTime();

        /// <summary>
        /// 오늘 날짜에 해당하는 시간 값을 반환한다.
        /// </summary>
        /// <returns>오늘 날짜를 나타내는 시간 값.</returns>
        long GetToday();
    }
}
