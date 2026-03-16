namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 특정 대상을 추적하는 카메라의 뷰 인터페이스
    /// </summary>
    public interface IFollowCamera
    {
        /// <summary>
        /// 카메라가 추적할 대상을 설정한다.
        /// </summary>
        /// <param name="_unit">추적할 대상 오브젝트</param>
        void SetTarget(Core.BattleFieldScene.IFollowable _unit);

        /// <summary>
        /// 카메라의 추적 대상을 해제한다.
        /// </summary>
        void UnsetTarget();
    }
}
