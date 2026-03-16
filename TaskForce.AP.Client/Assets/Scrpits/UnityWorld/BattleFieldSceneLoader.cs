using System;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;
using TaskForce.AP.Client.UnityWorld.AssetData;
using TaskForce.AP.Client.UnityWorld.BattleFieldScene;
using TaskForce.AP.Client.UnityWorld.View;
using TaskForce.AP.Client.UnityWorld.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 전투 씬(BattleFieldScene)의 로딩을 담당하는 클래스.
    /// 씬 전환, 팩토리 구성, 스킬 생성자 등록, 유닛 생성, 이벤트 바인딩 등
    /// 전투 씬에 필요한 모든 초기화 작업을 수행한다.
    /// </summary>
    public class BattleFieldSceneLoader
    {
        /// <summary>화면 전환 및 로딩 블라인드를 관리하는 Screen 인스턴스</summary>
        private readonly Screen _screen;
        /// <summary>게임 데이터 저장소</summary>
        private readonly GameDataStore _gameDataStore;
        /// <summary>난수 생성기</summary>
        private readonly Core.Random _random;
        /// <summary>시간 관리 인스턴스</summary>
        private readonly Time _time;
        /// <summary>텍스트 저장소</summary>
        private readonly TextStore _textStore;
        /// <summary>에셋 로더</summary>
        private readonly AssetLoader _assetLoader;
        /// <summary>로거 인스턴스</summary>
        private readonly Core.ILogger _logger;

        /// <summary>
        /// BattleFieldSceneLoader의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="screen">화면 전환 관리 객체</param>
        /// <param name="gameDataStore">게임 데이터 저장소</param>
        /// <param name="random">난수 생성기</param>
        /// <param name="time">시간 관리 객체</param>
        /// <param name="textStore">텍스트 저장소</param>
        /// <param name="assetLoader">에셋 로더</param>
        /// <param name="logger">로거 인스턴스</param>
        public BattleFieldSceneLoader(Screen screen, GameDataStore gameDataStore,
            Core.Random random, Time time, TextStore textStore,
            AssetLoader assetLoader, Core.ILogger logger)
        {
            _screen = screen;
            _gameDataStore = gameDataStore;
            _random = random;
            _time = time;
            _textStore = textStore;
            _assetLoader = assetLoader;
            _logger = logger;
        }

        /// <summary>
        /// 전투 씬을 비동기적으로 로드한다.
        /// 로딩 블라인드 표시, 기존 씬 파괴, 새 씬 부착, 각종 팩토리 및 시스템 초기화,
        /// 스킬 생성자 등록, 이벤트 핸들러 바인딩, 적 유닛 스포너 시작 등을 순차적으로 수행한다.
        /// </summary>
        public async void Load()
        {
            await _screen.ShowLoadingBlind();
            await _screen.DestroyLastScene();

            var instance = await _screen.AttachNewScene(SceneID.BattleFieldScene);

            var scene = instance.GetComponent<View.Scenes.BattleFieldScene>();

            var pathFinder = new PathFinder(scene.TileMapGrid, LayerMask.GetMask("CollisionTile"));
            var objFac = scene.ObjectFactory;
            var loop = scene.Loop;
            var world = scene.World;
            var joystick = new BattleFieldScene.Joystick(scene.Joystick);
            var targetFinder = new BattleFieldScene.TargetFinder();
            var soulFinder = new SoulFinder();
            var playerUnitSpawnPosition = scene.PlayerUnitSpawnPosition;
            var followCamera = scene.FollowCamera;
            var expBar = scene.ExpBar;
            var levelText = scene.LevelText;
            Func<Timer> createTimer = () => new Timer(_time, loop);

            Func<FloatingTextAnimator> createFloatingTextAnimator = () => objFac.Create<UnityWorld.View.FloatingTextAnimator>(ObjectID.FloatingTextAnimator);

            objFac.Logger = _logger;

            Action<UnityWorld.Object> unitPrepareHdlr = (go) =>
            {
                var unit = go.GetComponent<UnityWorld.View.BattleFieldScene.Unit>();

                unit.gameObject.name = Guid.NewGuid().ToString();
                unit.CreateFloatingTextAnimator = createFloatingTextAnimator;
                unit.Timer = createTimer();
            };
            objFac.RegisterPrepareHandler("WARRIOR_0", unitPrepareHdlr);
            objFac.RegisterPrepareHandler("WARRIOR_1", unitPrepareHdlr);
            objFac.RegisterPrepareHandler("MONK", unitPrepareHdlr);

            world.Random = _random;

            var formulaCalculator = new FormulaCalculator(_logger);
            formulaCalculator.RegisterFormula("FORMULA_0", (coeffs, variables) => { return coeffs["a"]; });
            formulaCalculator.RegisterFormula("FORMULA_2", (coeffs, variables) => { return coeffs["a"] + variables[0]; });

            var effectFactory = new TaskForce.AP.Client.Core.Entity.ModifyAttributeEffectFactory(_gameDataStore, formulaCalculator);
            var skillFactory = new SkillFactory();
            var unitLogicFactory = new UnitLogicFactroy(joystick, world, createTimer, loop, soulFinder, _gameDataStore, _logger);
            var soulFactory = new SoulFactory(() => objFac.Create<View.BattleFieldScene.Soul>(ObjectID.Soul));
            var dropHandler = new DropHandler(soulFactory, _random, _gameDataStore);
            var skillEntityFactory = new TaskForce.AP.Client.Core.Entity.SkillFactory(_gameDataStore, _logger, _textStore, effectFactory);
            var unitFactory = new UnitFactory(_random, createTimer, targetFinder,
                (id) => objFac.Create<View.BattleFieldScene.Unit>(id), _logger,
                skillFactory.Create, _gameDataStore, unitLogicFactory.Create, skillEntityFactory.CreateSkill);
            var unitEntityFactory = new Core.Entity.UnitFactory(_logger, _gameDataStore, skillEntityFactory.CreateSkill);

            skillFactory.AddCreator(Core.Entity.SkillID.Monk, (skill) =>
            {
                return new MonkSkill(skill, unitFactory.CreateNonPlayerUnit);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.SheepMissile, (skill) =>
            {
                return new SheepMissileSkill(_random, new RepeatTimer(createTimer()),
                    createTimer(), (int minDmg, int maxDmg) =>
                    {
                        var view = objFac.Create<Sheep>(ObjectID.SheepMissile);
                        return new SheepMissile(_random, view, minDmg, maxDmg, targetFinder);
                    }, skill);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.Dynamite, (skill) =>
            {
                return new DynamiteSkill(_random, new RepeatTimer(createTimer()),
                    createTimer(), (IUnit caster, int minDmg, int maxDmg, float explosionRadius) =>
                    {
                        var view = objFac.Create<Sheep>(ObjectID.Dynamite);
                        return new Dynamite(view, caster,
                        minDmg, maxDmg, explosionRadius, (IUnit caster, int minDmg, int maxDmg, float explosionRadius) =>
                        {
                            var view = objFac.Create<View.BattleFieldScene.Explosion>(ObjectID.Explosion0);
                            return new Core.BattleFieldScene.Skills.Explosion(view, caster, _random, minDmg, maxDmg, explosionRadius);
                        });
                    }, skill);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.PowderKeg, (skill) =>
            {
                return new PowderKegSkill(skill, createTimer(), (IUnit caster,
                    int minDmg, int maxDmg, float watchRadius, float explosionRadius, float expireTime) =>
                {
                    var view = objFac.Create<View.BattleFieldScene.PowderKeg>(ObjectID.PowderKeg);
                    return new Core.BattleFieldScene.Skills.PowderKeg(view, caster, createTimer(), minDmg, maxDmg,
                        watchRadius, explosionRadius, expireTime, (IUnit caster, int minDmg, int maxDmg, float explosionRadius) =>
                        {
                            var view = objFac.Create<View.BattleFieldScene.Explosion>(ObjectID.Explosion1);
                            return new Core.BattleFieldScene.Skills.Explosion(view, caster, _random, minDmg, maxDmg, explosionRadius);
                        });
                });
            });
            skillFactory.AddCreator(Core.Entity.SkillID.Heal, (skill) =>
            {
                return new HealSkill(createTimer(), () => objFac.Create<OneShotEffect>(ObjectID.HealEffect), skill);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.MeleeAttack, (skill) =>
            {
                return new Core.BattleFieldScene.Skills.MeleeAttackSkill(createTimer(), skill, _random);
            });

            soulFactory.SoulCreatedEvent += soulFinder.OnSoulCreatedEvent;
            unitFactory.UnitCreatedEvent += targetFinder.OnTargetCreatedEvent;
            unitFactory.UnitCreatedEvent += dropHandler.OnUnitCreatedEvent;

            EventHandler<DestroyEventArgs> hdlr = null;
            hdlr = (sender, args) =>
            {
                soulFactory.SoulCreatedEvent -= soulFinder.OnSoulCreatedEvent;
                unitFactory.UnitCreatedEvent -= targetFinder.OnTargetCreatedEvent;
                unitFactory.UnitCreatedEvent -= dropHandler.OnUnitCreatedEvent;

                targetFinder.Destory();

                scene.DestroyEvent -= hdlr;
            };
            scene.DestroyEvent += hdlr;

            var windowStack = scene.WindowStack;

            var window = windowStack.SkillSelectionWindow;
            foreach (var panel in window.SkillPanels)
            {
                panel.AssetLoader = _assetLoader;
                panel.Logger = _logger;
            }

            var winOpener = new WindowOpener(windowStack, _textStore, _logger);

            var sceneCtrl = new BattleFieldSceneController(scene, world, followCamera, winOpener,
                unitFactory.CreatePlayerUnit, _gameDataStore, _random, _logger,
                skillEntityFactory.CreateSkillEntity,
                unitEntityFactory.CreateUnitEntity);
            sceneCtrl.Start();

            var spawner = new EnemyUnitSpawner(world, _gameDataStore, new Core.Timer(_time, loop),
                _logger, _random, unitFactory.CreateEnemyUnit);
            loop.Add(spawner);

            spawner.Start(1);

            _screen.HideLoadingBlind();
        }
    }
}
