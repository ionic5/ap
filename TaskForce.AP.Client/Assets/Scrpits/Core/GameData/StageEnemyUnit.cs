namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 스테이지에 배치되는 적 유닛 정보를 정의하는 클래스.
    /// 특정 스테이지 레벨에 등장하는 적 유닛의 종류와 레벨을 지정한다.
    /// </summary>
    public class StageEnemyUnit
    {
        /// <summary>
        /// 적 유닛이 등장하는 스테이지의 레벨
        /// </summary>
        public int StageLevel;

        /// <summary>
        /// 등장하는 적 유닛의 식별자
        /// </summary>
        public string UnitID;

        /// <summary>
        /// 적 유닛의 레벨
        /// </summary>
        public int Level;
    }
}
