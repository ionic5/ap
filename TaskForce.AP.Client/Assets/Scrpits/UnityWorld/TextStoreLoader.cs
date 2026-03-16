using System.Linq;
using System.Threading.Tasks;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.UnityWorld.AssetData;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// CSV 에셋에서 다국어 텍스트 데이터를 로드하여 TextStore에 저장하는 클래스.
    /// 지정된 언어 코드에 해당하는 텍스트만 추출하여 저장한다.
    /// </summary>
    public class TextStoreLoader
    {
        /// <summary>CSV 데이터를 로드하기 위한 로더</summary>
        private readonly CsvLoader _csvLoader;

        /// <summary>
        /// TextStoreLoader의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="csvLoader">CSV 파일을 로드할 로더 인스턴스</param>
        public TextStoreLoader(CsvLoader csvLoader)
        {
            _csvLoader = csvLoader;
        }

        /// <summary>
        /// 텍스트 CSV 데이터를 로드하여 지정된 언어 코드의 텍스트를 TextStore에 추가한다.
        /// </summary>
        /// <param name="_languageCode">로드할 텍스트의 언어 코드 (예: "ko", "en")</param>
        /// <param name="textStore">텍스트를 저장할 대상 저장소</param>
        /// <returns>비동기 작업</returns>
        public async Task Load(string _languageCode, TextStore textStore)
        {
            var rows = await _csvLoader.LoadCsv(AssetID.Text);

            if (!rows.Any()) return;

            var firstRow = rows.First();
            var headers = firstRow.Keys.ToArray();
            var langHeaders = headers.Where(h => h != "key" && h != "TextID").ToArray();

            foreach (var row in rows)
            {
                if (!row.TryGetValue("key", out var key))
                    continue;

                string value = string.Empty;
                row.TryGetValue(_languageCode, out value);

                textStore.AddText(key, value);
            }
        }
    }
}
