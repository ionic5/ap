using System;
using TMPro;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View
{
    /// <summary>
    /// 데미지 등의 텍스트를 대상 위에 떠오르는 애니메이션으로 표시하는 클래스.
    /// 오브젝트 풀링을 지원하며, 대상을 추적하면서 텍스트를 표시한다.
    /// </summary>
    public class FloatingTextAnimator : PoolableObject
    {
        /// <summary>형제 순서 변경 시 모든 인스턴스에 알리는 정적 이벤트</summary>
        private static event EventHandler SiblingIndexChangedEvent;
        /// <summary>애니메이션 재생 완료 시 발생하는 이벤트</summary>
        public event EventHandler AnimationFinishedEvent;

        /// <summary>표시할 텍스트 컴포넌트</summary>
        [SerializeField]
        private TMP_Text _text;
        /// <summary>텍스트 애니메이션을 제어하는 Animator</summary>
        [SerializeField]
        private Animator _animator;
        /// <summary>렌더링 순서 관리를 위한 MeshRenderer</summary>
        [SerializeField]
        private MeshRenderer _renderer;
        /// <summary>대상 추적을 담당하는 Follower 컴포넌트</summary>
        [SerializeField]
        private Follower _follower;

        /// <summary>
        /// 애니메이션 클립 이름 상수를 정의하는 내부 클래스
        /// </summary>
        private class ClipName
        {
            /// <summary>데미지 애니메이션 클립 이름</summary>
            public const string Damage = "damage";
        }

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// 오브젝트 풀에서 재사용 시 호출되는 초기화 메서드.
        /// </summary>
        public override void Revive()
        {
            base.Revive();

            Initialize();
        }

        /// <summary>
        /// 형제 순서 변경 이벤트를 구독하여 초기화한다.
        /// </summary>
        private void Initialize()
        {
            SiblingIndexChangedEvent += OnSiblingIndexChangedEvent;
        }

        /// <summary>
        /// 지정된 대상을 오프셋을 적용하여 추적한다.
        /// </summary>
        /// <param name="target">추적할 대상</param>
        /// <param name="offset">추적 시 적용할 위치 오프셋</param>
        public void Follow(Core.BattleFieldScene.IFollowable target, System.Numerics.Vector2 offset)
        {
            _follower.Follow(target, offset);
        }

        /// <summary>
        /// 이 오브젝트를 형제 순서에서 최상위로 이동시키고, 다른 인스턴스에 변경을 알린다.
        /// </summary>
        public void BringToTop()
        {
            gameObject.transform.SetAsLastSibling();
            SiblingIndexChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 데미지 수치를 텍스트로 변환하여 데미지 애니메이션을 재생한다.
        /// </summary>
        /// <param name="damage">표시할 데미지 값</param>
        public void PlayDamageAnimation(int damage)
        {
            var text = damage.ToString();

            PlayAnimation(text, ClipName.Damage);
        }

        /// <summary>
        /// 지정된 텍스트와 클립 이름으로 애니메이션을 재생한다.
        /// </summary>
        /// <param name="text">표시할 텍스트</param>
        /// <param name="clipName">재생할 애니메이션 클립 이름</param>
        private void PlayAnimation(string text, string clipName)
        {
            _text.text = text;

            _animator.Play(clipName, 0, 0f);
        }

        /// <summary>
        /// 애니메이션 완료 시 애니메이션 이벤트로부터 호출된다.
        /// </summary>
        public void OnAnimationFinished()
        {
            AnimationFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 형제 순서 변경 이벤트 핸들러. 렌더링 순서를 형제 인덱스에 맞춰 갱신한다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void OnSiblingIndexChangedEvent(object sender, EventArgs e)
        {
            _renderer.sortingOrder = gameObject.transform.GetSiblingIndex();
        }

        /// <summary>
        /// 오브젝트 풀 반환 시 이벤트 구독 해제 및 추적을 중단한다.
        /// </summary>
        protected override void CleanUp()
        {
            base.CleanUp();

            AnimationFinishedEvent = null;

            SiblingIndexChangedEvent -= OnSiblingIndexChangedEvent;
            _follower.Unfollow();
        }
    }
}
