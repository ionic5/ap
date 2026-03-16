using System;
using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// Unity 씬의 루트 오브젝트에 부착되는 MonoBehaviour 컴포넌트.
    /// 씬 파괴 시 이벤트를 발생시켜 관련 리소스 정리를 가능하게 한다.
    /// </summary>
    public class Scene : MonoBehaviour, IDestroyable
    {
        /// <summary>씬이 파괴될 때 발생하는 이벤트</summary>
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        /// <summary>
        /// 씬의 게임오브젝트를 파괴한다.
        /// </summary>
        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;
        }
    }
}
