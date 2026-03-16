using System;
using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 오브젝트 풀링을 지원하는 게임 오브젝트의 기본 클래스.
    /// 파괴 시 실제로 제거하지 않고 풀에 반환하여 재사용할 수 있도록 한다.
    /// </summary>
    public class PoolableObject : Object, IDestroyable
    {
        /// <summary>오브젝트를 풀에 반환하는 핸들러</summary>
        private Action<GameObject> _returnToPoolHanlder;

        /// <summary>
        /// 풀에서 꺼낸 오브젝트를 재활성화하여 재사용 가능 상태로 만든다.
        /// </summary>
        public virtual void Revive()
        {
            _isDestroyed = false;
        }

        /// <summary>
        /// 오브젝트가 파괴될 때 풀에 반환하기 위한 핸들러를 설정한다.
        /// </summary>
        /// <param name="returnToPoolHandler">풀 반환 핸들러</param>
        public void SetReturnToPoolHandler(Action<GameObject> returnToPoolHandler)
        {
            _returnToPoolHanlder = returnToPoolHandler;
        }

        /// <summary>
        /// 오브젝트를 실제로 파괴하지 않고 정리 후 풀에 반환한다.
        /// </summary>
        protected override void HandleDestroy()
        {
            var hdlr = _returnToPoolHanlder;
            OnDestroy();
            hdlr.Invoke(gameObject);
        }

        /// <summary>
        /// 리소스 정리를 수행하고 풀 반환 핸들러를 해제한다.
        /// </summary>
        protected override void CleanUp()
        {
            base.CleanUp();

            _returnToPoolHanlder = null;
        }
    }
}
