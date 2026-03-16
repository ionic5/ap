using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    /// <summary>
    /// 한 번만 재생되는 이펙트를 표현하는 뷰 인터페이스
    /// </summary>
    public interface IOneShotEffect
    {
        /// <summary>
        /// 이펙트가 지정한 대상을 추적하도록 설정한다.
        /// </summary>
        /// <param name="target">추적할 대상 오브젝트</param>
        void Follow(IFollowable target);

        /// <summary>
        /// 이펙트를 재생한다.
        /// </summary>
        void Play();

        /// <summary>
        /// 이펙트의 위치를 설정한다.
        /// </summary>
        /// <param name="vector2">설정할 2D 위치 좌표</param>
        void SetPosition(Vector2 vector2);
    }
}
