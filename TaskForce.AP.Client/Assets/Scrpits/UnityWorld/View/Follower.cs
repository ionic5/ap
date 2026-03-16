using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View
{
    /// <summary>
    /// IFollowable 대상의 위치를 매 물리 프레임마다 추적하는 MonoBehaviour 컴포넌트.
    /// 대상이 파괴되면 자동으로 추적을 중단한다.
    /// </summary>
    public class Follower : MonoBehaviour
    {
        /// <summary>현재 추적 중인 대상</summary>
        private IFollowable _target;
        /// <summary>추적 시 적용되는 위치 오프셋</summary>
        private System.Numerics.Vector2 _offset;

        /// <summary>
        /// 오프셋 없이 대상을 추적한다.
        /// </summary>
        /// <param name="target">추적할 대상</param>
        public void Follow(IFollowable target)
        {
            Follow(target, System.Numerics.Vector2.Zero);
        }

        /// <summary>
        /// 지정된 오프셋을 적용하여 대상을 추적한다.
        /// </summary>
        /// <param name="target">추적할 대상</param>
        /// <param name="offset">위치 오프셋</param>
        public void Follow(IFollowable target, System.Numerics.Vector2 offset)
        {
            _target = target;
            _offset = offset;
            _target.DestroyEvent += OnDestroyTargetEvent;
        }

        /// <summary>
        /// 추적 대상 파괴 이벤트 핸들러. 추적을 자동 중단한다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="args">파괴 이벤트 인자</param>
        private void OnDestroyTargetEvent(object sender, DestroyEventArgs args)
        {
            Unfollow();
        }

        /// <summary>
        /// 대상 추적을 중단하고 이벤트 구독을 해제한다.
        /// </summary>
        public void Unfollow()
        {
            if (_target == null)
                return;

            _target.DestroyEvent -= OnDestroyTargetEvent;
            _target = null;
        }

        /// <summary>
        /// 매 물리 프레임마다 대상의 위치를 따라가도록 자신의 위치를 갱신한다.
        /// </summary>
        public void FixedUpdate()
        {
            if (_target == null)
                return;

            System.Numerics.Vector2 pos = _target.GetPosition();
            transform.position = new Vector3(pos.X + _offset.X, 0, pos.Y + _offset.Y);
        }

        /// <summary>
        /// 오브젝트 파괴 시 추적을 해제한다.
        /// </summary>
        public void OnDestroy()
        {
            Unfollow();
        }
    }
}
