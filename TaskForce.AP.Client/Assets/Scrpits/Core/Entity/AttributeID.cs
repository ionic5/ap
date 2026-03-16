namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 유닛 및 스킬에서 사용되는 속성(Attribute) ID 상수를 정의하는 클래스.
    /// 각 상수는 게임 데이터에서 속성을 식별하는 데 사용된다.
    /// </summary>
    public class AttributeID
    {
        /// <summary>발사 각도.</summary>
        public const string LaunchAngle = "LAUNCH_ANGLE";

        /// <summary>연발 횟수.</summary>
        public const string BurstCount = "BURST_COUNT";

        /// <summary>연발 간격.</summary>
        public const string BurstInterval = "BURST_INTERVAL";

        /// <summary>쿨다운 시간.</summary>
        public const string CooldownTime = "COOLDOWN_TIME";

        /// <summary>투사체 사거리.</summary>
        public const string MissileRange = "MISSILE_RANGE";

        /// <summary>투사체 속도.</summary>
        public const string MissileSpeed = "MISSILE_SPEED";

        /// <summary>휘두르기 각도.</summary>
        public const string SwingAngle = "SWING_ANGLE";

        /// <summary>최소 투사체 사거리.</summary>
        public const string MinMissileRange = "MIN_MISSILE_RANGE";

        /// <summary>최대 투사체 사거리.</summary>
        public const string MaxMissileRange = "MAX_MISSILE_RANGE";

        /// <summary>폭발 반경.</summary>
        public const string ExplosionRadius = "EXPLOSION_RADIUS";

        /// <summary>감시 반경.</summary>
        public const string WatchRadius = "WATCH_RADIUS";

        /// <summary>만료 시간.</summary>
        public const string ExpireTime = "EXPIRE_TIME";

        /// <summary>치유량.</summary>
        public const string HealAmount = "HEAL_AMOUNT";

        /// <summary>적용 지연 시간.</summary>
        public const string ApplyDelayTime = "APPLY_DELAY_TIME";

        /// <summary>시전 사거리.</summary>
        public const string CastRange = "CAST_RANGE";

        /// <summary>시전 시간.</summary>
        public const string CastTime = "CAST_TIME";

        /// <summary>이동 속도.</summary>
        public const string MoveSpeed = "MOVE_SPEED";

        /// <summary>공격 사거리.</summary>
        public const string AttackRange = "ATTACK_RANGE";

        /// <summary>공격 시간.</summary>
        public const string AttackTime = "ATTACK_TIME";

        /// <summary>공격 적중 시점.</summary>
        public const string AttackImpactTime = "ATTACK_IMPACT_TIME";

        /// <summary>최소 데미지.</summary>
        public const string MinDamage = "MIN_DAMAGE";

        /// <summary>최대 데미지.</summary>
        public const string MaxDamage = "MAX_DAMAGE";

        /// <summary>탐지 범위.</summary>
        public const string DetectRange = "DETECT_RANGE";

        /// <summary>최대 체력.</summary>
        public const string MaxHP = "MAX_HP";

        /// <summary>범위.</summary>
        public const string Range = "RANGE";
    }
}
