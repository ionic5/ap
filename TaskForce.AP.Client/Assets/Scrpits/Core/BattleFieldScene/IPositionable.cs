using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 2D 위치를 가지는 객체를 나타내는 인터페이스.
    /// 위치 설정 및 조회 기능을 제공한다.
    /// </summary>
    public interface IPositionable
    {
        /// <summary>
        /// 위치를 설정한다.
        /// </summary>
        /// <param name="vector2">설정할 위치 벡터</param>
        void SetPosition(Vector2 vector2);

        /// <summary>
        /// 현재 위치를 가져온다.
        /// </summary>
        /// <returns>현재 위치 벡터</returns>
        Vector2 GetPosition();
    }
}
