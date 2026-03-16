using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 적 유닛을 주기적으로 스폰하는 클래스.
    /// 스테이지 레벨에 따라 적 유닛 종류와 최대 수를 결정하며,
    /// 스테이지 시간이 경과하면 다음 스테이지로 자동 전환한다.
    /// </summary>
    public class EnemyUnitSpawner : IUpdatable
    {
        /// <summary>월드 인터페이스 (워프 포인트 등 제공)</summary>
        private readonly View.BattleFieldScene.IWorld _world;
        /// <summary>게임 데이터 저장소</summary>
        private readonly GameDataStore _gameDataStore;
        /// <summary>스테이지 타이머</summary>
        private readonly Core.Timer _timer;
        /// <summary>로거</summary>
        private readonly Core.ILogger _logger;
        /// <summary>난수 생성기</summary>
        private readonly Core.Random _random;
        /// <summary>유닛 생성 팩토리 함수</summary>
        private readonly Func<string, int, IUnit> _createUnit;

        /// <summary>현재 활성 상태인 적 유닛 목록</summary>
        private List<IUnit> _activeEnemyUnits;
        /// <summary>현재 스테이지의 적 유닛 데이터 목록</summary>
        private IReadOnlyList<StageEnemyUnit> _stageEnemyUnits;
        /// <summary>동시 존재 가능한 최대 적 유닛 수</summary>
        private int _maxEnemyUnitCount;
        /// <summary>현재 스테이지 레벨</summary>
        private int _stageLevel;

        /// <summary>
        /// EnemyUnitSpawner의 생성자.
        /// </summary>
        /// <param name="world">월드 인터페이스</param>
        /// <param name="gameDataStore">게임 데이터 저장소</param>
        /// <param name="timer">타이머</param>
        /// <param name="logger">로거</param>
        /// <param name="random">난수 생성기</param>
        /// <param name="createUnit">유닛 생성 팩토리 함수</param>
        public EnemyUnitSpawner(View.BattleFieldScene.IWorld world, GameDataStore gameDataStore,
            Timer timer, ILogger logger, Random random, Func<string, int, IUnit> createUnit)
        {
            _world = world;
            _gameDataStore = gameDataStore;
            _activeEnemyUnits = new List<IUnit>();
            _timer = timer;
            _logger = logger;
            _random = random;
            _createUnit = createUnit;
        }

        /// <summary>
        /// 지정된 스테이지 레벨로 적 유닛 스폰을 시작한다.
        /// </summary>
        /// <param name="stageLevel">시작할 스테이지 레벨</param>
        public void Start(int stageLevel)
        {
            _logger.Info($"Stage(level:{stageLevel}) started.");

            _stageLevel = stageLevel;

            var stage = GetStage(stageLevel);
            _timer.Start(0, stage.Time, OnStageFinished);
            _stageEnemyUnits = _gameDataStore.GetStageEnemyUnits().Where(entry => entry.StageLevel == stageLevel).ToList();
            _maxEnemyUnitCount = stage.MaxEnemyUnitCount;
        }

        private Stage GetStage(int stageLevel)
        {
            var stage = _gameDataStore.GetStages().Where(entry => entry.Level == stageLevel).FirstOrDefault();
            if (stage == null)
                _logger.Fatal($"Stage not exist for {stageLevel}");
            return stage;
        }

        private bool IsStageExist(int stageLevel)
        {
            return _gameDataStore.GetStages().Any(entry => entry.Level == stageLevel);
        }

        private void OnStageFinished()
        {
            var nextLevel = _stageLevel + 1;
            if (IsStageExist(nextLevel))
                Start(nextLevel);
            else
                _timer.Stop(0);
        }

        /// <summary>
        /// 매 프레임 호출되어 최대 적 유닛 수 미만일 경우 새로운 적 유닛을 스폰한다.
        /// </summary>
        public void Update()
        {
            if (!_timer.IsRunning(0))
                return;

            if (_activeEnemyUnits.Count >= _maxEnemyUnitCount)
                return;

            var mob = _stageEnemyUnits[_random.Next(0, _stageEnemyUnits.Count)];
            Spawn(mob.UnitID, mob.Level);
        }

        private void Spawn(string unitID, int level)
        {
            _logger.Info($"Enemy unit spawned. UnitID : {unitID} Level : {level}");

            var unit = _createUnit.Invoke(unitID, level);

            _activeEnemyUnits.Add(unit);
            unit.DestroyEvent += OnDestroyUnitEvent;
            unit.DiedEvent += OnUnitDiedEvent;

            var spawnPos = _world.GetWarpPoint();
            unit.SetPosition(spawnPos);
        }

        private void OnUnitDiedEvent(object sender, DiedEventArgs e)
        {
            var unit = _activeEnemyUnits.FirstOrDefault(u => ReferenceEquals(u, e.DiedTarget));
            if (unit == null)
                return;
            RemoveUnit(unit);
        }

        private void OnDestroyUnitEvent(object sender, DestroyEventArgs e)
        {
            var unit = _activeEnemyUnits.FirstOrDefault(u => ReferenceEquals(u, e.TargetObject));
            if (unit == null)
                return;
            RemoveUnit(unit);
        }

        private void RemoveUnit(IUnit unit)
        {
            if (!_activeEnemyUnits.Remove(unit))
                return;

            unit.DiedEvent -= OnUnitDiedEvent;
            unit.DestroyEvent -= OnDestroyUnitEvent;
        }
    }
}
