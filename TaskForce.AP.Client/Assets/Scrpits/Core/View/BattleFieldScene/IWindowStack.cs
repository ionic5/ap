using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// UI 윈도우 스택을 관리하는 뷰 인터페이스
    /// </summary>
    public interface IWindowStack
    {
        /// <summary>
        /// 스킬 선택 윈도우를 열고 해당 인스턴스를 반환한다.
        /// </summary>
        /// <returns>열린 스킬 선택 윈도우 인스턴스</returns>
        ISkillSelectionWindow OpenSkillSelectionWindow();
    }
}
