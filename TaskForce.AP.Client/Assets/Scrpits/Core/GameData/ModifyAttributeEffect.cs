namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 속성 수정 효과를 정의하는 게임 데이터 클래스.
    /// 특정 속성 집합에 대해 계산 방식과 계수 수식을 적용하여 속성값을 변경하는 효과를 나타낸다.
    /// </summary>
    public class ModifyAttributeEffect
    {
        /// <summary>
        /// 속성 수정 효과의 고유 식별자
        /// </summary>
        public string ID;

        /// <summary>
        /// 효과의 적용 순서 (낮은 값이 먼저 적용됨)
        /// </summary>
        public int ApplyOrder;

        /// <summary>
        /// 수정 대상이 되는 속성 집합의 식별자
        /// </summary>
        public string AttributeSetID;

        /// <summary>
        /// 속성 수정에 사용되는 계산 방식 (예: 고정값, 퍼센트)
        /// </summary>
        public string CalculationType;

        /// <summary>
        /// 속성 수정에 사용되는 계수 수식 집합의 식별자
        /// </summary>
        public string CoefficientFormulaSetID;

    }
}
