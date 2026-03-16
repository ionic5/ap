using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.GameData;
using TaskForce.AP.Client.UnityWorld.AssetData;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// CSV 에셋에서 게임 데이터를 비동기적으로 로드하여 GameDataStore에 적재하는 클래스.
    /// 유닛, 스킬, 스테이지, 공식, 계수 등 모든 게임 데이터 테이블을 병렬로 로드한다.
    /// </summary>
    public class GameDataLoader
    {
        /// <summary>CSV 데이터를 로드하기 위한 로더</summary>
        private readonly CsvLoader _csvLoader;

        /// <summary>
        /// GameDataLoader의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="csvLoader">CSV 파일을 로드할 로더 인스턴스</param>
        public GameDataLoader(CsvLoader csvLoader)
        {
            _csvLoader = csvLoader;
        }

        /// <summary>
        /// 모든 게임 데이터 테이블을 병렬로 로드하고, 완료 후 GameDataStore를 베이킹한다.
        /// </summary>
        /// <param name="gameDataStore">로드된 데이터를 저장할 게임 데이터 저장소</param>
        /// <returns>비동기 작업</returns>
        public async Task Load(GameDataStore gameDataStore)
        {
            var loadTasks = new List<Task>
            {
                LoadConstants(gameDataStore),
                LoadTable(AssetID.Coefficient, row => new Coefficient {
                    FormulaID = row["formulaID"],
                    Key = row["key"],
                    Value = float.Parse(row["value"])
                }, gameDataStore.AddCoefficient),
                LoadTable(AssetID.SkillBaseAttribute, row => new SkillBaseAttribute {
                    SkillID = row["skillID"],
                    AttributeID = row["attributeID"],
                    Value = new Core.Attribute(float.Parse(row["value"]))
                }, gameDataStore.AddSkillAttribute),
                LoadTable(AssetID.ModifyAttributeEffect, row => new Core.GameData.ModifyAttributeEffect {
                    ID = row["id"],
                    ApplyOrder = int.Parse(row["applyOrder"]),
                    AttributeSetID = row["attributeSetID"],
                    CalculationType = row["calculationType"],
                    CoefficientFormulaSetID = row["coeffcientFormulaSetID"]
                }, gameDataStore.AddModifyAttributeEffect),
                LoadTable(AssetID.StageEnemyUnit, row => new StageEnemyUnit {
                    StageLevel = int.Parse(row["stageLevel"]),
                    UnitID = row["unitID"],
                    Level = int.Parse(row["level"])
                }, gameDataStore.AddStageEnemyUnit),
                LoadTable(AssetID.Stage, row => new Stage {
                    Level = int.Parse(row["level"]),
                    Time = float.Parse(row["time"]),
                    MaxEnemyUnitCount = int.Parse(row["maxEnemyUnitCount"])
                }, gameDataStore.AddStage),
                LoadTable(AssetID.UnitBaseAttribute, row => new UnitBaseAttribute {
                    ID = row["id"],
                    AttributeID = row["attributeID"],
                    Value = new Core.Attribute(float.Parse(row["value"]))
                }, gameDataStore.AddUnitBaseAttribute),
                LoadTable(AssetID.Unit, row => new Core.GameData.Unit {
                    ID = row["id"],
                    BaseAttributeID = row["baseAttributeID"],
                    UnitBodyID = row["unitBodyID"],
                    AttributeGrowthFormulaID = row["growthFormulaID"]
                }, gameDataStore.AddUnit),
                LoadTable(AssetID.NonPlayerUnitLogic, row => new NonPlayerUnitLogic {
                    UnitID = row["unitID"],
                    UnitLogicID = row["unitLogicID"]
                }, gameDataStore.AddNonPlayerUnitLogic),
                LoadTable(AssetID.GrowthFormula, row => new GrowthFormula {
                    ID = row["id"],
                    TargetID = row["targetAttributeID"],
                    FormulaID = row["formulaID"]
                }, gameDataStore.AddUnitAttributeGrowthFormula),
                LoadTable(AssetID.Formula, row => new Formula {
                    ID = row["id"],
                    CalculationType = row["calculationType"]
                }, gameDataStore.AddFormula),
                LoadTable(AssetID.UnitDefaultSkill, row => new UnitDefaultSkill {
                    UnitID = row["unitID"],
                    SkillID = row["skillID"]
                }, gameDataStore.AddUnitDefaultSkill),
                LoadTable(AssetID.Skill, row => new Core.GameData.Skill {
                    ID = row["id"],
                    NameTextID = row["nameTextID"]
                }, gameDataStore.AddSkill),
                LoadTable(AssetID.SkillGrowthFormula, row => new SkillGrowthFormula {
                    SkillID = row["skillID"],
                    GrowthFormulaID = row["growthFormulaID"]
                }, gameDataStore.AddSkillGrowthFormula),
                LoadTable(AssetID.LevelUpRewardSkill, row => new LevelUpRewardSkill {
                    SkillID = row["skillID"]
                }, gameDataStore.AddLevelUpRewardSkill),
                LoadTable(AssetID.AttributeSet, row => new Core.GameData.AttributeSet {
                    ID = row["id"],
                    AttributeID = row["attributeID"]
                }, gameDataStore.AddAttributeSet),
                LoadTable(AssetID.ModifyAttributeSkill, row => new Core.GameData.ModifyAttributeSkill {
                    SkillID = row["skillID"],
                    ModifyAttributeEffectID = row["modifyAttributeEffectID"]
                }, gameDataStore.AddModifyAttributeSkill),
                LoadTable(AssetID.CoefficientFormulaSet, row => new Core.GameData.CoefficientFormulaSet {
                    ID = row["id"],
                    TargetCoefficientKey = row["targetCoefficientKey"],
                    FormulaID = row["formulaID"]
                }, gameDataStore.AddCoeffcientFomulaSet),
                LoadTable(AssetID.UnitDefaultActiveSkill, row => new Core.GameData.UnitDefaultActiveSkill {
                    UnitID = row["unitID"],
                    SkillID = row["skillID"]
                }, gameDataStore.AddUnitDefaultActiveSkill),
            };

            await Task.WhenAll(loadTasks);

            gameDataStore.Bake();
        }

        /// <summary>
        /// 지정된 에셋 ID의 CSV 데이터를 로드하여 각 행을 파싱한 뒤 저장소에 추가한다.
        /// </summary>
        /// <typeparam name="T">파싱 결과 데이터 타입</typeparam>
        /// <param name="assetId">로드할 CSV 에셋의 식별자</param>
        /// <param name="parser">CSV 행을 T 타입 객체로 변환하는 파서 함수</param>
        /// <param name="adder">파싱된 객체를 저장소에 추가하는 함수</param>
        /// <returns>비동기 작업</returns>
        private async Task LoadTable<T>(string assetId, Func<Dictionary<string, string>, T> parser, Action<T> adder)
        {
            var rows = await _csvLoader.LoadCsv(assetId);
            foreach (var row in rows)
            {
                adder(parser(row));
            }
        }

        /// <summary>
        /// 상수 데이터를 CSV에서 로드하여 GameDataStore에 설정한다.
        /// </summary>
        /// <param name="gameDataStore">상수를 설정할 게임 데이터 저장소</param>
        /// <returns>비동기 작업</returns>
        private async Task LoadConstants(GameDataStore gameDataStore)
        {
            var rows = await _csvLoader.LoadCsv(AssetID.Constants);
            var map = rows.ToDictionary(row => row["constantID"], row => row["value"]);

            if (map.TryGetValue("SOUL_DROP_RATE", out var soulDropRate))
            {
                gameDataStore.SetSoulDropRate(float.Parse(soulDropRate));
            }
        }
    }
}
