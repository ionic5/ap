using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 전장 씬 전용 윈도우 스택. 기본 WindowStack을 확장하여 스킬 선택 윈도우 열기 기능을 제공한다.
    /// </summary>
    public class WindowStack : View.WindowStack, IWindowStack
    {
        /// <summary>스킬 선택 윈도우 인스턴스 참조</summary>
        public Windows.SkillSelectionWindow SkillSelectionWindow;

        /// <summary>
        /// 스킬 선택 윈도우를 열고 반환한다.
        /// </summary>
        /// <returns>열린 스킬 선택 윈도우 인터페이스</returns>
        public ISkillSelectionWindow OpenSkillSelectionWindow()
        {
            return OpenWindow(SkillSelectionWindow);
        }
    }
}