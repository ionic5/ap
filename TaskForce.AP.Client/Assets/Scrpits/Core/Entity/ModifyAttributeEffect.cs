using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 속성 변경 효과의 구현 클래스. 공식 계산기를 사용하여 대상 속성 값을 변경하며,
    /// 동일한 적용 순서/계산 유형/대상 속성을 가진 효과끼리 계수를 합산하여 병합할 수 있다.
    /// </summary>
    public class ModifyAttributeEffect : IModifyAttributeEffect
    {
        /// <summary>효과 적용 순서.</summary>
        private int _applyOrder;

        /// <summary>변경 대상 속성 ID.</summary>
        private string _targetAttributeID;

        /// <summary>계산 유형 (공식 종류).</summary>
        private string _calculationType;

        /// <summary>공식에 사용되는 계수 딕셔너리.</summary>
        private Dictionary<string, float> _coefficients;

        /// <summary>공식 계산기 인스턴스.</summary>
        private readonly FormulaCalculator _formulaCalculator;

        /// <summary>
        /// ModifyAttributeEffect의 생성자.
        /// </summary>
        /// <param name="applyOrder">효과 적용 순서.</param>
        /// <param name="targetAttributeID">변경 대상 속성의 고유 식별자.</param>
        /// <param name="calculationType">계산 유형 문자열.</param>
        /// <param name="coefficients">공식에 사용할 계수 딕셔너리.</param>
        /// <param name="formulaCalculator">공식 계산기.</param>
        public ModifyAttributeEffect(int applyOrder,
            string targetAttributeID, string calculationType, Dictionary<string, float> coefficients,
            FormulaCalculator formulaCalculator)
        {
            _applyOrder = applyOrder;
            _targetAttributeID = targetAttributeID;
            _calculationType = calculationType;
            _coefficients = coefficients;
            _formulaCalculator = formulaCalculator;
        }

        /// <summary>
        /// 속성 저장소에서 대상 속성 값을 가져와 공식을 적용한 뒤 결과를 다시 저장한다.
        /// </summary>
        /// <param name="store">효과를 적용할 속성 저장소.</param>
        public void Apply(AttributeStore store)
        {
            var x = _formulaCalculator.Calculate(_calculationType, _coefficients, store.Get(_targetAttributeID).AsFloat());
            store.Set(_targetAttributeID, new Attribute(x));
        }

        /// <summary>
        /// 주어진 효과와 병합 가능한지 판별한다. 적용 순서, 계산 유형, 대상 속성이 모두 동일해야 병합 가능하다.
        /// </summary>
        /// <param name="effect">병합 가능 여부를 확인할 대상 효과.</param>
        /// <returns>병합 가능하면 true, 아니면 false.</returns>
        public bool CanMerge(IModifyAttributeEffect effect)
        {
            if (effect is not ModifyAttributeEffect)
                return false;

            var other = effect as ModifyAttributeEffect;
            if (_applyOrder != other._applyOrder)
                return false;
            if (_calculationType != other._calculationType)
                return false;
            if (_targetAttributeID != other._targetAttributeID)
                return false;

            return true;
        }

        /// <summary>
        /// 효과의 적용 순서를 반환한다.
        /// </summary>
        /// <returns>적용 순서 값.</returns>
        public int GetApplyOrder()
        {
            return _applyOrder;
        }

        /// <summary>
        /// 주어진 효과의 계수를 이 효과의 계수에 합산하여 병합한다.
        /// </summary>
        /// <param name="effect">병합할 대상 효과.</param>
        public void Merge(IModifyAttributeEffect effect)
        {
            if (effect is not ModifyAttributeEffect)
                return;

            var other = effect as ModifyAttributeEffect;
            foreach (var entry in _coefficients)
            {
                var key = entry.Key;
                _coefficients[key] += other._coefficients[key];
            }
        }

        /// <summary>
        /// 이 효과의 복제본을 생성하여 반환한다.
        /// </summary>
        /// <returns>복제된 <see cref="ModifyAttributeEffect"/> 인스턴스.</returns>
        public IModifyAttributeEffect Clone()
        {
            return new ModifyAttributeEffect(_applyOrder,
                _targetAttributeID, _calculationType, _coefficients,
                _formulaCalculator);
        }
    }
}
