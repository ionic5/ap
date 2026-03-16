namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 조이스틱 입력을 추상화하는 인터페이스.
    /// 입력 방향 벡터와 조작 중 여부를 제공한다.
    /// </summary>
    public interface IJoystick
    {
        /// <summary>
        /// 조이스틱의 입력 방향 벡터를 가져온다.
        /// </summary>
        /// <returns>입력 방향 벡터</returns>
        System.Numerics.Vector2 GetInputVector();

        /// <summary>
        /// 사용자가 현재 조이스틱을 조작 중인지 확인한다.
        /// </summary>
        /// <returns>조작 중이면 true</returns>
        bool IsOnControl();
    }
}
