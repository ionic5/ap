using System.Collections.Generic;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 전투 필드 씬에서 윈도우(팝업)를 여는 유틸리티 클래스.
    /// 스킬 선택 윈도우 등의 팝업 생성 및 초기화를 담당한다.
    /// </summary>
    public class WindowOpener
    {
        /// <summary>윈도우 스택 (윈도우 열기/관리)</summary>
        private readonly IWindowStack _windowStack;
        /// <summary>텍스트 저장소</summary>
        private readonly TextStore _textStore;
        /// <summary>로거</summary>
        private readonly ILogger _logger;

        /// <summary>
        /// WindowOpener의 생성자.
        /// </summary>
        /// <param name="windowStack">윈도우 스택</param>
        /// <param name="textStore">텍스트 저장소</param>
        /// <param name="logger">로거</param>
        public WindowOpener(IWindowStack windowStack, TextStore textStore, ILogger logger)
        {
            _windowStack = windowStack;
            _textStore = textStore;
            _logger = logger;
        }

        /// <summary>
        /// 스킬 선택 윈도우를 열고 컨트롤러를 초기화한다.
        /// </summary>
        /// <param name="unit">스킬을 적용할 유닛 엔티티</param>
        /// <param name="skills">선택 가능한 스킬 목록</param>
        public void OpenPerkSelectionWindow(Entity.Unit unit, IEnumerable<Entity.ISkill> skills)
        {
            var window = _windowStack.OpenSkillSelectionWindow();
            var ctrl = new SkillSelectionWindowController(window, skills, unit, _textStore);
            ctrl.Start();
        }
    }
}
