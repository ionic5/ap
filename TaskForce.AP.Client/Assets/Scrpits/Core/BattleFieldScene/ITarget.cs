namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 공격 대상이 될 수 있는 객체를 나타내는 인터페이스.
    /// 피격, 회복, 생존 여부 확인, 진영 판별 등의 기능을 제공한다.
    /// </summary>
    public interface ITarget : IDestroyable, IMortal, IPositionable, IFollowable
    {
        /// <summary>
        /// 대상에게 데미지를 적용한다.
        /// </summary>
        /// <param name="damage">적용할 데미지 양</param>
        void Hit(int damage);

        /// <summary>
        /// 생존 상태인지 확인한다.
        /// </summary>
        /// <returns>생존 중이면 true</returns>
        bool IsAlive();

        /// <summary>
        /// 플레이어 진영인지 확인한다.
        /// </summary>
        /// <returns>플레이어 진영이면 true</returns>
        bool IsPlayerSide();

        /// <summary>
        /// 뷰 객체 ID를 가져온다.
        /// </summary>
        /// <returns>뷰 객체 ID 문자열</returns>
        string GetViewID();

        /// <summary>
        /// 남은 HP를 가져온다.
        /// </summary>
        /// <returns>남은 HP</returns>
        int GetRemainHP();

        /// <summary>
        /// HP가 최대인지 확인한다.
        /// </summary>
        /// <returns>최대 HP이면 true</returns>
        bool IsFullHP();

        /// <summary>
        /// 대상을 회복한다.
        /// </summary>
        /// <param name="healAmount">회복량</param>
        void Heal(int healAmount);
    }
}
