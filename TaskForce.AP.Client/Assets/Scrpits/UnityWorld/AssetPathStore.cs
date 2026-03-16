using System.Collections.Generic;
using TaskForce.AP.Client.Core;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 에셋 ID와 Addressable 경로의 매핑, 그리고 언어별 폰트 경로를 저장하고 조회하는 저장소 클래스.
    /// </summary>
    public class AssetPathStore
    {
        /// <summary>에셋 ID를 키로, Addressable 경로를 값으로 저장하는 딕셔너리</summary>
        private readonly Dictionary<string, string> _assetPaths;
        /// <summary>언어 코드를 키로, 폰트 경로를 값으로 저장하는 딕셔너리</summary>
        private readonly Dictionary<string, string> _fontPaths;
        /// <summary>로거 인스턴스</summary>
        private readonly ILogger _logger;

        /// <summary>
        /// AssetPathStore의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="logger">오류 로깅을 위한 로거 인스턴스</param>
        public AssetPathStore(ILogger logger)
        {
            _assetPaths = new Dictionary<string, string>();
            _fontPaths = new Dictionary<string, string>();
            _logger = logger;
        }

        /// <summary>
        /// 에셋 ID에 해당하는 Addressable 경로를 반환한다.
        /// 존재하지 않으면 치명적 오류를 로깅하고 빈 문자열을 반환한다.
        /// </summary>
        /// <param name="assetID">조회할 에셋의 식별자</param>
        /// <returns>에셋의 Addressable 경로, 실패 시 빈 문자열</returns>
        public string GetAssetPath(string assetID)
        {
            if (string.IsNullOrEmpty(assetID))
                _logger.Fatal($"Empty asset id used.");

            if (_assetPaths.ContainsKey(assetID))
                return _assetPaths[assetID];

            _logger.Fatal($"Failed to find asset path for {assetID}");
            return string.Empty;
        }

        /// <summary>
        /// 언어 코드에 해당하는 폰트 경로를 반환한다.
        /// </summary>
        /// <param name="languageCode">조회할 언어 코드</param>
        /// <returns>폰트 경로, 존재하지 않으면 빈 문자열</returns>
        public string GetAssetPathByLanguageCode(string languageCode)
        {
            if (_fontPaths.ContainsKey(languageCode))
                return _fontPaths[languageCode];

            return string.Empty;
        }

        /// <summary>
        /// 에셋 ID와 경로 매핑을 저장소에 추가한다.
        /// </summary>
        /// <param name="assetID">에셋 식별자</param>
        /// <param name="path">에셋의 Addressable 경로</param>
        public void AddAssetPath(string assetID, string path)
        {
            _assetPaths.Add(assetID, path);
        }

        /// <summary>
        /// 언어 코드와 폰트 경로 매핑을 저장소에 추가한다.
        /// </summary>
        /// <param name="languageCode">언어 코드</param>
        /// <param name="path">폰트 에셋의 경로</param>
        public void AddFontPath(string languageCode, string path)
        {
            _fontPaths.Add(languageCode, path);
        }

        /// <summary>
        /// 저장된 모든 에셋 경로와 폰트 경로를 초기화한다.
        /// </summary>
        public void Clear()
        {
            _assetPaths.Clear();
            _fontPaths.Clear();
        }
    }
}
