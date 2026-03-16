using System;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View
{
    /// <summary>
    /// UI 윈도우의 기본 클래스. 열기/닫기 동작과 닫힘 이벤트를 제공한다.
    /// </summary>
    public class Window : MonoBehaviour
    {
        /// <summary>윈도우가 닫힐 때 발생하는 이벤트</summary>
        public event EventHandler ClosedEvent;

        private void OnDestroy()
        {
            Clear();
        }

        /// <summary>
        /// 윈도우를 닫는다. 게임오브젝트를 비활성화하고 ClosedEvent를 발생시킨 뒤 정리한다.
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
            ClosedEvent?.Invoke(this, EventArgs.Empty);

            Clear();
        }

        /// <summary>
        /// 윈도우 리소스를 정리한다. 이벤트 구독을 초기화한다.
        /// 하위 클래스에서 오버라이드하여 추가 정리 작업을 수행할 수 있다.
        /// </summary>
        public virtual void Clear()
        {
            ClosedEvent = null;
        }
    }
}