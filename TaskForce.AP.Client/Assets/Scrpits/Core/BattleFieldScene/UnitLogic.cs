using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 유닛 로직의 추상 기본 클래스.
    /// 게임 루프에 등록되어 매 프레임 OnUpdate를 호출하며,
    /// 유닛이 사망하거나 파괴되면 업데이트를 중지한다.
    /// </summary>
    public abstract class UnitLogic : IUnitLogic, IUpdatable
    {
        /// <summary>게임 루프</summary>
        private readonly ILoop _loop;
        /// <summary>제어 대상 유닛</summary>
        private IControllableUnit _unit;
        /// <summary>파괴 여부 플래그</summary>
        private bool _isDestroyed;
        /// <summary>제어 시작 여부 플래그</summary>
        private bool _isOnControl;

        /// <summary>로직 파괴 시 발생하는 이벤트</summary>
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        /// <summary>
        /// UnitLogic의 생성자.
        /// </summary>
        /// <param name="loop">게임 루프</param>
        protected UnitLogic(ILoop loop)
        {
            _loop = loop;
            _isOnControl = false;
            _isDestroyed = false;
        }

        /// <summary>
        /// 지정된 유닛에 대한 제어를 시작한다. 게임 루프에 등록한다.
        /// </summary>
        /// <param name="unit">제어할 유닛</param>
        public void StartControl(IControllableUnit unit)
        {
            _loop.Add(this);
            _isOnControl = true;
            _unit = unit;
        }

        /// <summary>
        /// 제어 대상 유닛을 가져온다.
        /// </summary>
        /// <returns>제어 대상 유닛</returns>
        protected IControllableUnit GetControlTarget()
        {
            return _unit;
        }

        /// <summary>
        /// 로직이 파괴 상태인지 확인한다.
        /// </summary>
        /// <returns>파괴되었으면 true</returns>
        protected bool IsDestroyed()
        {
            return _isDestroyed;
        }

        /// <summary>
        /// 매 프레임 호출된다. 파괴되었거나 유닛이 사망한 경우 업데이트를 건너뛴다.
        /// </summary>
        public void Update()
        {
            if (IsDestroyed() || _unit.IsDead())
                return;

            OnUpdate();
        }

        /// <summary>
        /// 파생 클래스에서 구현할 업데이트 로직.
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// 로직을 파괴한다. 게임 루프에서 제거하고 리소스를 정리한다.
        /// </summary>
        public void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            if (_isOnControl)
                _loop.Remove(this);

            _unit = null;

            OnDestroy();
        }

        /// <summary>
        /// 파괴 시 파생 클래스에서 추가 정리 작업을 수행하기 위한 가상 메서드.
        /// </summary>
        protected virtual void OnDestroy()
        {
        }
    }
}
