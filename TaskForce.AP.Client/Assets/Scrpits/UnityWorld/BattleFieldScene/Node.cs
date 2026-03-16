namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    /// <summary>
    /// A* 경로 탐색에 사용되는 그리드 노드 클래스.
    /// 각 노드는 그리드 좌표, 이동 비용, 이동 가능 여부 정보를 보유한다.
    /// </summary>
    public class Node
    {
        /// <summary>그리드 상의 X 좌표</summary>
        public int GridX;
        /// <summary>그리드 상의 Y 좌표</summary>
        public int GridY;

        /// <summary>시작 노드로부터의 이동 비용 (G 비용)</summary>
        public int GCost;
        /// <summary>목표 노드까지의 추정 비용 (H 비용, 휴리스틱)</summary>
        public int HCost;
        /// <summary>총 비용 (F = G + H)</summary>
        public int FCost { get { return GCost + HCost; } }

        /// <summary>해당 노드가 이동 가능한지 여부</summary>
        public bool IsWalkable;

        /// <summary>경로 역추적을 위한 부모 노드 참조</summary>
        public Node Parent;

        /// <summary>
        /// Node 인스턴스를 생성한다.
        /// </summary>
        /// <param name="isWalkable">이동 가능 여부</param>
        /// <param name="gridX">그리드 X 좌표</param>
        /// <param name="gridY">그리드 Y 좌표</param>
        public Node(bool isWalkable, int gridX, int gridY)
        {
            IsWalkable = isWalkable;
            GridX = gridX;
            GridY = gridY;
        }
    }
}
