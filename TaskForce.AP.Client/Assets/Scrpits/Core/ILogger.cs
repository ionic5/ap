using System.Runtime.CompilerServices;

namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 로그 메시지를 출력하기 위한 인터페이스.
    /// 정보, 경고, 치명적 오류 수준의 로그를 지원한다.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 정보 수준의 로그 메시지를 출력한다.
        /// </summary>
        /// <param name="msg">출력할 로그 메시지.</param>
        /// <param name="filePath">호출 위치의 소스 파일 경로 (자동 입력).</param>
        /// <param name="lineNumber">호출 위치의 소스 줄 번호 (자동 입력).</param>
        void Info(string msg, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);

        /// <summary>
        /// 경고 수준의 로그 메시지를 출력한다.
        /// </summary>
        /// <param name="msg">출력할 로그 메시지.</param>
        /// <param name="filePath">호출 위치의 소스 파일 경로 (자동 입력).</param>
        /// <param name="lineNumber">호출 위치의 소스 줄 번호 (자동 입력).</param>
        void Warn(string msg, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);

        /// <summary>
        /// 치명적 오류 수준의 로그 메시지를 출력한다.
        /// </summary>
        /// <param name="msg">출력할 로그 메시지.</param>
        /// <param name="filePath">호출 위치의 소스 파일 경로 (자동 입력).</param>
        /// <param name="lineNumber">호출 위치의 소스 줄 번호 (자동 입력).</param>
        void Fatal(string msg, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);
    }
}
