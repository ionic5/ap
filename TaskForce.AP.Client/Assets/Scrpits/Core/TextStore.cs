using System.Collections.Generic;

namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 텍스트 ID를 키로 사용하여 다국어 텍스트를 저장하고 조회하는 저장소 클래스.
    /// </summary>
    public class TextStore
    {
        /// <summary>로그 출력에 사용하는 로거 인스턴스.</summary>
        private readonly Core.ILogger _logger;

        /// <summary>텍스트 ID를 키, 표시 문자열을 값으로 저장하는 딕셔너리.</summary>
        private readonly Dictionary<string, string> _texts;

        /// <summary>
        /// <see cref="TextStore"/>의 새 인스턴스를 생성한다.
        /// </summary>
        /// <param name="logger">텍스트 조회 실패 시 경고를 출력할 로거.</param>
        public TextStore(Core.ILogger logger)
        {
            _logger = logger;
            _texts = new Dictionary<string, string>();
        }

        /// <summary>
        /// 지정된 텍스트 ID에 해당하는 텍스트를 반환한다.
        /// 존재하지 않는 ID인 경우 경고를 출력하고 ID 문자열 자체를 반환한다.
        /// </summary>
        /// <param name="textID">조회할 텍스트의 고유 ID.</param>
        /// <returns>텍스트 ID에 대응하는 문자열. 없으면 ID 자체를 반환.</returns>
        public string GetText(string textID)
        {
            if (_texts.TryGetValue(textID, out var value))
                return value;
            _logger.Warn($"TextID '{textID}' not found.");
            return textID;
        }

        /// <summary>
        /// 저장된 모든 텍스트 데이터를 삭제한다.
        /// </summary>
        public void Clear()
        {
            _texts.Clear();
        }

        /// <summary>
        /// 텍스트 항목을 추가하거나 기존 항목을 덮어쓴다.
        /// </summary>
        /// <param name="key">텍스트를 식별하는 고유 키.</param>
        /// <param name="value">저장할 텍스트 문자열.</param>
        public void AddText(string key, string value)
        {
            _texts[key] = value;
        }
    }
}
