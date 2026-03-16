using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// CSV 파일을 비동기적으로 로드하고 파싱하여 행(row) 데이터 리스트를 반환하는 클래스.
    /// AssetLoader를 통해 TextAsset을 로드한 뒤 CsvReader로 파싱한다.
    /// </summary>
    public class CsvLoader
    {
        /// <summary>CSV 텍스트 파싱을 담당하는 리더</summary>
        private readonly CsvReader _csvReader;
        /// <summary>에셋 파일을 비동기 로드하는 로더</summary>
        private readonly AssetLoader _assetLoader;

        /// <summary>
        /// CsvLoader의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="csvReader">CSV 텍스트를 파싱할 리더 인스턴스</param>
        /// <param name="assetLoader">에셋을 비동기적으로 로드할 로더 인스턴스</param>
        public CsvLoader(CsvReader csvReader, AssetLoader assetLoader)
        {
            _csvReader = csvReader;
            _assetLoader = assetLoader;
        }

        /// <summary>
        /// 지정된 경로의 CSV 파일을 로드하여 파싱된 행 데이터 리스트를 반환한다.
        /// </summary>
        /// <param name="path">로드할 CSV 파일의 Addressable 경로</param>
        /// <returns>각 행이 헤더-값 딕셔너리인 리스트</returns>
        public async Task<List<Dictionary<string, string>>> LoadCsvByPath(string path)
        {
            var textAsset = await _assetLoader.LoadAssetByPath<TextAsset>(path);
            var rows = _csvReader.ReadCsvData(textAsset.text);
            return rows;
        }

        /// <summary>
        /// 에셋 ID를 기반으로 CSV 파일을 로드하여 파싱된 행 데이터 리스트를 반환한다.
        /// </summary>
        /// <param name="assetID">로드할 CSV 에셋의 식별자</param>
        /// <returns>각 행이 헤더-값 딕셔너리인 리스트</returns>
        public async Task<List<Dictionary<string, string>>> LoadCsv(string assetID)
        {
            var textAsset = await _assetLoader.LoadAsset<TextAsset>(assetID);
            var rows = _csvReader.ReadCsvData(textAsset.text);
            return rows;
        }
    }
}
