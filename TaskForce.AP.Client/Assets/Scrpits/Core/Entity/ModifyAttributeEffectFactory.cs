using System.Collections.Generic;
using System.Linq;

namespace TaskForce.AP.Client.Core.Entity
{
    public class ModifyAttributeEffectFactory
    {
        private readonly GameDataStore _gameDataStore;
        private readonly FormulaCalculator _formulaCalculator;

        public ModifyAttributeEffectFactory(GameDataStore gameDataStore, FormulaCalculator formulaCalculator)
        {
            _gameDataStore = gameDataStore;
            _formulaCalculator = formulaCalculator;
        }

        public IModifyAttributeEffect Create(string effectID, int level)
        {
            var effectData = _gameDataStore.GetModifyAttributeEffects().FirstOrDefault(entry => entry.ID == effectID);
            var coeffcients = CreateCoeffcients(level, effectData);

            return new ModifyAttributeEffect(effectData.ApplyOrder, effectData.AttributeSetID,
                effectData.CalculationType, coeffcients, _formulaCalculator);
        }

        private Dictionary<string, float> CreateCoeffcients(int level, GameData.ModifyAttributeEffect effectData)
        {
            var formulas = _gameDataStore.GetCoefficientFormulasBySetID(effectData.CoefficientFormulaSetID);
            var coeffcients = new Dictionary<string, float>();
            foreach (var entry in formulas)
                coeffcients[entry.Key] = CalculateCoeffcient(entry.Value, level);
            return coeffcients;
        }

        private float CalculateCoeffcient(GameData.Formula formula, int level)
        {
            var coeffs = _gameDataStore.GetCoefficientByFormulaID(formula.ID);
            var value = _formulaCalculator.Calculate(formula.CalculationType, coeffs, level);
            return value;
        }
    }
}
