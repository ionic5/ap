using UnityEngine;
using UnityEngine.UI;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 체력바, 경험치바 등의 게이지 UI를 표시하는 MonoBehaviour 컴포넌트.
    /// Image의 fillAmount를 사용하여 현재 값과 최대 값의 비율을 시각적으로 나타낸다.
    /// </summary>
    public class GaugeBar : MonoBehaviour
    {
        /// <summary>현재 게이지 값</summary>
        private int _value;
        /// <summary>게이지 최대 값</summary>
        private int _maxValue;

        /// <summary>게이지를 표시하는 UI Image 컴포넌트</summary>
        [SerializeField]
        private Image _image;

        /// <summary>
        /// 게이지의 현재 값을 설정하고 UI를 갱신한다.
        /// </summary>
        /// <param name="value">설정할 현재 값</param>
        public void SetValue(int value)
        {
            _value = value;
            UpdateGaugeBar();
        }

        /// <summary>
        /// 게이지의 최대 값을 설정하고 UI를 갱신한다.
        /// </summary>
        /// <param name="value">설정할 최대 값</param>
        public void SetMaxValue(int value)
        {
            _maxValue = value;
            UpdateGaugeBar();
        }

        /// <summary>
        /// 게이지 바의 표시 여부를 설정한다.
        /// </summary>
        /// <param name="v">true이면 표시, false이면 숨김</param>
        public void SetVisible(bool v)
        {
            gameObject.SetActive(v);
        }

        /// <summary>
        /// 현재 값과 최대 값의 비율에 따라 게이지 바의 fillAmount를 갱신한다.
        /// </summary>
        public void UpdateGaugeBar()
        {
            if (_maxValue <= 0)
                return;

            if (_value < 0)
                _value = 0;

            _image.fillAmount = _value / (float)_maxValue;
        }
    }
}
