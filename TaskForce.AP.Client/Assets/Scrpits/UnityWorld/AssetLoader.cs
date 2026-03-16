using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// Unity Addressables 시스템을 사용하여 에셋을 비동기적으로 로드하고 관리하는 클래스.
    /// 로드된 에셋 핸들을 캐싱하여 중복 로드를 방지한다.
    /// </summary>
    public class AssetLoader
    {
        /// <summary>로드된 에셋의 비동기 핸들을 경로별로 캐싱하는 스레드 안전 딕셔너리</summary>
        private readonly ConcurrentDictionary<string, AsyncOperationHandle> _loadedHandles;
        /// <summary>에셋 경로 저장소</summary>
        private readonly AssetPathStore _assetPathStore;
        /// <summary>로거 인스턴스</summary>
        private readonly Core.ILogger _logger;

        /// <summary>
        /// AssetLoader의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="assetPathStore">에셋 ID와 경로 매핑을 제공하는 저장소</param>
        /// <param name="logger">로깅을 위한 로거 인스턴스</param>
        public AssetLoader(AssetPathStore assetPathStore, Core.ILogger logger)
        {
            _assetPathStore = assetPathStore;
            _logger = logger;
            _loadedHandles = new ConcurrentDictionary<string, AsyncOperationHandle>();
        }

        /// <summary>
        /// 에셋 ID를 기반으로 에셋을 비동기적으로 로드한다.
        /// </summary>
        /// <typeparam name="T">로드할 에셋의 Unity Object 타입</typeparam>
        /// <param name="assetID">로드할 에셋의 식별자</param>
        /// <returns>로드된 에셋 인스턴스, 실패 시 null</returns>
        public async Task<T> LoadAsset<T>(string assetID) where T : UnityEngine.Object
        {
            return await LoadAssetByPath<T>(_assetPathStore.GetAssetPath(assetID));
        }

        /// <summary>
        /// 취소 토큰을 지원하여 에셋 ID를 기반으로 에셋을 비동기적으로 로드한다.
        /// </summary>
        /// <typeparam name="T">로드할 에셋의 Unity Object 타입</typeparam>
        /// <param name="assetID">로드할 에셋의 식별자</param>
        /// <param name="token">비동기 작업 취소를 위한 토큰</param>
        /// <returns>로드된 에셋 인스턴스, 실패 시 null</returns>
        public async Task<T> LoadAsset<T>(string assetID, System.Threading.CancellationToken token) where T : UnityEngine.Object
        {
            return await LoadAssetByPath<T>(_assetPathStore.GetAssetPath(assetID), token);
        }

        /// <summary>
        /// 언어 코드에 해당하는 폰트 에셋을 비동기적으로 로드한다.
        /// </summary>
        /// <param name="languageCode">로드할 폰트의 언어 코드</param>
        /// <returns>로드된 폰트 인스턴스</returns>
        public async Task<Font> LoadFont(string languageCode)
        {
            return await LoadAssetByPath<Font>(_assetPathStore.GetAssetPathByLanguageCode(languageCode));
        }

        /// <summary>
        /// 경로를 기반으로 에셋을 비동기적으로 로드한다.
        /// </summary>
        /// <typeparam name="T">로드할 에셋의 Unity Object 타입</typeparam>
        /// <param name="path">에셋의 Addressable 경로</param>
        /// <returns>로드된 에셋 인스턴스, 실패 시 null</returns>
        public async Task<T> LoadAssetByPath<T>(string path) where T : UnityEngine.Object
        {
            return await LoadAssetByPath<T>(path, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 취소 토큰을 지원하여 경로 기반으로 에셋을 비동기적으로 로드한다.
        /// 이미 로드된 에셋은 캐시에서 반환하며, 로드 실패 시 핸들을 정리한다.
        /// </summary>
        /// <typeparam name="T">로드할 에셋의 Unity Object 타입</typeparam>
        /// <param name="path">에셋의 Addressable 경로</param>
        /// <param name="token">비동기 작업 취소를 위한 토큰</param>
        /// <returns>로드된 에셋 인스턴스, 실패 시 null</returns>
        public async Task<T> LoadAssetByPath<T>(string path, System.Threading.CancellationToken token) where T : UnityEngine.Object
        {
            AsyncOperationHandle handle = _loadedHandles.GetOrAdd(path, (key) =>
            {
                return Addressables.LoadAssetAsync<T>(key);
            });

            try
            {
                if (!handle.IsDone)
                {
                    if (token.CanBeCanceled)
                    {
                        var completed = await Task.WhenAny(handle.Task, Task.Delay(-1, token));
                        if (completed != handle.Task)
                        {
                            TryRemoveHandle(path);
                            throw new System.OperationCanceledException(token);
                        }
                    }
                    else
                    {
                        await handle.Task;
                    }
                }

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result as T;
                }
                else
                {
                    TryRemoveHandle(path);
                    _logger.Fatal($"Asset Load Failed ({typeof(T).Name}): {path}");
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                TryRemoveHandle(path);
                _logger.Fatal($"Error during Asset Load ({path}): {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 지정된 경로의 에셋 핸들을 캐시에서 제거하고 Addressables 리소스를 해제한다.
        /// </summary>
        /// <param name="path">제거할 에셋의 경로</param>
        private void TryRemoveHandle(string path)
        {
            if (_loadedHandles.TryRemove(path, out AsyncOperationHandle handle))
                Addressables.Release(handle);
        }

        /// <summary>
        /// 로드된 모든 에셋 핸들을 해제하고 캐시를 초기화한다.
        /// </summary>
        public void ClearAllAssets()
        {
            foreach (var pair in _loadedHandles)
                if (pair.Value.IsValid())
                    Addressables.Release(pair.Value);

            _loadedHandles.Clear();
            _logger.Info("All loaded asset data released.");
        }
    }
}
