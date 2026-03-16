namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 추적 가능한 객체를 나타내는 인터페이스.
    /// 위치 정보와 파괴 이벤트를 제공하여 카메라나 다른 객체가 이 객체를 추적할 수 있게 한다.
    /// </summary>
    public interface IFollowable : IDestroyable, IPositionable
    {
    }
}
