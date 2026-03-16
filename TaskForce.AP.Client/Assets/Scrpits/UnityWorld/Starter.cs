using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 애플리케이션의 진입점 역할을 하는 MonoBehaviour 컴포넌트.
    /// 핵심 시스템 초기화, 데이터 로딩, 첫 씬 로드를 수행한다.
    /// </summary>
    public class Starter : MonoBehaviour
    {
        /// <summary>화면 전환 관리 컴포넌트</summary>
        [SerializeField]
        private Screen _screen;
        /// <summary>로딩 블라인드 게임오브젝트</summary>
        [SerializeField]
        private GameObject _loadingBlind;

        private void Start()
        {
            DontDestroyOnLoad(_screen.gameObject);
            DontDestroyOnLoad(_loadingBlind.gameObject);

            StartAsync();
        }

        /// <summary>
        /// 핵심 시스템을 초기화하고, 에셋 경로/게임 데이터/텍스트를 로드한 뒤
        /// 전투 씬을 로드하는 비동기 시작 메서드.
        /// </summary>
        private async void StartAsync()
        {
            var application = new EditorApplication();
            var logger = new DebugLogger(application);
            var time = new Time();
            var random = new Core.Random();
            var assetPathStore = new AssetPathStore(logger);
            var gameDataStore = new GameDataStore();
            var textStore = new TextStore(logger);

            var csvReader = new CsvReader(logger);
            var assetLoader = new AssetLoader(assetPathStore, logger);
            var csvLoader = new CsvLoader(csvReader, assetLoader);
            var assetPathLoader = new AssetPathLoader(csvLoader);
            var gameDataLoader = new GameDataLoader(csvLoader);
            var textStoreLoader = new TextStoreLoader(csvLoader);

            await assetPathLoader.Load(assetPathStore);
            await gameDataLoader.Load(gameDataStore);
            await textStoreLoader.Load("ko", textStore);

            _screen.AssetLoader = assetLoader;
            _screen.Logger = logger;

            Destroy(gameObject);

            var loader = new BattleFieldSceneLoader(_screen, gameDataStore, random, time, textStore, assetLoader, logger);
            loader.Load();
        }
    }
}
