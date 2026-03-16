using System.Collections.Generic;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View
{
    /// <summary>
    /// 윈도우 스택을 관리하는 MonoBehaviour 클래스.
    /// 열린 윈도우들을 스택 형태로 관리하며, 배경 오브젝트의 표시/위치를 자동 갱신한다.
    /// </summary>
    public class WindowStack : MonoBehaviour
    {
        /// <summary>현재 열려 있는 윈도우 목록</summary>
        private List<Window> _windows;

        /// <summary>윈도우 뒤에 표시되는 배경 게임오브젝트</summary>
        [SerializeField]
        private GameObject _windowBackground;

        void Awake()
        {
            _windows = new List<Window>();
        }

        /// <summary>
        /// 윈도우를 열어 스택에 추가한다. 닫힘 시 자동으로 스택에서 제거된다.
        /// </summary>
        /// <typeparam name="T">Window를 상속하는 윈도우 타입</typeparam>
        /// <param name="window">열 윈도우 인스턴스</param>
        /// <returns>열린 윈도우 인스턴스</returns>
        public T OpenWindow<T>(T window) where T : Window
        {
            window.transform.SetAsLastSibling();
            window.gameObject.SetActive(true);
            _windows.Add(window);

            UpdateWindowBackground();

            window.ClosedEvent += (sender, args) =>
            {
                window.transform.SetSiblingIndex(0);
                _windows.Remove(window);

                UpdateWindowBackground();
            };

            return window;
        }

        /// <summary>
        /// 윈도우 배경의 활성화 상태와 형제 순서를 최상위 윈도우에 맞춰 갱신한다.
        /// </summary>
        private void UpdateWindowBackground()
        {
            if (!IsOpenedWindowExist())
            {
                _windowBackground.SetActive(false);
                return;
            }

            _windowBackground.SetActive(true);

            var lastWindow = _windows[_windows.Count - 1];
            int index = lastWindow.transform.GetSiblingIndex();
            _windowBackground.transform.SetSiblingIndex(index - 1);
        }

        /// <summary>
        /// 현재 열려 있는 윈도우가 존재하는지 확인한다.
        /// </summary>
        /// <returns>열린 윈도우가 있으면 true</returns>
        private bool IsOpenedWindowExist()
        {
            return _windows.Count > 0;
        }
    }
}