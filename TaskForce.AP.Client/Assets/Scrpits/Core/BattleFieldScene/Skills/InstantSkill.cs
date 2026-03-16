namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 즉시 시전(인스턴트) 스킬의 기본 구현을 제공하는 추상 클래스.
    /// 시전 시간이 없으며 사용 즉시 완료 처리되고, 취소가 불필요한 스킬에 사용된다.
    /// </summary>
    public abstract class InstantSkill : ActiveSkill
    {
        /// <summary>
        /// InstantSkill의 생성자.
        /// </summary>
        /// <param name="skillEntity">스킬 데이터 엔티티</param>
        protected InstantSkill(Entity.IActiveSkill skillEntity) : base(skillEntity)
        {
        }

        /// <summary>
        /// 즉시 시전 스킬이므로 항상 true를 반환한다.
        /// </summary>
        /// <returns>항상 true</returns>
        public override bool IsInstantSkill()
        {
            return true;
        }

        /// <summary>
        /// 즉시 시전 스킬이므로 항상 완료 상태(true)를 반환한다.
        /// </summary>
        /// <returns>항상 true</returns>
        public override bool IsCompleted()
        {
            return true;
        }

        /// <summary>
        /// 즉시 시전 스킬은 취소할 필요가 없으므로 아무 동작도 수행하지 않는다.
        /// </summary>
        public override void Cancel()
        {
        }
    }
}
