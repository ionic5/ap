using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 게임에서 사용하는 정적 데이터(유닛, 스킬, 스테이지, 수식, 계수 등)를 저장하고 조회하는 데이터 저장소 클래스.
    /// 데이터를 추가한 뒤 <see cref="Bake"/>를 호출하면 빠른 조회를 위한 인덱스가 구축된다.
    /// </summary>
    public class GameDataStore
    {
        /// <summary>스킬 기본 속성 목록.</summary>
        private readonly List<SkillBaseAttribute> _skillBaseAttributes;

        /// <summary>계수 목록.</summary>
        private readonly List<Coefficient> _coefficients;

        /// <summary>스테이지별 적 유닛 배치 목록.</summary>
        private readonly List<StageEnemyUnit> _stageEnemyUnits;

        /// <summary>스테이지 정의 목록.</summary>
        private readonly List<Stage> _stages;

        /// <summary>유닛 정의 목록.</summary>
        private readonly List<Unit> _units;

        /// <summary>NPC 유닛 로직 목록.</summary>
        private readonly List<NonPlayerUnitLogic> _nonPlayerUnitLogics;

        /// <summary>유닛 기본 속성 목록.</summary>
        private readonly List<UnitBaseAttribute> _unitBaseAttributes;

        /// <summary>속성 변경 효과 목록.</summary>
        private readonly List<ModifyAttributeEffect> _modifyAttributeEffects;

        /// <summary>성장 수식 목록.</summary>
        private readonly List<GrowthFormula> _growthFormulas;

        /// <summary>수식 정의 목록.</summary>
        private readonly List<Formula> _formulas;

        /// <summary>스킬 성장 수식 목록.</summary>
        private readonly List<SkillGrowthFormula> _skillGrowthFormulas;

        /// <summary>유닛 기본 스킬 할당 목록.</summary>
        private readonly List<UnitDefaultSkill> _unitDefaultSkills;

        /// <summary>스킬 정의 목록.</summary>
        private readonly List<Skill> _skills;

        /// <summary>레벨업 보상 스킬 목록.</summary>
        private readonly List<LevelUpRewardSkill> _levelUpRewardSkills;

        /// <summary>속성 세트 목록.</summary>
        private readonly List<AttributeSet> _attributeSets;

        /// <summary>속성 변경 스킬 목록.</summary>
        private readonly List<ModifyAttributeSkill> _modifyAttributeSkills;

        /// <summary>계수 수식 세트 목록.</summary>
        private readonly List<CoefficientFormulaSet> _coefficientFormulaSets;

        /// <summary>유닛 기본 액티브 스킬 목록.</summary>
        private readonly List<UnitDefaultActiveSkill> _unitDefaultActiveSkill;

        /// <summary>소울 드롭 확률.</summary>
        private float _soulDropRate;

        /// <summary>ID를 키로 하는 수식 딕셔너리 (Bake 후 사용).</summary>
        private Dictionary<string, Formula> _formulasByID;

        /// <summary>ID를 키로 하는 성장 수식 룩업 (Bake 후 사용).</summary>
        private ILookup<string, GrowthFormula> _growthFormulasByID;

        /// <summary>ID를 키로 하는 스킬 딕셔너리 (Bake 후 사용).</summary>
        private Dictionary<string, Skill> _skillsByID;

        /// <summary>수식 ID를 키로 하는 계수 딕셔너리 (Bake 후 사용).</summary>
        private Dictionary<string, Dictionary<string, float>> _coefficientsByFormulaID;

        /// <summary>스킬 ID를 키로 하는 속성 변경 스킬 딕셔너리 (Bake 후 사용).</summary>
        private Dictionary<string, IEnumerable<ModifyAttributeSkill>> _modifyAttributeSkillsBySkillID;

        /// <summary>세트 ID를 키로 하는 계수 수식 딕셔너리 (Bake 후 사용).</summary>
        private Dictionary<string, Dictionary<string, Formula>> _coefficientFormulasBySetID;

        /// <summary>
        /// <see cref="GameDataStore"/>의 새 인스턴스를 생성하고 내부 컬렉션을 초기화한다.
        /// </summary>
        public GameDataStore()
        {
            _skillBaseAttributes = new List<SkillBaseAttribute>();
            _coefficients = new List<Coefficient>();
            _modifyAttributeEffects = new List<ModifyAttributeEffect>();
            _stageEnemyUnits = new List<StageEnemyUnit>();
            _stages = new List<Stage>();
            _unitBaseAttributes = new List<UnitBaseAttribute>();
            _units = new List<Unit>();
            _nonPlayerUnitLogics = new List<NonPlayerUnitLogic>();
            _growthFormulas = new List<GrowthFormula>();
            _formulas = new List<Formula>();
            _unitDefaultSkills = new List<UnitDefaultSkill>();
            _skills = new List<Skill>();
            _skillGrowthFormulas = new List<SkillGrowthFormula>();
            _levelUpRewardSkills = new List<LevelUpRewardSkill>();
            _coefficientFormulaSets = new List<CoefficientFormulaSet>();
            _attributeSets = new List<AttributeSet>();
            _modifyAttributeSkills = new List<ModifyAttributeSkill>();
            _modifyAttributeSkillsBySkillID = new Dictionary<string, IEnumerable<ModifyAttributeSkill>>();
            _coefficientFormulasBySetID = new Dictionary<string, Dictionary<string, Formula>>();
            _unitDefaultActiveSkill = new List<UnitDefaultActiveSkill>();
        }

        /// <summary>
        /// 추가된 데이터를 기반으로 빠른 조회를 위한 딕셔너리 및 룩업 인덱스를 구축한다.
        /// 모든 데이터 추가가 완료된 후 반드시 호출해야 한다.
        /// </summary>
        public void Bake()
        {
            _formulasByID = _formulas.ToDictionary(entry => entry.ID);
            _skillsByID = _skills.ToDictionary(entry => entry.ID);
            _coefficientsByFormulaID = _coefficients.GroupBy(entry => entry.FormulaID).ToDictionary(
                group => group.Key,
                group => group.ToDictionary(entry => entry.Key, entry => entry.Value));
            _growthFormulasByID = _growthFormulas.ToLookup(entry => entry.ID);
            _modifyAttributeSkillsBySkillID = _modifyAttributeSkills.GroupBy(entry => entry.SkillID).ToDictionary(
                group => group.Key,
                group => group.AsEnumerable());
            _coefficientFormulasBySetID = _coefficientFormulaSets.GroupBy(entry => entry.ID).ToDictionary(
                group => group.Key,
                group => group.ToDictionary(
                    entry => entry.TargetCoefficientKey,
                    entry => GetFormulaByID(entry.FormulaID)));
        }

        /// <summary>
        /// 레벨업 보상 스킬 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 레벨업 보상 스킬 데이터.</param>
        public void AddLevelUpRewardSkill(LevelUpRewardSkill entry)
        {
            _levelUpRewardSkills.Add(entry);
        }

        /// <summary>
        /// 스킬 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 스킬 데이터.</param>
        public void AddSkill(Skill entry)
        {
            _skills.Add(entry);
        }

        /// <summary>
        /// 스킬 성장 수식 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 스킬 성장 수식 데이터.</param>
        public void AddSkillGrowthFormula(SkillGrowthFormula entry)
        {
            _skillGrowthFormulas.Add(entry);
        }

        /// <summary>
        /// 유닛 기본 스킬 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 유닛 기본 스킬 데이터.</param>
        public void AddUnitDefaultSkill(UnitDefaultSkill entry)
        {
            _unitDefaultSkills.Add(entry);
        }

        /// <summary>
        /// 수식 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 수식 데이터.</param>
        public void AddFormula(Formula entry)
        {
            _formulas.Add(entry);
        }

        /// <summary>
        /// 유닛 속성 성장 수식 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 성장 수식 데이터.</param>
        public void AddUnitAttributeGrowthFormula(GrowthFormula entry)
        {
            _growthFormulas.Add(entry);
        }

        /// <summary>
        /// NPC 유닛 로직 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 NPC 유닛 로직 데이터.</param>
        public void AddNonPlayerUnitLogic(NonPlayerUnitLogic entry)
        {
            _nonPlayerUnitLogics.Add(entry);
        }

        /// <summary>
        /// 유닛 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 유닛 데이터.</param>
        public void AddUnit(Unit entry)
        {
            _units.Add(entry);
        }

        /// <summary>
        /// 유닛 기본 속성 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 유닛 기본 속성 데이터.</param>
        public void AddUnitBaseAttribute(UnitBaseAttribute entry)
        {
            _unitBaseAttributes.Add(entry);
        }

        /// <summary>
        /// 스테이지 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 스테이지 데이터.</param>
        public void AddStage(Stage entry)
        {
            _stages.Add(entry);
        }

        /// <summary>
        /// 스테이지 적 유닛 배치 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 스테이지 적 유닛 데이터.</param>
        public void AddStageEnemyUnit(StageEnemyUnit entry)
        {
            _stageEnemyUnits.Add(entry);
        }

        /// <summary>
        /// 스킬 기본 속성 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 스킬 기본 속성 데이터.</param>
        public void AddSkillAttribute(SkillBaseAttribute entry)
        {
            _skillBaseAttributes.Add(entry);
        }

        /// <summary>
        /// 계수 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 계수 데이터.</param>
        public void AddCoefficient(Coefficient entry)
        {
            _coefficients.Add(entry);
        }

        /// <summary>
        /// 속성 변경 효과 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 속성 변경 효과 데이터.</param>
        public void AddModifyAttributeEffect(ModifyAttributeEffect entry)
        {
            _modifyAttributeEffects.Add(entry);
        }

        /// <summary>
        /// 소울 드롭 확률을 설정한다.
        /// </summary>
        /// <param name="value">설정할 소울 드롭 확률 값.</param>
        public void SetSoulDropRate(float value)
        {
            _soulDropRate = value;
        }

        /// <summary>
        /// 현재 설정된 소울 드롭 확률을 반환한다.
        /// </summary>
        /// <returns>소울 드롭 확률 값.</returns>
        public float GetSoulDropRate()
        {
            return _soulDropRate;
        }

        /// <summary>
        /// 지정된 수식 ID에 해당하는 계수 딕셔너리를 반환한다.
        /// </summary>
        /// <param name="id">조회할 수식 ID.</param>
        /// <returns>계수 키-값 쌍의 읽기 전용 딕셔너리.</returns>
        public IReadOnlyDictionary<string, float> GetCoefficientByFormulaID(string id)
        {
            return _coefficientsByFormulaID[id];
        }

        /// <summary>
        /// 지정된 ID에 해당하는 성장 수식 목록을 반환한다.
        /// </summary>
        /// <param name="id">조회할 성장 수식 ID.</param>
        /// <returns>해당 ID의 성장 수식 컬렉션.</returns>
        public IEnumerable<GrowthFormula> GetGrowthFormulaByID(string id)
        {
            return _growthFormulasByID[id];
        }

        /// <summary>
        /// 지정된 ID에 해당하는 수식을 반환한다. 존재하지 않으면 기본값을 반환한다.
        /// </summary>
        /// <param name="id">조회할 수식 ID.</param>
        /// <returns>해당 ID의 수식. 없으면 기본값.</returns>
        public Formula GetFormulaByID(string id)
        {
            return _formulasByID.GetValueOrDefault(id);
        }

        /// <summary>
        /// 지정된 ID에 해당하는 스킬을 반환한다. 존재하지 않으면 기본값을 반환한다.
        /// </summary>
        /// <param name="id">조회할 스킬 ID.</param>
        /// <returns>해당 ID의 스킬. 없으면 기본값.</returns>
        public Skill GetSkillById(string id)
        {
            return _skillsByID.GetValueOrDefault(id);
        }

        /// <summary>
        /// 지정된 레벨에서 획득 가능한 소울 경험치를 계산하여 반환한다.
        /// </summary>
        /// <param name="level">계산 기준 레벨.</param>
        /// <returns>해당 레벨의 소울 경험치.</returns>
        public int GetSoulExp(int level)
        {
            var factors = _coefficientsByFormulaID[CoefficientID.SoulExp].ToList();
            if (factors.Count == 0) return 0;

            float a = factors[0].Value;
            return (int)Math.Floor(a * level);
        }

        /// <summary>
        /// 지정된 레벨에서 다음 레벨로 올리기 위해 필요한 경험치를 계산하여 반환한다.
        /// 이차 다항식(a*level^2 + b*level + c)으로 계산된다.
        /// </summary>
        /// <param name="level">계산 기준 레벨.</param>
        /// <returns>필요 경험치.</returns>
        public int GetRequireExp(int level)
        {
            var factors = _coefficients.Where(entry => entry.FormulaID == CoefficientID.RequireExp);
            var a = factors.ElementAt(0).Value;
            var b = factors.ElementAt(1).Value;
            var c = factors.ElementAt(2).Value;

            return (int)Math.Floor(a * Math.Pow(level, 2) + b * level + c);
        }

        /// <summary>
        /// 지정된 스킬 ID에 해당하는 속성 변경 스킬 효과 목록을 반환한다.
        /// </summary>
        /// <param name="skillID">조회할 스킬 ID.</param>
        /// <returns>속성 변경 스킬 효과 컬렉션. 없으면 null.</returns>
        public IEnumerable<ModifyAttributeSkill> GetModifyAttributeSkillEffects(string skillID)
        {
            return _modifyAttributeSkillsBySkillID.GetValueOrDefault(skillID);
        }

        /// <summary>
        /// 모든 스킬 기본 속성 목록을 반환한다.
        /// </summary>
        /// <returns>스킬 기본 속성 컬렉션.</returns>
        public IEnumerable<SkillBaseAttribute> GetSkillBaseAttributes()
        {
            return _skillBaseAttributes;
        }

        /// <summary>
        /// 모든 계수 목록을 반환한다.
        /// </summary>
        /// <returns>계수 컬렉션.</returns>
        public IEnumerable<Coefficient> GetCoefficients()
        {
            return _coefficients;
        }

        /// <summary>
        /// 모든 속성 변경 효과 목록을 반환한다.
        /// </summary>
        /// <returns>속성 변경 효과 컬렉션.</returns>
        public IEnumerable<ModifyAttributeEffect> GetModifyAttributeEffects()
        {
            return _modifyAttributeEffects;
        }

        /// <summary>
        /// 모든 스테이지 목록을 반환한다.
        /// </summary>
        /// <returns>스테이지 컬렉션.</returns>
        public IEnumerable<Stage> GetStages()
        {
            return _stages;
        }

        /// <summary>
        /// 모든 스테이지 적 유닛 배치 목록을 반환한다.
        /// </summary>
        /// <returns>스테이지 적 유닛 컬렉션.</returns>
        public IEnumerable<StageEnemyUnit> GetStageEnemyUnits()
        {
            return _stageEnemyUnits;
        }

        /// <summary>
        /// 모든 유닛 목록을 반환한다.
        /// </summary>
        /// <returns>유닛 컬렉션.</returns>
        public IEnumerable<Unit> GetUnits()
        {
            return _units;
        }

        /// <summary>
        /// 모든 NPC 유닛 로직 목록을 반환한다.
        /// </summary>
        /// <returns>NPC 유닛 로직 컬렉션.</returns>
        public IEnumerable<NonPlayerUnitLogic> GetNonPlayerUnitLogics()
        {
            return _nonPlayerUnitLogics;
        }

        /// <summary>
        /// 모든 유닛 기본 속성 목록을 반환한다.
        /// </summary>
        /// <returns>유닛 기본 속성 컬렉션.</returns>
        public IEnumerable<UnitBaseAttribute> GetUnitBaseAttributes()
        {
            return _unitBaseAttributes;
        }

        /// <summary>
        /// 모든 성장 수식 목록을 반환한다.
        /// </summary>
        /// <returns>성장 수식 컬렉션.</returns>
        public IEnumerable<GrowthFormula> GetGrowthFormulas()
        {
            return _growthFormulas;
        }

        /// <summary>
        /// 지정된 ID에 해당하는 성장 수식 목록을 필터링하여 반환한다.
        /// </summary>
        /// <param name="id">필터링할 성장 수식 ID.</param>
        /// <returns>해당 ID와 일치하는 성장 수식 컬렉션.</returns>
        public IEnumerable<GrowthFormula> GetGrowthFormulas(string id)
        {
            return _growthFormulas.Where(entry => entry.ID == id);
        }

        /// <summary>
        /// 모든 수식 목록을 반환한다.
        /// </summary>
        /// <returns>수식 컬렉션.</returns>
        public IEnumerable<Formula> GetFormulas()
        {
            return _formulas;
        }

        /// <summary>
        /// 모든 스킬 목록을 반환한다.
        /// </summary>
        /// <returns>스킬 컬렉션.</returns>
        public IEnumerable<Skill> GetSkills()
        {
            return _skills;
        }

        /// <summary>
        /// 모든 유닛 기본 스킬 할당 목록을 반환한다.
        /// </summary>
        /// <returns>유닛 기본 스킬 컬렉션.</returns>
        public IEnumerable<UnitDefaultSkill> GetUnitDefaultSkills()
        {
            return _unitDefaultSkills;
        }

        /// <summary>
        /// 모든 스킬 성장 수식 목록을 반환한다.
        /// </summary>
        /// <returns>스킬 성장 수식 컬렉션.</returns>
        public IEnumerable<SkillGrowthFormula> GetSkillGrowthFormulas()
        {
            return _skillGrowthFormulas;
        }

        /// <summary>
        /// 지정된 스킬 ID에 연결된 성장 수식 목록을 반환한다.
        /// 스킬 성장 수식 매핑을 통해 성장 수식 ID를 찾은 뒤 해당 성장 수식을 조회한다.
        /// </summary>
        /// <param name="skillID">조회할 스킬 ID.</param>
        /// <returns>해당 스킬의 성장 수식 컬렉션.</returns>
        public IEnumerable<GrowthFormula> GetSkillGrowthFormulas(string skillID)
        {
            var growthFormulaID = GetSkillGrowthFormulas().Where(entry => entry.SkillID == skillID).Select(entry => entry.GrowthFormulaID).FirstOrDefault();
            return GetGrowthFormulas(growthFormulaID);
        }

        /// <summary>
        /// 지정된 스킬 ID에 해당하는 기본 속성을 속성 ID를 키로 하는 딕셔너리로 반환한다.
        /// </summary>
        /// <param name="skillID">조회할 스킬 ID.</param>
        /// <returns>속성 ID를 키, <see cref="Attribute"/>를 값으로 하는 읽기 전용 딕셔너리.</returns>
        public IReadOnlyDictionary<string, Attribute> GetSkillBaseAttributes(string skillID)
        {
            var result = new Dictionary<string, Attribute>();
            var attributes = GetSkillBaseAttributes().Where(entry => entry.SkillID == skillID);
            if (!attributes.Any())
                return result;
            foreach (var entry in attributes)
                result[entry.AttributeID] = entry.Value;
            return result;
        }

        /// <summary>
        /// 모든 레벨업 보상 스킬 목록을 반환한다.
        /// </summary>
        /// <returns>레벨업 보상 스킬 컬렉션.</returns>
        public IEnumerable<LevelUpRewardSkill> GetLevelUpRewardSkills()
        {
            return _levelUpRewardSkills;
        }

        /// <summary>
        /// 속성 세트 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 속성 세트 데이터.</param>
        public void AddAttributeSet(AttributeSet entry)
        {
            _attributeSets.Add(entry);
        }

        /// <summary>
        /// 속성 변경 스킬 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 속성 변경 스킬 데이터.</param>
        public void AddModifyAttributeSkill(ModifyAttributeSkill entry)
        {
            _modifyAttributeSkills.Add(entry);
        }

        /// <summary>
        /// 계수 수식 세트 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 계수 수식 세트 데이터.</param>
        public void AddCoeffcientFomulaSet(CoefficientFormulaSet entry)
        {
            _coefficientFormulaSets.Add(entry);
        }

        /// <summary>
        /// 유닛 기본 액티브 스킬 항목을 추가한다.
        /// </summary>
        /// <param name="entry">추가할 유닛 기본 액티브 스킬 데이터.</param>
        public void AddUnitDefaultActiveSkill(UnitDefaultActiveSkill entry)
        {
            _unitDefaultActiveSkill.Add(entry);
        }

        /// <summary>
        /// 지정된 세트 ID에 해당하는 계수 수식 딕셔너리를 반환한다.
        /// </summary>
        /// <param name="setID">조회할 계수 수식 세트 ID.</param>
        /// <returns>계수 키를 키로, 수식을 값으로 하는 읽기 전용 딕셔너리.</returns>
        public IReadOnlyDictionary<string, Formula> GetCoefficientFormulasBySetID(string setID)
        {
            return _coefficientFormulasBySetID[setID];
        }

        /// <summary>
        /// 지정된 유닛 ID에 할당된 기본 액티브 스킬을 반환한다.
        /// </summary>
        /// <param name="unitID">조회할 유닛 ID.</param>
        /// <returns>해당 유닛의 기본 액티브 스킬. 없으면 기본값.</returns>
        public UnitDefaultActiveSkill GetUnitDefaultActiveSkillByUnitID(string unitID)
        {
            return _unitDefaultActiveSkill.FirstOrDefault(entry => entry.UnitID == unitID);
        }
    }
}
