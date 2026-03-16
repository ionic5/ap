using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 프리팹 기반 오브젝트를 생성하고, 오브젝트 풀링을 통해 재사용을 관리하는 팩토리 클래스.
    /// 오브젝트 ID별 프리팹 매핑과 사전 준비 핸들러 등록을 지원한다.
    /// </summary>
    public class ObjectFactory : MonoBehaviour
    {
        /// <summary>오류 로깅을 위한 로거 인스턴스</summary>
        public Core.ILogger Logger;

        /// <summary>프리팹이 생성될 기본 루트 게임오브젝트</summary>
        [SerializeField]
        private GameObject _defaultRoot;
        /// <summary>오브젝트 ID와 프리팹 매핑 배열</summary>
        [SerializeField]
        private PrefabContainer[] _prefabContainers;

        /// <summary>오브젝트 ID별 생성 전 사전 준비 핸들러 딕셔너리</summary>
        private Dictionary<string, Action<UnityWorld.Object>> _prepareHandlers;
        /// <summary>오브젝트 ID별 풀링된 오브젝트 큐 딕셔너리</summary>
        private Dictionary<string, Queue<PoolableObject>> _objectPools;

        /// <summary>
        /// 오브젝트 ID, 프리팹, 루트 오브젝트를 묶는 직렬화 가능한 컨테이너 클래스.
        /// </summary>
        [Serializable]
        public class PrefabContainer
        {
            /// <summary>오브젝트 식별자</summary>
            [SerializeField]
            public string ObjectID;

            /// <summary>인스턴스화할 프리팹</summary>
            [SerializeField]
            public GameObject Prefab;

            /// <summary>생성된 오브젝트의 부모 루트 (null이면 기본 루트 사용)</summary>
            [SerializeField]
            public GameObject Root;
        }

        private void Awake()
        {
            _prepareHandlers = new Dictionary<string, Action<UnityWorld.Object>>();
            _objectPools = new Dictionary<string, Queue<PoolableObject>>();
        }

        /// <summary>
        /// 특정 오브젝트 ID에 대한 생성 전 사전 준비 핸들러를 등록한다.
        /// 오브젝트 생성 시 해당 핸들러가 호출되어 초기 설정을 수행한다.
        /// </summary>
        /// <param name="objectID">핸들러를 등록할 오브젝트 식별자</param>
        /// <param name="handler">오브젝트 생성 시 호출될 사전 준비 핸들러</param>
        public void RegisterPrepareHandler(string objectID, Action<UnityWorld.Object> handler)
        {
            if (string.IsNullOrEmpty(objectID) || handler == null)
            {
                Logger.Fatal("Invalid prepare handler registration.");
                return;
            }
            _prepareHandlers[objectID] = handler;
        }

        /// <summary>
        /// 지정된 오브젝트 ID에 해당하는 오브젝트를 풀에서 가져오거나 새로 생성한다.
        /// 등록된 사전 준비 핸들러가 있으면 호출한다.
        /// </summary>
        /// <typeparam name="T">생성할 오브젝트의 타입</typeparam>
        /// <param name="objectID">생성할 오브젝트의 식별자</param>
        /// <returns>생성된 오브젝트 인스턴스, 실패 시 null</returns>
        public T Create<T>(string objectID) where T : UnityWorld.Object
        {
            var newObj = GetNewObject<T>(objectID) ?? CreateNewObject<T>(objectID);

            if (newObj != null && _prepareHandlers.TryGetValue(objectID, out var handler))
                handler?.Invoke(newObj);

            return newObj;
        }

        /// <summary>
        /// 프리팹으로부터 새 오브젝트를 인스턴스화한다.
        /// </summary>
        /// <typeparam name="T">생성할 오브젝트의 타입</typeparam>
        /// <param name="objectID">생성할 오브젝트의 식별자</param>
        /// <returns>생성된 오브젝트 인스턴스, 실패 시 null</returns>
        private T CreateNewObject<T>(string objectID) where T : UnityWorld.Object
        {
            var container = _prefabContainers.FirstOrDefault(c => c.ObjectID == objectID);
            if (container == null || container.Prefab == null)
            {
                Logger.Fatal($"Object Creation Failed: ID '{objectID}' not found in Pool or Prefab list.");
                return null;
            }

            var root = container.Root == null ? _defaultRoot.transform : container.Root.transform;
            GameObject go = Instantiate(container.Prefab, root);
            if (!go.TryGetComponent<T>(out T newObj))
            {
                Logger.Fatal($"Component {typeof(T)} not found on instantiated prefab '{objectID}'.");
                return null;
            }

            if (newObj is PoolableObject poolable)
                poolable.SetReturnToPoolHandler(_ => Release(objectID, poolable));

            return newObj;
        }

        /// <summary>
        /// 오브젝트 풀에서 재사용 가능한 오브젝트를 가져온다.
        /// </summary>
        /// <typeparam name="T">가져올 오브젝트의 타입</typeparam>
        /// <param name="objectID">가져올 오브젝트의 식별자</param>
        /// <returns>풀에서 가져온 오브젝트 인스턴스, 풀이 비어있으면 null</returns>
        private T GetNewObject<T>(string objectID) where T : UnityWorld.Object
        {
            if (!_objectPools.TryGetValue(objectID, out var pool) || pool.Count <= 0)
                return null;

            var pooledObj = pool.Dequeue();
            if (pooledObj == null)
                return null;

            pooledObj.gameObject.SetActive(true);
            pooledObj.Revive();

            pooledObj.SetReturnToPoolHandler(_ => Release(objectID, pooledObj));

            return pooledObj as T;
        }

        /// <summary>
        /// 오브젝트를 비활성화하고 풀에 반환한다.
        /// </summary>
        /// <param name="objectID">반환할 오브젝트의 식별자</param>
        /// <param name="obj">풀에 반환할 PoolableObject 인스턴스</param>
        private void Release(string objectID, UnityWorld.PoolableObject obj)
        {
            if (string.IsNullOrEmpty(objectID) || obj == null)
                return;

            obj.gameObject.SetActive(false);

            if (!_objectPools.ContainsKey(objectID))
                _objectPools[objectID] = new Queue<UnityWorld.PoolableObject>();

            _objectPools[objectID].Enqueue(obj);
        }
    }
}
