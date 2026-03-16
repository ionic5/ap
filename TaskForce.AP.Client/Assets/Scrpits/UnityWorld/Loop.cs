using System.Collections.Generic;
using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// Unity의 Update 루프를 통해 등록된 IUpdatable 객체들을 매 프레임 갱신하는 MonoBehaviour 컴포넌트.
    /// 게임 루프의 업데이트 관리자 역할을 한다.
    /// </summary>
    public class Loop : MonoBehaviour, ILoop
    {
        /// <summary>매 프레임 업데이트될 IUpdatable 객체들의 연결 리스트</summary>
        public readonly LinkedList<IUpdatable> updatables = new LinkedList<IUpdatable>();

        /// <summary>
        /// 업데이트 대상 객체를 루프에 추가한다.
        /// </summary>
        /// <param name="updatable">매 프레임 갱신할 객체</param>
        public void Add(IUpdatable updatable)
        {
            updatables.AddLast(updatable);
        }

        /// <summary>
        /// 업데이트 대상 객체를 루프에서 제거한다.
        /// </summary>
        /// <param name="updatable">제거할 객체</param>
        public void Remove(IUpdatable updatable)
        {
            updatables.Remove(updatable);
        }

        private void OnDestroy()
        {
            updatables.Clear();
        }

        private void Update()
        {
            var node = updatables.First;
            while (node != null)
            {
                var current = node;
                node = node.Next;
                current.Value.Update();
            }
        }
    }
}
