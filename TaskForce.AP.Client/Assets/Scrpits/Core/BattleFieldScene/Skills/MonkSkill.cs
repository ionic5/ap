using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 몽크(수도승) 소환 스킬 클래스.
    /// 사용 시 소환자 위치에 몽크 유닛을 생성하는 즉시 시전 스킬이다.
    /// 소환 횟수에 따라 쿨다운이 관리된다.
    /// </summary>
    public class MonkSkill : InstantSkill, ISkill
    {
        /// <summary>
        /// 현재까지의 소환 횟수
        /// </summary>
        private int _summonCount;

        /// <summary>
        /// 유닛 생성 팩토리 함수 (유닛 타입 ID, 레벨을 받아 유닛을 반환)
        /// </summary>
        private readonly Func<string, int, IUnit> _createUnit;

        /// <summary>
        /// MonkSkill의 생성자.
        /// </summary>
        /// <param name="skillEntity">스킬 데이터 엔티티</param>
        /// <param name="createUnit">유닛 생성 팩토리 함수</param>
        public MonkSkill(Entity.IActiveSkill skillEntity, Func<string, int, IUnit> createUnit) : base(skillEntity)
        {
            _createUnit = createUnit;
            _summonCount = 0;
        }

        /// <summary>
        /// 쿨다운이 완료되었는지 확인한다. 소환 횟수가 0 이하이면 사용 가능하다.
        /// </summary>
        /// <returns>소환 횟수가 0 이하이면 true</returns>
        public override bool IsCooldownFinished()
        {
            return _summonCount <= 0;
        }

        /// <summary>
        /// 몽크 소환 스킬을 사용한다. 소환자 위치에 몽크 유닛을 생성하고 마스터로 설정한다.
        /// </summary>
        /// <param name="args">스킬 사용 인자</param>
        public override void Use(UseSkillArgs args)
        {
            var summoner = GetOwner();

            _summonCount++;

            var unit = _createUnit("MONK", 1);
            unit.SetPosition(summoner.GetPosition());
            unit.SetMaster(summoner);
        }

        /// <summary>
        /// 소환 스킬은 대상 기반이 아니므로 항상 false를 반환한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <param name="target">판정 대상</param>
        /// <returns>항상 false</returns>
        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            return false;
        }

        /// <summary>
        /// 소환 스킬은 대상이 필요 없으므로 빈 목록을 반환한다.
        /// </summary>
        /// <param name="unit">스킬을 사용하는 유닛</param>
        /// <returns>빈 대상 목록</returns>
        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            return Enumerable.Empty<ITarget>();
        }
    }
}
