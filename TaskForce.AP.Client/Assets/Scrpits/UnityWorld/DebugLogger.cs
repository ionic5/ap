using System.Runtime.CompilerServices;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// Unity의 Debug 로그 시스템을 사용하여 메시지를 출력하는 로거 구현 클래스.
    /// 치명적 오류 발생 시 애플리케이션을 종료한다.
    /// 호출 위치의 파일명과 줄 번호를 자동으로 포함한다.
    /// </summary>
    public class DebugLogger : Core.ILogger
    {
        /// <summary>치명적 오류 시 종료를 위한 애플리케이션 인스턴스</summary>
        private readonly IApplication _application;

        /// <summary>
        /// DebugLogger의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="application">치명적 오류 시 종료를 수행할 애플리케이션 인스턴스</param>
        public DebugLogger(IApplication application)
        {
            _application = application;
        }

        /// <summary>
        /// 로그 메시지에 파일명과 줄 번호를 포맷팅하여 반환한다.
        /// </summary>
        /// <param name="msg">원본 로그 메시지</param>
        /// <param name="filePath">호출 소스 파일 경로</param>
        /// <param name="lineNumber">호출 줄 번호</param>
        /// <returns>포맷팅된 로그 메시지</returns>
        private string FormatLogMessage(string msg, string filePath, int lineNumber)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            return $"[{fileName}:{lineNumber}] {msg}";
        }

        /// <summary>
        /// 정보 수준의 로그 메시지를 출력한다.
        /// </summary>
        /// <param name="msg">출력할 메시지</param>
        /// <param name="filePath">호출 소스 파일 경로 (자동 입력)</param>
        /// <param name="lineNumber">호출 줄 번호 (자동 입력)</param>
        public void Info(
            string msg,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.Log(msg);
        }

        /// <summary>
        /// 경고 수준의 로그 메시지를 파일명과 줄 번호와 함께 출력한다.
        /// </summary>
        /// <param name="msg">출력할 경고 메시지</param>
        /// <param name="filePath">호출 소스 파일 경로 (자동 입력)</param>
        /// <param name="lineNumber">호출 줄 번호 (자동 입력)</param>
        public void Warn(
            string msg,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogWarning(FormatLogMessage(msg, filePath, lineNumber));
        }

        /// <summary>
        /// 치명적 오류 메시지를 출력하고 애플리케이션을 종료한다.
        /// </summary>
        /// <param name="msg">출력할 치명적 오류 메시지</param>
        /// <param name="filePath">호출 소스 파일 경로 (자동 입력)</param>
        /// <param name="lineNumber">호출 줄 번호 (자동 입력)</param>
        public void Fatal(
            string msg,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            string formattedMsg = FormatLogMessage(msg, filePath, lineNumber);

            Debug.LogError($"[FATAL ERROR] {formattedMsg}");

            _application.Shutdown();
        }
    }
}
