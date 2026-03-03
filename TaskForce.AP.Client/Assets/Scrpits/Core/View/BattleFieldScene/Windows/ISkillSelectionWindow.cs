using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.SkillSelectionWindow;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene.Windows
{
    public interface ISkillSelectionWindow
    {
        event EventHandler OKButtonClickedEvent;

        ISkillPanel AddSkillPanel();
        void Close();
        int GetSelectedSkillIndex();
    }
}
