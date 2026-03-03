using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class WindowStack : View.WindowStack, IWindowStack
    {
        public Windows.SkillSelectionWindow SkillSelectionWindow;

        public ISkillSelectionWindow OpenSkillSelectionWindow()
        {
            return OpenWindow(SkillSelectionWindow);
        }
    }
}