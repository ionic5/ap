namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 비플레이어 유닛(NPC)의 AI 로직 매핑을 정의하는 클래스.
    /// 특정 유닛에 적용할 유닛 로직(AI 행동 패턴)을 지정한다.
    /// </summary>
    public class NonPlayerUnitLogic
    {
        /// <summary>
        /// 대상 유닛의 식별자
        /// </summary>
        public string UnitID;

        /// <summary>
        /// 유닛에 적용할 AI 로직의 식별자
        /// </summary>
        public string UnitLogicID;
    }
}
