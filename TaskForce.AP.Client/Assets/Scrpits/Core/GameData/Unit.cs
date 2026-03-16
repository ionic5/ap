namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 유닛의 기본 정보를 정의하는 게임 데이터 클래스.
    /// 유닛의 식별자, 기본 속성, 외형, 속성 성장 수식을 포함한다.
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// 유닛의 고유 식별자
        /// </summary>
        public string ID;

        /// <summary>
        /// 유닛의 기본 속성 데이터 식별자
        /// </summary>
        public string BaseAttributeID;

        /// <summary>
        /// 유닛 외형(바디) 리소스의 식별자
        /// </summary>
        public string UnitBodyID;

        /// <summary>
        /// 유닛의 레벨별 속성 성장 수식 식별자
        /// </summary>
        public string AttributeGrowthFormulaID;
    }
}
