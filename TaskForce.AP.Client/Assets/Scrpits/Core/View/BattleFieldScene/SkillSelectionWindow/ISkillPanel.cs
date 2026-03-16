namespace TaskForce.AP.Client.Core.View.BattleFieldScene.SkillSelectionWindow
{
    /// <summary>
    /// 스킬 선택 윈도우 내 개별 스킬 패널을 표현하는 뷰 인터페이스
    /// </summary>
    public interface ISkillPanel
    {
        /// <summary>
        /// 스킬의 설명 텍스트를 설정한다.
        /// </summary>
        /// <param name="v">표시할 스킬 설명 문자열</param>
        void SetDescription(string v);

        /// <summary>
        /// 스킬의 아이콘을 설정한다.
        /// </summary>
        /// <param name="value">아이콘 리소스 식별자 문자열</param>
        void SetIcon(string value);

        /// <summary>
        /// 스킬의 레벨 텍스트를 설정한다.
        /// </summary>
        /// <param name="v">표시할 스킬 레벨 문자열</param>
        void SetLevel(string v);

        /// <summary>
        /// 스킬의 이름을 설정한다.
        /// </summary>
        /// <param name="v">표시할 스킬 이름 문자열</param>
        void SetName(string v);
    }
}
