using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 전투 유닛을 표현하는 뷰 인터페이스
    /// </summary>
    public interface IUnit : IDestroyable
    {
        /// <summary>
        /// 사망 애니메이션 재생이 완료되었을 때 호출되는 이벤트
        /// </summary>
        event EventHandler DieAnimationFinishedEvent;

        /// <summary>
        /// 유닛의 이동 방향이 변경되었을 때 호출되는 이벤트
        /// </summary>
        event EventHandler MoveDirectionChangedEvent;

        /// <summary>
        /// 유닛의 고유 오브젝트 식별자를 반환한다.
        /// </summary>
        /// <returns>유닛의 고유 오브젝트 ID 문자열</returns>
        string GetObjectID();

        /// <summary>
        /// 지정한 모션을 방향, 재생 시간, 강제 재시작 여부와 함께 재생한다.
        /// </summary>
        /// <param name="motionID">재생할 모션 식별자</param>
        /// <param name="direction">모션 재생 방향</param>
        /// <param name="playTime">모션 재생 시간</param>
        /// <param name="forceRestart">동일 모션 재생 중일 때 강제로 재시작할지 여부</param>
        void PlayMotion(UnitMotionID motionID, Vector2 direction, float playTime, bool forceRestart);

        /// <summary>
        /// 지정한 모션을 기본 설정으로 재생한다.
        /// </summary>
        /// <param name="motionID">재생할 모션 식별자</param>
        void PlayMotion(UnitMotionID motionID);

        /// <summary>
        /// 피격 시 데미지 애니메이션을 재생한다.
        /// </summary>
        /// <param name="damage">표시할 데미지 수치</param>
        void PlayDamageAnimation(int damage);

        /// <summary>
        /// 회복 시 힐 애니메이션을 재생한다.
        /// </summary>
        /// <param name="healAmount">표시할 회복량</param>
        void PlayHealAnimation(int healAmount);

        /// <summary>
        /// 유닛의 위치를 설정한다.
        /// </summary>
        /// <param name="position">설정할 2D 위치 좌표</param>
        void SetPosition(Vector2 position);

        /// <summary>
        /// 유닛의 현재 위치를 반환한다.
        /// </summary>
        /// <returns>유닛의 현재 2D 위치 좌표</returns>
        Vector2 GetPosition();

        /// <summary>
        /// 유닛의 바라보는 방향을 설정한다.
        /// </summary>
        /// <param name="vector2">설정할 방향 벡터</param>
        void SetDirection(Vector2 vector2);

        /// <summary>
        /// 유닛의 현재 바라보는 방향을 반환한다.
        /// </summary>
        /// <returns>유닛의 현재 방향 벡터</returns>
        Vector2 GetDirection();

        /// <summary>
        /// 유닛을 지정한 속도 벡터로 이동시킨다.
        /// </summary>
        /// <param name="velocity">이동 속도 벡터</param>
        void Move(Vector2 velocity);

        /// <summary>
        /// 유닛을 지정한 위치로 일정 속도로 이동시킨다.
        /// </summary>
        /// <param name="position">이동 목적지 좌표</param>
        /// <param name="speed">이동 속도</param>
        void MoveTo(Vector2 position, float speed);

        /// <summary>
        /// 유닛의 이동을 정지시킨다.
        /// </summary>
        void Stop();

        /// <summary>
        /// 유닛의 현재 이동 방향을 반환한다.
        /// </summary>
        /// <returns>유닛의 현재 이동 방향 벡터</returns>
        Vector2 GetMoveDirection();
    }
}
