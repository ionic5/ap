using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 유닛 로직 객체를 생성하는 팩토리 클래스.
    /// 로직 ID에 따라 PlayerUnitLogic, NonPlayerUnitLogic, MonkLogic 등의 인스턴스를 생성한다.
    /// </summary>
    public class UnitLogicFactroy
    {
        /// <summary>조이스틱 입력 인터페이스</summary>
        private readonly IJoystick _joystick;
        /// <summary>소울 검색 인터페이스</summary>
        private readonly ISoulFinder _soulFinder;
        /// <summary>월드 인터페이스</summary>
        private readonly Core.View.BattleFieldScene.IWorld _world;
        /// <summary>로직 ID별 생성 함수 딕셔너리</summary>
        private readonly Dictionary<string, Func<IControllableUnit, IUnitLogic>> _creationFunction;
        /// <summary>게임 데이터 저장소</summary>
        private readonly GameDataStore _gameDataStore;
        /// <summary>게임 루프</summary>
        private readonly ILoop _loop;
        /// <summary>로거</summary>
        private readonly ILogger _logger;
        /// <summary>타이머 생성 팩토리 함수</summary>
        private readonly Func<Core.Timer> _createTimer;

        /// <summary>
        /// UnitLogicFactroy의 생성자.
        /// </summary>
        /// <param name="joystick">조이스틱 입력 인터페이스</param>
        /// <param name="world">월드 인터페이스</param>
        /// <param name="createTimer">타이머 생성 팩토리 함수</param>
        /// <param name="loop">게임 루프</param>
        /// <param name="soulFinder">소울 검색 인터페이스</param>
        /// <param name="gameDataStore">게임 데이터 저장소</param>
        /// <param name="logger">로거</param>
        public UnitLogicFactroy(IJoystick joystick, View.BattleFieldScene.IWorld world,
            Func<Timer> createTimer, ILoop loop, ISoulFinder soulFinder, GameDataStore gameDataStore, ILogger logger)
        {
            _joystick = joystick;
            _world = world;
            _createTimer = createTimer;
            _loop = loop;
            _soulFinder = soulFinder;
            _gameDataStore = gameDataStore;
            _logger = logger;

            _creationFunction = new Dictionary<string, Func<IControllableUnit, IUnitLogic>>{
                { "PLAYER", (unit) =>  new PlayerUnitLogic(_loop, _joystick, _soulFinder, _gameDataStore) },
                { "NON_PLAYER", (unit) =>  new NonPlayerUnitLogic(_loop, _createTimer.Invoke(), _world) },
                { "MONK", (unit) =>  new MonkLogic(_loop, new Core.Random()) }
            };
        }

        /// <summary>
        /// 지정된 로직 ID에 해당하는 유닛 로직을 생성한다.
        /// </summary>
        /// <param name="unit">제어할 유닛</param>
        /// <param name="logicID">유닛 로직 ID ("PLAYER", "NON_PLAYER", "MONK" 등)</param>
        /// <returns>생성된 유닛 로직. 해당 ID가 없으면 null</returns>
        public IUnitLogic Create(IControllableUnit unit, string logicID)
        {
            if (_creationFunction.TryGetValue(logicID, out var creationFunction))
                return creationFunction.Invoke(unit);

            _logger.Warn($"Failed to find logic for logic id ({logicID})");
            return null;
        }
    }
}
