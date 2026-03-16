namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// Unity 씬의 식별자 상수를 정의하는 클래스.
    /// 씬 로드 및 전환 시 사용되는 씬 이름 문자열을 제공한다.
    /// </summary>
    public class SceneID
    {
        /// <summary>전투 씬 식별자</summary>
        public const string BattleFieldScene = "BattleFieldScene";
        /// <summary>빈 씬 식별자 (씬 전환 시 기존 씬 언로드를 위해 사용)</summary>
        public const string EmptyScene = "EmptyScene";
    }
}
