using System.Collections.Generic;
using System.Linq;

namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 속성(Attribute) 중재자 클래스. 기본 속성, 성장 공식, 속성 변경 효과를 통합 관리하여
    /// 레벨 및 효과 적용에 따라 최종 속성 값을 계산한다.
    /// </summary>
    public class AttributeMediator
    {
        /// <summary>게임 데이터 저장소.</summary>
        private readonly GameDataStore _gameDataStore;

        /// <summary>로거 인스턴스.</summary>
        private readonly Core.ILogger _logger;

        /// <summary>기본(베이스) 속성 저장소.</summary>
        private readonly AttributeStore _baseAttributeStore;

        /// <summary>최종 계산된 속성 저장소.</summary>
        private readonly AttributeStore _attributeStore;

        /// <summary>적용된 속성 변경 효과 목록.</summary>
        private readonly List<IModifyAttributeEffect> _modifyAttributeEffects;

        /// <summary>성장 공식 목록.</summary>
        private readonly List<GameData.GrowthFormula> _growthFormulas;

        /// <summary>현재 레벨.</summary>
        private int _level;

        /// <summary>
        /// AttributeMediator의 생성자. 초기 레벨을 1로 설정하고 속성을 갱신한다.
        /// </summary>
        /// <param name="gameDataStore">게임 데이터 저장소.</param>
        /// <param name="logger">로거 인스턴스.</param>
        public AttributeMediator(GameDataStore gameDataStore, ILogger logger)
        {
            _attributeStore = new AttributeStore();
            _modifyAttributeEffects = new List<IModifyAttributeEffect>();
            _baseAttributeStore = new AttributeStore();
            _growthFormulas = new List<GameData.GrowthFormula>();
            _gameDataStore = gameDataStore;
            _logger = logger;
            _level = 1;

            UpdateAttributes();
        }

        /// <summary>
        /// 레벨을 설정하고 속성을 다시 계산한다.
        /// </summary>
        /// <param name="level">설정할 레벨 값.</param>
        public void SetLevel(int level)
        {
            _level = level;

            UpdateAttributes();
        }

        /// <summary>
        /// 기본 속성을 설정하고 속성을 다시 계산한다.
        /// </summary>
        /// <param name="attributes">속성 ID를 키로 하는 기본 속성 딕셔너리.</param>
        public void SetBaseAttributes(IReadOnlyDictionary<string, Attribute> attributes)
        {
            _baseAttributeStore.Clear();
            foreach (var entry in attributes)
                _baseAttributeStore.Set(entry.Key, entry.Value);

            UpdateAttributes();
        }

        /// <summary>
        /// 성장 공식 목록을 설정한다.
        /// </summary>
        /// <param name="formulas">적용할 성장 공식 컬렉션.</param>
        public void SetGrowthFormulas(IEnumerable<GameData.GrowthFormula> formulas)
        {
            _growthFormulas.Clear();
            _growthFormulas.AddRange(formulas);
        }

        /// <summary>
        /// 여러 속성 변경 효과를 추가하고 속성을 다시 계산한다.
        /// </summary>
        /// <param name="effects">추가할 속성 변경 효과 컬렉션.</param>
        public void AddModifyAttributeEffects(IEnumerable<IModifyAttributeEffect> effects)
        {
            _modifyAttributeEffects.AddRange(effects);

            UpdateAttributes();
        }

        /// <summary>
        /// 단일 속성 변경 효과를 추가하고 속성을 다시 계산한다.
        /// </summary>
        /// <param name="effect">추가할 속성 변경 효과.</param>
        public void AddModifyAttributeEffect(IModifyAttributeEffect effect)
        {
            _modifyAttributeEffects.Add(effect);

            UpdateAttributes();
        }

        /// <summary>
        /// 지정된 ID에 해당하는 최종 계산된 속성 값을 반환한다.
        /// </summary>
        /// <param name="id">조회할 속성의 고유 식별자.</param>
        /// <returns>최종 계산된 <see cref="Attribute"/> 객체.</returns>
        public Attribute GetAttribute(string id)
        {
            return _attributeStore.Get(id);
        }

        /// <summary>
        /// 기본 속성을 복사한 뒤 성장 공식과 속성 변경 효과를 순서대로 적용하여
        /// 최종 속성 값을 갱신한다.
        /// </summary>
        private void UpdateAttributes()
        {
            _baseAttributeStore.CopyTo(_attributeStore);

            foreach (var formula in _growthFormulas)
            {
                var value = ApplyGrowth(formula.FormulaID, _attributeStore.Get(formula.TargetID), _level);
                _attributeStore.Set(formula.TargetID, value);
            }

            var mergedEffects = new List<IModifyAttributeEffect>();
            foreach (var entry in _modifyAttributeEffects)
            {
                var existing = mergedEffects.FirstOrDefault(m => m.CanMerge(entry));
                if (existing == null)
                    mergedEffects.Add(entry.Clone());
                else
                    existing.Merge(entry);
            }
            mergedEffects.Sort((a, b) => a.GetApplyOrder().CompareTo(b.GetApplyOrder()));

            foreach (var entry in mergedEffects)
                entry.Apply(_attributeStore);
        }

        /// <summary>
        /// 주어진 공식 ID와 레벨에 따라 성장 계수를 적용한 속성 값을 계산한다.
        /// </summary>
        /// <param name="formulaID">성장 공식의 고유 식별자.</param>
        /// <param name="attribute">적용 대상 기본 속성.</param>
        /// <param name="level">현재 레벨.</param>
        /// <returns>성장이 적용된 <see cref="Attribute"/> 객체.</returns>
        private Attribute ApplyGrowth(string formulaID, Attribute attribute, int level)
        {
            var formula = _gameDataStore.GetFormulas().FirstOrDefault(entry => entry.ID == formulaID);
            if (formula == null)
            {
                _logger.Warn($"Failed to find formula for id ({formulaID}) base stat returned.");
                return attribute;
            }

            var coeffs = _gameDataStore.GetCoefficients().Where(entry => entry.FormulaID == formulaID);
            var a = coeffs.ElementAtOrDefault(0).Value;
            return new Attribute(attribute.AsFloat() + a * (1 - level));
        }
    }
}
