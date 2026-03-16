using System;
using System.Threading;
using TaskForce.AP.Client.Core.View.BattleFieldScene.SkillSelectionWindow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.SkillSelectionWindow
{
    /// <summary>
    /// 스킬 선택 윈도우에서 개별 스킬 정보를 표시하는 패널 UI 컴포넌트.
    /// 스킬의 이름, 레벨, 설명, 아이콘을 표시하며 선택 상태를 토글로 관리한다.
    /// </summary>
    public class SkillPanel : MonoBehaviour, ISkillPanel
    {
        /// <summary>스킬 설명 텍스트</summary>
        [SerializeField]
        private TMP_Text _descriptionText;
        /// <summary>스킬 레벨 텍스트</summary>
        [SerializeField]
        private TMP_Text _levelText;
        /// <summary>스킬 이름 텍스트</summary>
        [SerializeField]
        private TMP_Text _nameText;
        /// <summary>스킬 아이콘 이미지</summary>
        [SerializeField]
        private Image _iconImage;
        /// <summary>스킬 선택 토글 버튼</summary>
        [SerializeField]
        private Toggle _toggle;

        /// <summary>에셋 비동기 로드를 담당하는 로더</summary>
        public AssetLoader AssetLoader;
        /// <summary>로깅을 위한 로거</summary>
        public Core.ILogger Logger;

        /// <summary>아이콘 로드 취소를 위한 토큰 소스</summary>
        private CancellationTokenSource _loadIconToken;
        /// <summary>현재 로드 중인 아이콘 ID (중복 로드 방지)</summary>
        private string _currentLoadingIconID;

        /// <summary>
        /// 이 패널이 선택 상태인지 반환한다.
        /// </summary>
        /// <returns>선택 상태이면 true</returns>
        public bool IsSelected()
        {
            return _toggle.isOn;
        }

        /// <summary>
        /// 스킬 설명 텍스트를 설정한다.
        /// </summary>
        /// <param name="value">설명 문자열</param>
        public void SetDescription(string value)
        {
            _descriptionText.text = value;
        }

        /// <summary>
        /// 스킬 아이콘을 비동기로 로드하여 설정한다. 동일 아이콘 중복 로드를 방지한다.
        /// </summary>
        /// <param name="iconID">로드할 아이콘 에셋 ID</param>
        public async void SetIcon(string iconID)
        {
            if (_currentLoadingIconID == iconID)
                return;

            _currentLoadingIconID = iconID;

            var iconSprite = await LoadIcon(iconID);
            if (iconSprite == null)
                return;

            if (_iconImage && _currentLoadingIconID == iconID)
                _iconImage.sprite = iconSprite;
        }

        /// <summary>
        /// 아이콘 스프라이트를 비동기로 로드한다. 이전 로드 작업이 있으면 취소한다.
        /// </summary>
        /// <param name="iconID">로드할 아이콘 에셋 ID</param>
        /// <returns>로드된 스프라이트. 취소되거나 실패하면 null</returns>
        private async System.Threading.Tasks.Task<Sprite> LoadIcon(string iconID)
        {
            ResetLoadIconToken();

            _loadIconToken = new CancellationTokenSource();
            CancellationToken token = _loadIconToken.Token;

            try
            {
                return await AssetLoader.LoadAsset<Sprite>(iconID, token);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (System.Exception ex)
            {
                Logger.Warn($"Failed to load icon ({iconID}): {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 현재 진행 중인 아이콘 로드 작업을 취소하고 토큰을 해제한다.
        /// </summary>
        private void ResetLoadIconToken()
        {
            if (_loadIconToken == null)
                return;

            _loadIconToken.Cancel();
            _loadIconToken.Dispose();
            _loadIconToken = null;
        }

        private void OnDestroy()
        {
            ResetLoadIconToken();
        }

        /// <summary>
        /// 스킬 레벨 텍스트를 설정한다.
        /// </summary>
        /// <param name="value">레벨 문자열</param>
        public void SetLevel(string value)
        {
            _levelText.text = value;
        }

        /// <summary>
        /// 스킬 이름 텍스트를 설정한다.
        /// </summary>
        /// <param name="value">이름 문자열</param>
        public void SetName(string value)
        {
            _nameText.text = value;
        }

        /// <summary>
        /// 이 패널을 선택 상태로 설정한다.
        /// </summary>
        public void SetSelected()
        {
            _toggle.isOn = true;
        }
    }
}