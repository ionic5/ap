using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.Entity;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 스킬 선택 윈도우의 컨트롤러 클래스.
    /// 레벨업 시 선택 가능한 스킬 목록을 표시하고, 플레이어가 선택한 스킬을 유닛에 적용한다.
    /// </summary>
    public class SkillSelectionWindowController
    {
        /// <summary>스킬 선택 윈도우 뷰</summary>
        private readonly ISkillSelectionWindow _window;
        /// <summary>선택 가능한 스킬 목록</summary>
        private readonly IEnumerable<ISkill> _skills;
        /// <summary>텍스트 저장소 (현지화 텍스트 제공)</summary>
        private readonly TextStore _textStore;
        /// <summary>스킬을 적용할 유닛 엔티티</summary>
        private readonly Entity.Unit _unit;

        /// <summary>
        /// SkillSelectionWindowController의 생성자.
        /// </summary>
        /// <param name="window">스킬 선택 윈도우 뷰</param>
        /// <param name="skills">선택 가능한 스킬 목록</param>
        /// <param name="unit">스킬을 적용할 유닛 엔티티</param>
        /// <param name="textStore">텍스트 저장소</param>
        public SkillSelectionWindowController(ISkillSelectionWindow window, IEnumerable<Entity.ISkill> skills,
            Entity.Unit unit, TextStore textStore)
        {
            _window = window;
            _skills = skills;
            _unit = unit;
            _textStore = textStore;
        }

        /// <summary>
        /// 스킬 선택 윈도우를 초기화하고 스킬 패널들을 생성하여 표시한다.
        /// </summary>
        public void Start()
        {
            foreach (var skill in _skills)
            {
                var panel = _window.AddSkillPanel();
                panel.SetName(skill.GetName());
                //panel.SetDescription(_textStore.GetText(skill.GetDescTextID()));
                var form = _textStore.GetText(TextID.LevelFormat0);
                panel.SetLevel(string.Format(form, skill.GetLevel()));
                //panel.SetIcon(skill.GetIconID());
            }

            _window.OKButtonClickedEvent += OnOKButtonClickedEvent;
        }

        /// <summary>
        /// 확인 버튼 클릭 이벤트 핸들러. 선택된 스킬을 유닛에 추가하거나 레벨업시키고 윈도우를 닫는다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="args">이벤트 인자</param>
        public void OnOKButtonClickedEvent(object sender, EventArgs args)
        {
            var index = _window.GetSelectedSkillIndex();


            var newSKill = _skills.ElementAt(index);
            var skill = _unit.GetSkill(newSKill.GetSkillID());
            if (skill != null)
            {
                skill.LevelUp();
            }
            else
            {
                newSKill.SetOwner(_unit);
                newSKill.AddToOwner();
            }

            _window.OKButtonClickedEvent -= OnOKButtonClickedEvent;
            _window.Close();
        }
    }
}
