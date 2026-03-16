namespace TaskForce.AP.Client.Core.GameData
{
    /// <summary>
    /// 스킬의 기본 정보를 정의하는 게임 데이터 클래스.
    /// 스킬의 고유 식별자와 표시 이름 텍스트 ID를 포함한다.
    /// </summary>
    public class Skill
    {
        /// <summary>
        /// 스킬의 고유 식별자
        /// </summary>
        public string ID;

        /// <summary>
        /// 스킬 이름에 해당하는 텍스트 리소스 식별자
        /// </summary>
        public string NameTextID;
    }
}
