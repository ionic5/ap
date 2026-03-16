using TaskForce.AP.Client.Core.View.Scenes;
using TaskForce.AP.Client.UnityWorld.BattleFieldScene;
using TaskForce.AP.Client.UnityWorld.View.BattleFieldScene;
using TMPro;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.Scenes
{
    /// <summary>
    /// 전장 씬의 루트 MonoBehaviour 클래스.
    /// 씬 내 주요 컴포넌트(루프, 조이스틱, 월드, UI 등)에 대한 참조를 관리하고,
    /// 경험치/레벨 UI 설정 기능을 제공한다.
    /// </summary>
    public class BattleFieldScene : Scene, IBattleFieldScene
    {
        /// <summary>게임 루프 컴포넌트</summary>
        [SerializeField]
        private Loop _loop;
        /// <summary>조이스틱 입력 컴포넌트</summary>
        [SerializeField]
        private global::Joystick _joystick;
        /// <summary>플레이어 유닛 스폰 위치 오브젝트</summary>
        [SerializeField]
        private GameObject _playerUnitSpawnPosition;
        /// <summary>타일맵 그리드 컴포넌트</summary>
        [SerializeField]
        private TilemapGrid _grid;
        /// <summary>오브젝트 팩토리</summary>
        [SerializeField]
        public ObjectFactory ObjectFactory;
        /// <summary>카메라 추적 컴포넌트</summary>
        [SerializeField]
        private FollowCamera _followCamera;
        /// <summary>월드 관리 컴포넌트</summary>
        [SerializeField]
        private World _world;
        /// <summary>경험치 게이지 바 UI</summary>
        [SerializeField]
        private GaugeBar _expBar;
        /// <summary>레벨 표시 텍스트 UI</summary>
        [SerializeField]
        private TMP_Text _levelText;
        /// <summary>전장 씬 전용 윈도우 스택</summary>
        [SerializeField]
        private View.BattleFieldScene.WindowStack _windowStack;

        /// <summary>타일맵 그리드 접근 프로퍼티</summary>
        public TilemapGrid TileMapGrid => _grid;
        /// <summary>게임 루프 접근 프로퍼티</summary>
        public Loop Loop => _loop;
        /// <summary>월드 접근 프로퍼티</summary>
        public World World => _world;
        /// <summary>조이스틱 접근 프로퍼티</summary>
        public Joystick Joystick => _joystick;
        /// <summary>플레이어 스폰 위치 접근 프로퍼티</summary>
        public GameObject PlayerUnitSpawnPosition => _playerUnitSpawnPosition;
        /// <summary>추적 카메라 접근 프로퍼티</summary>
        public FollowCamera FollowCamera => _followCamera;
        /// <summary>경험치 바 접근 프로퍼티</summary>
        public GaugeBar ExpBar => _expBar;
        /// <summary>레벨 텍스트 접근 프로퍼티</summary>
        public TMP_Text LevelText => _levelText;
        /// <summary>윈도우 스택 접근 프로퍼티</summary>
        public View.BattleFieldScene.WindowStack WindowStack => _windowStack;

        /// <summary>
        /// 현재 경험치 값을 설정한다.
        /// </summary>
        /// <param name="v">경험치 값</param>
        public void SetExp(int v)
        {
            _expBar.SetValue(v);
        }

        /// <summary>
        /// 현재 레벨을 표시한다.
        /// </summary>
        /// <param name="v">레벨 문자열</param>
        public void SetLevel(string v)
        {
            _levelText.text = v;
        }

        /// <summary>
        /// 레벨업에 필요한 경험치 최대값을 설정한다.
        /// </summary>
        /// <param name="v">필요 경험치 값</param>
        public void SetRequireExp(int v)
        {
            _expBar.SetMaxValue(v);
        }
    }
}
