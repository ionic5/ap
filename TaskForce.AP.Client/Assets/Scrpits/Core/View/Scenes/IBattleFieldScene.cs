namespace TaskForce.AP.Client.Core.View.Scenes
{
    /// <summary>
    /// 전투 필드 씬의 UI를 관리하는 뷰 인터페이스
    /// </summary>
    public interface IBattleFieldScene : IDestroyable
    {
        /// <summary>
        /// 현재 경험치 수치를 설정한다.
        /// </summary>
        /// <param name="v">표시할 현재 경험치 값</param>
        void SetExp(int v);

        /// <summary>
        /// 현재 레벨 텍스트를 설정한다.
        /// </summary>
        /// <param name="v">표시할 레벨 문자열</param>
        void SetLevel(string v);

        /// <summary>
        /// 레벨업에 필요한 경험치 수치를 설정한다.
        /// </summary>
        /// <param name="v">레벨업에 필요한 경험치 값</param>
        void SetRequireExp(int v);
    }
}
