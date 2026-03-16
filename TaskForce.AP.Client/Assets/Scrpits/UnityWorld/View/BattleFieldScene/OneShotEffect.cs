using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 한 번만 재생되고 자동으로 파괴되는 이펙트를 처리하는 풀링 가능한 오브젝트.
    /// 대상을 추적하거나 고정 위치에서 재생할 수 있다.
    /// </summary>
    public class OneShotEffect : PoolableObject, IOneShotEffect
    {
        /// <summary>이펙트 애니메이션 컨트롤러</summary>
        [SerializeField]
        private Animator _animator;

        /// <summary>대상 추적을 담당하는 Follower 컴포넌트</summary>
        [SerializeField]
        private Follower _follower;

        /// <summary>
        /// 기본 이펙트 애니메이션을 재생한다.
        /// </summary>
        public void Play()
        {
            _animator.Play("default");
        }

        /// <summary>
        /// 이펙트의 위치를 2D 좌표로 설정한다.
        /// </summary>
        /// <param name="pos">이펙트 위치 (X, Y를 월드 X, Z로 변환)</param>
        public void SetPosition(System.Numerics.Vector2 pos)
        {
            transform.position = new Vector3(pos.X, 0, pos.Y);
        }

        /// <summary>
        /// 애니메이션 완료 시 애니메이션 이벤트로부터 호출된다. 오브젝트를 파괴한다.
        /// </summary>
        public void OnAnimationFinished()
        {
            Destroy();
        }

        /// <summary>
        /// 지정된 대상을 추적한다.
        /// </summary>
        /// <param name="target">추적할 대상</param>
        public void Follow(IFollowable target)
        {
            _follower.Follow(target);
        }
    }
}
