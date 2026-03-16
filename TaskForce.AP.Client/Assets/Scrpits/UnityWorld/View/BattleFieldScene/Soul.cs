using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 전장의 영혼(Soul) 오브젝트를 나타내는 풀링 가능한 클래스.
    /// 대상을 향해 이동하거나, 고정 위치에 배치될 수 있다.
    /// </summary>
    public class Soul : PoolableObject, Core.View.BattleFieldScene.ISoul
    {
        /// <summary>추적 대상</summary>
        private Core.BattleFieldScene.IFollowable _followTarget;
        /// <summary>대상 위치 캐싱용 벡터 (재할당 방지)</summary>
        private Vector3 _unitPosition;
        /// <summary>현재 위치 캐싱용 2D 벡터</summary>
        private System.Numerics.Vector2 _position;
        /// <summary>이동 속도</summary>
        private float _speed;

        private void Awake()
        {
            _unitPosition = new Vector3();
        }

        /// <summary>
        /// 영혼의 현재 위치를 2D 좌표로 반환한다.
        /// </summary>
        /// <returns>현재 월드 위치의 2D 좌표 (X, Z를 X, Y로 변환)</returns>
        public System.Numerics.Vector2 GetPosition()
        {
            _position.X = transform.position.x;
            _position.Y = transform.position.z;

            return _position;
        }

        /// <summary>
        /// 지정된 대상을 향해 일정 속도로 이동을 시작한다.
        /// </summary>
        /// <param name="followTarget">이동 대상</param>
        /// <param name="speed">이동 속도</param>
        public void MoveTo(Core.BattleFieldScene.IFollowable followTarget, float speed)
        {
            _followTarget = followTarget;
            _speed = speed;
        }

        /// <summary>
        /// 영혼의 위치를 2D 좌표로 설정한다. Y축(높이)은 유지된다.
        /// </summary>
        /// <param name="position">설정할 위치 (X, Y를 월드 X, Z로 변환)</param>
        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, transform.position.y, position.Y);
        }

        /// <summary>
        /// 대상 추적을 중단한다.
        /// </summary>
        public void Stop()
        {
            _followTarget = null;
        }

        private void Update()
        {
            if (_followTarget == null)
                return;

            transform.position = Vector3.MoveTowards(
                transform.position,
                GetUnitPosition(),
                _speed * UnityEngine.Time.deltaTime
            );
        }

        /// <summary>
        /// 추적 대상의 위치를 3D 벡터로 변환하여 반환한다. Y축은 현재 높이를 유지한다.
        /// </summary>
        /// <returns>대상의 월드 좌표</returns>
        private Vector3 GetUnitPosition()
        {
            var pos = _followTarget.GetPosition();

            _unitPosition.x = pos.X;
            _unitPosition.z = pos.Y;
            _unitPosition.y = transform.position.y;

            return _unitPosition;
        }
    }
}
