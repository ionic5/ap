using System.Collections.Generic;
using System.Linq;

namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 속성 변경 효과(ModifyAttributeEffect)를 생성하는 팩토리 클래스.
    /// 게임 데이터에서 효과 정보를 조회하고 레벨에 맞는 계수를 계산하여 효과 인스턴스를 생성한다.
    /// </summary>
    public class ModifyAttributeEffectFactory
    {
        /// <summary>게임 데이터 저장소.</summary>
        private readonly GameDataStore _gameDataStore;

        /// <summary>공식 계산기 인스턴스.</summary>
        private readonly FormulaCalculator _formulaCalculator;

        /// <summary>
        /// ModifyAttributeEffectFactory의 생성자.
        /// </summary>
        /// <param name="gameDataStore">게임 데이터 저장소.</param>
        /// <param name="formulaCalculator">공식 계산기.</param>
        public ModifyAttributeEffectFactory(GameDataStore gameDataStore, FormulaCalculator formulaCalculator)
        {
            _gameDataStore = gameDataStore;
            _formulaCalculator = formulaCalculator;
        }

        /// <summary>
        /// 지정된 효과 ID와 레벨을 기반으로 속성 변경 효과를 생성한다.
        /// </summary>
        /// <param name="effectID">효과의 고유 식별자.</param>
        /// <param name="level">효과에 적용할 레벨.</param>
        /// <returns>생성된 <see cref="IModifyAttributeEffect"/> 인스턴스.</returns>
        public IModifyAttributeEffect Create(string effectID, int level)
        {
            var effectData = _gameDataStore.GetModifyAttributeEffects().FirstOrDefault(entry => entry.ID == effectID);
            var coeffcients = CreateCoeffcients(level, effectData);

            return new ModifyAttributeEffect(effectData.ApplyOrder, effectData.AttributeSetID,
                effectData.CalculationType, coeffcients, _formulaCalculator);
        }

        /// <summary>
        /// 레벨에 따른 계수 딕셔너리를 생성한다.
        /// </summary>
        /// <param name="level">계수 계산에 사용할 레벨.</param>
        /// <param name="effectData">효과의 게임 데이터.</param>
        /// <returns>계수 키-값 쌍의 딕셔너리.</returns>
        private Dictionary<string, float> CreateCoeffcients(int level, GameData.ModifyAttributeEffect effectData)
        {
            var formulas = _gameDataStore.GetCoefficientFormulasBySetID(effectData.CoefficientFormulaSetID);
            var coeffcients = new Dictionary<string, float>();
            foreach (var entry in formulas)
                coeffcients[entry.Key] = CalculateCoeffcient(entry.Value, level);
            return coeffcients;
        }

        /// <summary>
        /// 주어진 공식과 레벨을 사용하여 단일 계수 값을 계산한다.
        /// </summary>
        /// <param name="formula">계수 계산에 사용할 공식 데이터.</param>
        /// <param name="level">계수 계산에 사용할 레벨.</param>
        /// <returns>계산된 계수 값.</returns>
        private float CalculateCoeffcient(GameData.Formula formula, int level)
        {
            var coeffs = _gameDataStore.GetCoefficientByFormulaID(formula.ID);
            var value = _formulaCalculator.Calculate(formula.CalculationType, coeffs, level);
            return value;
        }
    }
}
