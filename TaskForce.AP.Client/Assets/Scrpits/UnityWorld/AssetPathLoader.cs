using System.Threading.Tasks;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// CSV 파일로부터 에셋 경로 매핑 데이터를 로드하여 AssetPathStore에 저장하는 클래스.
    /// </summary>
    public class AssetPathLoader
    {
        /// <summary>CSV 데이터를 로드하기 위한 로더</summary>
        private readonly CsvLoader _csvLoader;

        /// <summary>
        /// AssetPathLoader의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="csvLoader">CSV 파일을 로드할 로더 인스턴스</param>
        public AssetPathLoader(CsvLoader csvLoader)
        {
            _csvLoader = csvLoader;
        }

        /// <summary>
        /// 에셋 경로 데이터를 로드하여 AssetPathStore에 추가한다.
        /// </summary>
        /// <param name="assetPathStore">에셋 경로를 저장할 대상 저장소</param>
        /// <returns>비동기 작업</returns>
        public async Task Load(AssetPathStore assetPathStore)
        {
            await LoadAssetPath(assetPathStore);
        }

        /// <summary>
        /// Asset.csv 파일을 읽어 에셋 ID와 경로 쌍을 AssetPathStore에 등록한다.
        /// </summary>
        /// <param name="assetPathStore">에셋 경로를 저장할 대상 저장소</param>
        /// <returns>비동기 작업</returns>
        private async Task LoadAssetPath(AssetPathStore assetPathStore)
        {
            var rows = await _csvLoader.LoadCsvByPath("Assets/Addressables/AssetData/Asset.csv");
            foreach (var row in rows)
                assetPathStore.AddAssetPath(row["assetID"], row["path"]);
        }
    }
}
