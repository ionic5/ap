using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene.SkillSelectionWindow;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;
using TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.SkillSelectionWindow;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.Windows
{
    /// <summary>
    /// 스킬 선택 UI 윈도우. 복수의 스킬 패널을 표시하고 플레이어의 선택을 처리한다.
    /// </summary>
    public class SkillSelectionWindow : Window, ISkillSelectionWindow
    {
        /// <summary>확인 버튼 클릭 시 발생하는 이벤트</summary>
        public event EventHandler OKButtonClickedEvent;

        /// <summary>스킬 패널 배열 (미리 배치된 UI 요소)</summary>
        [SerializeField]
        public SkillPanel[] SkillPanels;

        private void Awake()
        {
            ResetSkillPanels();
        }

        /// <summary>
        /// 비활성화된 스킬 패널 하나를 활성화하여 반환한다. 모두 사용 중이면 null을 반환한다.
        /// </summary>
        /// <returns>활성화된 스킬 패널 인터페이스. 가용한 패널이 없으면 null</returns>
        public ISkillPanel AddSkillPanel()
        {
            for (var i = 0; i < SkillPanels.Count(); i++)
            {
                var panel = SkillPanels[i];
                if (panel.gameObject.activeSelf)
                    continue;

                panel.gameObject.SetActive(true);

                return panel;
            }

            return null;
        }

        /// <summary>
        /// 현재 선택된 스킬 패널의 인덱스를 반환한다.
        /// </summary>
        /// <returns>선택된 패널의 인덱스. 선택된 것이 없으면 -1</returns>
        public int GetSelectedSkillIndex()
        {
            for (var i = 0; i < SkillPanels.Count(); i++)
            {
                if (SkillPanels[i].IsSelected())
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 윈도우 리소스를 정리하고 스킬 패널을 초기 상태로 되돌린다.
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            ResetSkillPanels();
        }

        /// <summary>
        /// 모든 스킬 패널을 비활성화하고 첫 번째 패널을 기본 선택 상태로 설정한다.
        /// </summary>
        private void ResetSkillPanels()
        {
            foreach (var panel in SkillPanels)
                panel.gameObject.SetActive(false);
            SkillPanels[0].SetSelected();
        }

        /// <summary>
        /// 확인 버튼 클릭 시 UI 이벤트로부터 호출된다.
        /// </summary>
        public void OnOKButtonClicked()
        {
            OKButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}