using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 스킬 사용 시 전달되는 인자를 담는 구조체.
    /// 스킬 대상과 완료 콜백 정보를 포함한다.
    /// </summary>
    public struct UseSkillArgs
    {
        /// <summary>
        /// 스킬의 대상이 되는 타겟
        /// </summary>
        public ITarget Target;

        /// <summary>
        /// 스킬 사용 완료 시 호출되는 콜백 액션
        /// </summary>
        public Action OnCompleted;
    }
}
