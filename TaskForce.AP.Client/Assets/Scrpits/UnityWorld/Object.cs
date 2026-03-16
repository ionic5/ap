using System;
using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 파괴 가능한 게임 오브젝트의 기본 클래스.
    /// IDestroyable 인터페이스를 구현하며, 파괴 시 이벤트를 발생시키고 정리 작업을 수행한다.
    /// </summary>
    public class Object : MonoBehaviour, IDestroyable
    {
        /// <summary>오브젝트가 파괴될 때 발생하는 이벤트</summary>
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        /// <summary>오브젝트의 파괴 여부</summary>
        protected bool _isDestroyed;

        /// <summary>
        /// 오브젝트를 파괴한다. 이미 파괴된 경우 무시한다.
        /// </summary>
        public void Destroy()
        {
            if (_isDestroyed)
                return;

            HandleDestroy();
        }

        /// <summary>
        /// 실제 파괴 로직을 수행한다. 하위 클래스에서 재정의할 수 있다.
        /// </summary>
        protected virtual void HandleDestroy()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Unity의 OnDestroy 콜백. 파괴 이벤트를 발생시키고 정리 작업을 수행한다.
        /// </summary>
        protected void OnDestroy()
        {
            if (_isDestroyed) return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));

            CleanUp();
        }

        /// <summary>
        /// 오브젝트 파괴 시 리소스 정리를 수행한다. 하위 클래스에서 재정의할 수 있다.
        /// </summary>
        protected virtual void CleanUp()
        {
            DestroyEvent = null;
        }
    }
}
