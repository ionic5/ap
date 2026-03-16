namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 유닛의 모션 종류를 나타내는 열거형
    /// </summary>
    public enum UnitMotionID
    {
        /// <summary>
        /// 공격 모션
        /// </summary>
        Attack,

        /// <summary>
        /// 사망 모션
        /// </summary>
        Die,

        /// <summary>
        /// 대기 모션
        /// </summary>
        Stand,

        /// <summary>
        /// 이동 모션
        /// </summary>
        Move,

        /// <summary>
        /// 시전(캐스팅) 모션
        /// </summary>
        Cast
    }
}
