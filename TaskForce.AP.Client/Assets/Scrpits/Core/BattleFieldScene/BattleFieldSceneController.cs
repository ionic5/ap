using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.Scenes;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class BattleFieldSceneController
    {
        private readonly IBattleFieldScene _scene;
        private readonly IWorld _world;
        private readonly IFollowCamera _followCamera;
        private readonly WindowOpener _windowOpener;
        private readonly GameDataStore _gameDataStore;
        private readonly Func<Entity.Unit, IUnit> _createPlayerUnit;
        private readonly Core.Random _random;
        private readonly ILogger _logger;
        private readonly Func<string, Entity.Unit> _createUnitEntity;
        private readonly Func<Entity.Unit, string, int, Entity.ISkill> _createSkillEntity;

        private bool _isDestroyed;
        private IUnit _unit;
        private Entity.Unit _unitEntity;

        public BattleFieldSceneController(IBattleFieldScene scene, IWorld world, IFollowCamera followCamera,
            WindowOpener windowOpener, Func<Entity.Unit, IUnit> createUnit,
            GameDataStore gameDataStore, Random random, ILogger logger,
            Func<Entity.Unit, string, int, Entity.ISkill> createSkillEntity,
            Func<string, Entity.Unit> createUnitEntity)
        {
            _scene = scene;
            _world = world;
            _followCamera = followCamera;
            _createPlayerUnit = createUnit;
            _windowOpener = windowOpener;
            _isDestroyed = false;
            _gameDataStore = gameDataStore;
            _random = random;
            _logger = logger;
            _createSkillEntity = createSkillEntity;
            _createUnitEntity = createUnitEntity;
        }

        public void Start()
        {
            _unitEntity = _createUnitEntity.Invoke("WARRIOR_0");

            _unit = _createPlayerUnit(_unitEntity);
            _unit.SetPosition(_world.GetPlayerUnitSpawnPosition());

            UpdateLevel();
            UpdateRequireExp();
            UpdateExp();

            _unit.RequireExpChangedEvent += OnRequireExpChnagedEvent;
            _unit.ExpChangedEvent += OnExpChnagedEvent;
            _unit.LevelUpEvent += OnLevelUpEvent;
            _unit.DestroyEvent += OnUnitDestroyEvent;

            _followCamera.SetTarget(_unit);

            _scene.DestroyEvent += OnDestroySceneEvent;
        }

        private void OnLevelUpEvent(object sender, EventArgs e)
        {
            UpdateLevel();

            var skillIDs = _gameDataStore.GetLevelUpRewardSkills().Select(entry => entry.SkillID).ToArray();
            _random.Shuffle(skillIDs);
            var newSkills = new List<Entity.ISkill>();

            foreach (var skillID in skillIDs.Take(3))
            {
                var skill = _unitEntity.GetSkill(skillID);
                var level = skill != null ? skill.GetLevel() + 1 : 1;
                newSkills.Add(_createSkillEntity.Invoke(_unitEntity, skillID, level));
            }

            _windowOpener.OpenPerkSelectionWindow(_unitEntity, newSkills);
        }

        //private IEnumerable<string> GetPerkIDs()
        //{
        //    var ownedPerks = new Dictionary<string, int>();
        //    foreach (var perk in _unit.GetPerks())
        //    {
        //        var perkID = perk.GetPerkID();
        //        var level = perk.GetLevel();
        //        if (ownedPerks.TryGetValue(perkID, out var max))
        //            if (level > max)
        //                ownedPerks[perkID] = level;
        //            else
        //                ownedPerks[perkID] = level;
        //    }

        //    var upgradeableEffects = _gameDataStore.GetEffects()
        //        .Where(entry =>
        //        {
        //            if (ownedPerks.TryGetValue(entry.PerkID, out var maxLevel))
        //                return entry.Level == maxLevel + 1;
        //            else
        //                return entry.Level == 1;
        //        });

        //    return upgradeableEffects.Select(entry => entry.PerkID).Distinct();
        //}

        //private Entity.Perk SelectPerk(IEnumerable<string> perkIDs)
        //{
        //    var index = _random.Next(perkIDs.Count());
        //    //var perkID = perkIDs.ElementAt(index);
        //    var perkID = "MONK";

        //    var targetPerk = _gameDataStore.GetPerks().FirstOrDefault(entry => entry.ID == perkID);

        //    PerkEffect targetPerkEffect = null;
        //    var perks = _unit.GetPerks().Where(entry => entry.GetPerkID() == perkID);
        //    if (perks.Any())
        //    {
        //        var level = _unit.GetPerks().Where(entry => entry.GetPerkID() == perkID).Max(entry => entry.GetLevel());
        //        targetPerkEffect = _gameDataStore.GetEffects().Where(entry => entry.PerkID == perkID && entry.Level > level).OrderBy(entry => entry.Level).FirstOrDefault();
        //    }
        //    else
        //        targetPerkEffect = _gameDataStore.GetEffects().Where(entry => entry.PerkID == perkID).OrderBy(entry => entry.Level).FirstOrDefault();

        //    var perk = new Entity.Perk();
        //    perk.SetID(targetPerk.ID);
        //    perk.SetLevel(targetPerkEffect.Level);
        //    perk.SetNameTextID(targetPerk.NameTextID);
        //    perk.SetDescTextID(targetPerkEffect.DescTextID);
        //    perk.SetIconID(targetPerk.IconID);

        //    if (targetPerkEffect.EffectType == PerkEffectType.AddSkill)
        //    {
        //        var data = _gameDataStore.GetAddSkillEffects().FirstOrDefault(entry => entry.PerkEffectID == targetPerkEffect.ID);
        //        var attributes = new Dictionary<string, Attribute>();
        //        foreach (var entry in _gameDataStore.GetSkillAttributes().Where(entry => entry.SkillID == data.SkillID))
        //            attributes.Add(entry.AttributeID, entry.Value);
        //        var attributeSet = new AttributeSet(_gameDataStore, _logger);
        //        attributeSet.SetBaseAttributes(attributes);
        //        var entity = new Entity.CommonSkill(data.SkillID, attributeSet);

        //        var effect = new Entity.Effects.AddSkillEffect(entity);

        //        perk.AddPerkEffect(effect);
        //    }

        //    if (targetPerkEffect.EffectType == PerkEffectType.ModifySkillAttribute)
        //    {
        //        var datas = _gameDataStore.GetModifySkillAttributeEffects().Where(entry => entry.PerkEffectID == targetPerkEffect.ID);
        //        foreach (var data in datas)
        //        {
        //            var coeffs = _gameDataStore.GetCoefficients().Where(entry => entry.FormulaID == data.CoefficientID)
        //                .Select(entry => entry.Value).ToList();
        //            var effect = new Entity.Effects.ModifySkillAttributeEffect(data.SkillID, new Entity.Effects.ModifyAttributeEffect(data.CalculateType, data.SkillID, coeffs)); ;

        //            perk.AddPerkEffect(effect);
        //        }
        //    }

        //    if (targetPerkEffect.EffectType == PerkEffectType.ModifyAttribute)
        //    {
        //        var datas = _gameDataStore.GetModifyAttributeEffects().Where(entry => entry.PerkEffectID == targetPerkEffect.ID);
        //        foreach (var data in datas)
        //        {
        //            var coeffs = _gameDataStore.GetCoefficients().Where(entry => entry.FormulaID == data.CoefficientID)
        //                .Select(entry => entry.Value).ToList();
        //            var effect = new Entity.Effects.ModifyAttributeEffect(data.CalculateType, data.AttributeID, coeffs); ;

        //            perk.AddPerkEffect(effect);
        //        }
        //    }

        //    return perk;
        //}

        private void OnExpChnagedEvent(object sender, EventArgs e)
        {
            UpdateExp();
        }

        private void OnRequireExpChnagedEvent(object sender, EventArgs e)
        {
            UpdateRequireExp();
        }

        private void UpdateLevel()
        {
            _scene.SetLevel(_unit.GetLevel().ToString());
        }

        private void UpdateExp()
        {
            _scene.SetExp(_unit.GetExp());
        }

        private void UpdateRequireExp()
        {
            _scene.SetRequireExp(_unit.GetRequireExp());
        }

        private void OnDestroySceneEvent(object sender, EventArgs e)
        {
            Destroy();
        }

        private void OnUnitDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _followCamera.UnsetTarget();

            _unit.RequireExpChangedEvent -= OnRequireExpChnagedEvent;
            _unit.ExpChangedEvent -= OnExpChnagedEvent;
            _unit.LevelUpEvent -= OnLevelUpEvent;
            _unit.DestroyEvent -= OnUnitDestroyEvent;
            _unit.Destroy();
            _unit = null;
        }
    }
}
