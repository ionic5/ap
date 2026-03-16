using System;

namespace TaskForce.AP.Client.Core.View
{
    /// <summary>
    /// 충돌(히트) 발생 시 전달되는 이벤트 인자 클래스
    /// </summary>
    public class HitEventArgs : EventArgs
    {
        /// <summary>
        /// 충돌이 발생한 대상 오브젝트의 고유 식별자
        /// </summary>
        public string ObjectID;
    }
}
