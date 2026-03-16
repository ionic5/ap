using System;
using TaskForce.AP.Client.Core;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// Unity 환경에서 시간 관련 기능을 제공하는 ITime 인터페이스 구현 클래스.
    /// 현재 시간, 델타 타임, 오늘 날짜 등을 Unix 타임스탬프 형식으로 제공한다.
    /// </summary>
    public class Time : ITime
    {
        /// <summary>
        /// 현재 UTC 시간을 밀리초 단위의 Unix 타임스탬프로 반환한다.
        /// </summary>
        /// <returns>현재 시간의 밀리초 단위 Unix 타임스탬프</returns>
        public long GetCurrentTimeMilliseconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 현재 UTC 시간을 초 단위의 Unix 타임스탬프로 반환한다.
        /// </summary>
        /// <returns>현재 시간의 초 단위 Unix 타임스탬프</returns>
        public long GetCurrentTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Unity의 프레임 간 경과 시간(델타 타임)을 반환한다.
        /// </summary>
        /// <returns>이전 프레임으로부터의 경과 시간(초)</returns>
        public float GetDeltaTime()
        {
            return UnityEngine.Time.deltaTime;
        }

        /// <summary>
        /// 오늘 UTC 자정 시각을 초 단위의 Unix 타임스탬프로 반환한다.
        /// </summary>
        /// <returns>오늘 자정의 초 단위 Unix 타임스탬프</returns>
        public long GetToday()
        {
            DateTime todayUtc = DateTime.UtcNow.Date;
            return new DateTimeOffset(todayUtc).ToUnixTimeSeconds();
        }
    }
}
