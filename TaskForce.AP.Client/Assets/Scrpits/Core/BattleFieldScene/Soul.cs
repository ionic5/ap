using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 적 유닛 사망 시 드롭되는 소울 객체 클래스.
    /// 플레이어 유닛이 흡수하면 경험치를 제공한다. 특정 대상을 추적하여 이동할 수 있다.
    /// </summary>
    public class Soul : IDestroyable
    {
        /// <summary>소울 파괴 시 발생하는 이벤트</summary>
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        /// <summary>파괴 여부 플래그</summary>
        private bool _isDestroyed;
        /// <summary>소울 뷰 객체</summary>
        private readonly Core.View.BattleFieldScene.ISoul _soulView;
        /// <summary>소울 레벨 (경험치 양 결정)</summary>
        private readonly int _level;
        /// <summary>현재 추적 중인 대상</summary>
        private BattleFieldScene.IFollowable _followTarget;

        /// <summary>
        /// Soul의 생성자.
        /// </summary>
        /// <param name="soulView">소울 뷰 객체</param>
        /// <param name="level">소울 레벨</param>
        public Soul(View.BattleFieldScene.ISoul soulView, int level)
        {
            _soulView = soulView;
            _level = level;

            _soulView.DestroyEvent += OnDestroyEvent;
        }

        /// <summary>
        /// 소울의 위치를 설정한다.
        /// </summary>
        /// <param name="position">설정할 위치</param>
        public void SetPosition(Vector2 position)
        {
            _soulView.SetPosition(position);
        }

        /// <summary>
        /// 지정된 대상을 향해 이동 중인지 확인한다.
        /// </summary>
        /// <param name="followTarget">확인할 추적 대상</param>
        /// <returns>해당 대상을 추적 중이면 true</returns>
        public bool IsMovingTo(Core.BattleFieldScene.IFollowable followTarget)
        {
            return _followTarget == followTarget;
        }

        /// <summary>
        /// 지정된 대상을 향해 이동을 시작한다.
        /// </summary>
        /// <param name="followTarget">추적할 대상</param>
        /// <param name="speed">이동 속도</param>
        public void MoveTo(BattleFieldScene.IFollowable followTarget, float speed)
        {
            if (_followTarget != null)
                UnsetFollowTarget();

            _followTarget = followTarget;

            _followTarget.DestroyEvent += OnDestoryUnitEvent;
            _soulView.MoveTo(_followTarget, speed);
        }

        private void UnsetFollowTarget()
        {
            if (_followTarget == null)
                return;

            _followTarget.DestroyEvent -= OnDestoryUnitEvent;
            _followTarget = null;
        }

        private void OnDestoryUnitEvent(object sender, DestroyEventArgs args)
        {
            if (args.TargetObject != _followTarget)
                return;

            UnsetFollowTarget();
            _soulView.Stop();
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        /// <summary>
        /// 소울의 현재 위치를 가져온다.
        /// </summary>
        /// <returns>현재 위치 벡터</returns>
        public Vector2 GetPosition()
        {
            return _soulView.GetPosition();
        }

        /// <summary>
        /// 소울을 파괴한다. 뷰와 추적 대상의 이벤트 구독을 해제하고 리소스를 정리한다.
        /// </summary>
        public void Destroy()
        {
            if (_isDestroyed)
                return;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            _isDestroyed = true;

            UnsetFollowTarget();

            _soulView.DestroyEvent -= OnDestroyEvent;
            _soulView.Destroy();
        }

        /// <summary>
        /// 소울의 레벨을 가져온다.
        /// </summary>
        /// <returns>소울 레벨</returns>
        public int GetLevel()
        {
            return _level;
        }
    }
}
