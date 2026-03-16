using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 씬 전환, 로딩 블라인드 표시/숨김, 리소스 정리 등 화면 관리를 담당하는 MonoBehaviour 컴포넌트.
    /// 씬 로드와 언로드, 에셋 해제를 조율한다.
    /// </summary>
    public class Screen : MonoBehaviour
    {
        /// <summary>로딩 중 화면을 가리는 블라인드 캔버스</summary>
        [SerializeField]
        private Canvas _loadingBlind;

        /// <summary>로거 인스턴스</summary>
        public Core.ILogger Logger;
        /// <summary>에셋 로더 인스턴스</summary>
        public AssetLoader AssetLoader;

        /// <summary>로딩 블라인드 표시 완료를 알리는 TaskCompletionSource</summary>
        private TaskCompletionSource<bool> _loadingTcs = null;
        /// <summary>로딩 블라인드의 현재 표시 상태</summary>
        private volatile bool _isShowing = false;

        /// <summary>
        /// 로딩 블라인드를 화면에 표시한다.
        /// 이미 표시 중이면 표시 완료될 때까지 대기한다.
        /// </summary>
        /// <returns>비동기 작업</returns>
        public async Task ShowLoadingBlind()
        {
            if (_isShowing)
            {
                if (_loadingTcs != null)
                {
                    await _loadingTcs.Task;
                    return;
                }
            }

            _loadingTcs = new TaskCompletionSource<bool>();
            _isShowing = true;
            _loadingBlind.gameObject.SetActive(true);
        }

        /// <summary>
        /// 로딩 블라인드를 숨기고 대기 중인 작업을 완료시킨다.
        /// </summary>
        public void HideLoadingBlind()
        {
            if (!_isShowing) return;

            _loadingBlind.gameObject.SetActive(false);
            _isShowing = false;

            var tcs = _loadingTcs;
            _loadingTcs = null;
            tcs?.TrySetResult(true);
        }

        /// <summary>
        /// 이전 씬을 파괴하고 빈 씬으로 교체한 뒤 모든 리소스를 정리한다.
        /// </summary>
        /// <returns>비동기 작업</returns>
        public async Task DestroyLastScene()
        {
            // Unity는 활성화된 씬이 없는 상태를 허용하지 않으므로, 
            // 기존 씬을 완전히 언로드하기 위해 '빈 씬(EmptyScene)'을 로드하여 대체합니다.
            if (!await TryLoadSceneAsync(SceneID.EmptyScene))
                return;

            await ClearAllResources();
        }

        /// <summary>
        /// 로드된 에셋을 모두 해제하고 GC를 수행하여 미사용 리소스를 정리한다.
        /// </summary>
        /// <returns>비동기 작업</returns>
        private async Task ClearAllResources()
        {
            AssetLoader.ClearAllAssets();

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            var unloadOp = Resources.UnloadUnusedAssets();
            while (!unloadOp.isDone)
                await Task.Yield();
        }

        /// <summary>
        /// 지정된 씬을 비동기적으로 로드한다.
        /// </summary>
        /// <param name="sceneID">로드할 씬의 식별자</param>
        /// <returns>로드 성공 시 true, 실패 시 false</returns>
        private async Task<bool> TryLoadSceneAsync(string sceneID)
        {
            var op = SceneManager.LoadSceneAsync(sceneID, LoadSceneMode.Single);
            if (op == null)
            {
                Logger.Fatal($"Failed to find scene. Scene id : {sceneID}");
                return false;
            }

            while (!op.isDone)
                await Task.Yield();

            return true;
        }

        /// <summary>
        /// 새 씬을 로드하여 활성 씬으로 설정하고, 씬 내의 Scene 컴포넌트가 부착된 루트 오브젝트를 반환한다.
        /// </summary>
        /// <param name="sceneID">로드할 씬의 식별자</param>
        /// <returns>씬의 루트 게임오브젝트, 실패 시 null</returns>
        public async Task<GameObject> AttachNewScene(string sceneID)
        {
            if (!await TryLoadSceneAsync(sceneID))
                return null;

            var loadedScene = SceneManager.GetSceneByName(sceneID);
            if (!loadedScene.IsValid())
                return null;

            SceneManager.SetActiveScene(loadedScene);

            var sceneObj = FindSceneObject(loadedScene);
            if (sceneObj == null)
                Logger.Fatal($"Failed to find scene object in scene. Scene id : {sceneID}");

            return sceneObj;
        }

        /// <summary>
        /// 로드된 씬에서 Scene 컴포넌트가 부착된 루트 게임오브젝트를 찾는다.
        /// </summary>
        /// <param name="loadedScene">검색할 Unity 씬</param>
        /// <returns>Scene 컴포넌트가 있는 루트 게임오브젝트, 없으면 null</returns>
        private static GameObject FindSceneObject(UnityEngine.SceneManagement.Scene loadedScene)
        {
            foreach (var rootObj in loadedScene.GetRootGameObjects())
                if (rootObj.TryGetComponent<Scene>(out _))
                    return rootObj;
            return null;
        }
    }
}
