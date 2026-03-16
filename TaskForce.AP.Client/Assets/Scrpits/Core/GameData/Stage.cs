namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 스테이지 정보를 정의하는 게임 데이터 클래스.
    /// 스테이지의 레벨, 제한 시간, 최대 적 유닛 수를 포함한다.
    /// </summary>
    public class Stage
    {
        /// <summary>
        /// 스테이지의 레벨 (난이도 단계)
        /// </summary>
        public int Level;

        /// <summary>
        /// 스테이지의 제한 시간 (초 단위)
        /// </summary>
        public float Time;

        /// <summary>
        /// 스테이지에 동시에 등장할 수 있는 최대 적 유닛 수
        /// </summary>
        public int MaxEnemyUnitCount;
    }
}
