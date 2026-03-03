using System.Threading.Tasks;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class Screen : MonoBehaviour
    {
        private Scene _lastScene;

        [SerializeField]
        private Transform _sceneRoot;
        [SerializeField]
        private Canvas _loadingBlind;

        public Core.ILogger Logger;
        public AssetLoader AssetLoader;

        private TaskCompletionSource<bool> _loadingTcs = null;
        private volatile bool _isShowing = false;

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

        public void HideLoadingBlind()
        {
            if (!_isShowing)
                return;

            _loadingBlind.gameObject.SetActive(false);

            _isShowing = false;

            var tcs = _loadingTcs;
            _loadingTcs = null;
            tcs?.TrySetResult(true);
        }

        public void DestroyLastScene()
        {
            if (_lastScene == null)
                return;

            _lastScene.Destroy();
            _lastScene = null;

            AssetLoader.ClearAllAssets();
            System.GC.Collect();
        }

        public async Task<GameObject> AttachNewScene(string prefabID)
        {
            var prefab = await AssetLoader.LoadAsset<GameObject>(prefabID);
            if (prefab == null)
            {
                Logger.Fatal($"Prefab not found for id: {prefabID}");
                return null;
            }

            var instance = Instantiate(prefab, _sceneRoot);
            instance.transform.SetSiblingIndex(0);

            _lastScene = instance.GetComponent<Scene>();
            if (_lastScene == null)
            {
                Logger.Fatal($"Critical Error: Attached GameObject '{instance.name}' at path '{prefabID}' " +
                             $"is missing the required 'Scene' component.");

                Destroy(instance.gameObject);
                return null;
            }

            return instance;
        }
    }
}
