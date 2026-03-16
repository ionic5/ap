using System;

namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 스킬 추가 이벤트의 인자 클래스. 유닛에 스킬이 추가될 때 발생하는 이벤트에서
    /// 추가된 스킬의 ID를 전달한다.
    /// </summary>
    public class SkillAddedEventArgs : EventArgs
    {
        /// <summary>추가된 스킬의 고유 식별자.</summary>
        public string SkillID;
    }
}
