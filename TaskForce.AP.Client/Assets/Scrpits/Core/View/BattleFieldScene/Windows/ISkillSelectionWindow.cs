using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.SkillSelectionWindow;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene.Windows
{
    /// <summary>
    /// 스킬 선택 윈도우를 표현하는 뷰 인터페이스
    /// </summary>
    public interface ISkillSelectionWindow
    {
        /// <summary>
        /// 확인(OK) 버튼이 클릭되었을 때 호출되는 이벤트
        /// </summary>
        event EventHandler OKButtonClickedEvent;

        /// <summary>
        /// 새로운 스킬 패널을 추가하고 해당 인스턴스를 반환한다.
        /// </summary>
        /// <returns>추가된 스킬 패널 인스턴스</returns>
        ISkillPanel AddSkillPanel();

        /// <summary>
        /// 스킬 선택 윈도우를 닫는다.
        /// </summary>
        void Close();

        /// <summary>
        /// 현재 선택된 스킬의 인덱스를 반환한다.
        /// </summary>
        /// <returns>선택된 스킬의 0부터 시작하는 인덱스</returns>
        int GetSelectedSkillIndex();
    }
}
