using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    /// <summary>
    /// Unity 조이스틱 입력을 래핑하여 IJoystick 인터페이스를 구현하는 클래스.
    /// 플레이어 캐릭터의 이동 방향 입력을 처리한다.
    /// </summary>
    public class Joystick : IJoystick
    {
        /// <summary>Unity 조이스틱 컴포넌트 참조</summary>
        private readonly global::Joystick _joystick;
        /// <summary>현재 입력 방향 벡터 (재사용을 위해 캐싱)</summary>
        private Vector2 _inputVector;

        /// <summary>
        /// Joystick 인스턴스를 생성한다.
        /// </summary>
        /// <param name="joystick">래핑할 Unity 조이스틱 컴포넌트</param>
        public Joystick(global::Joystick joystick)
        {
            _inputVector = new Vector2();
            _joystick = joystick;
        }

        /// <summary>
        /// 현재 조이스틱의 입력 방향 벡터를 반환한다.
        /// </summary>
        /// <returns>수평/수직 입력값이 포함된 2D 벡터</returns>
        public Vector2 GetInputVector()
        {
            _inputVector.Y = _joystick.Vertical;
            _inputVector.X = _joystick.Horizontal;

            return _inputVector;
        }

        /// <summary>
        /// 현재 조이스틱이 조작 중인지 여부를 반환한다.
        /// </summary>
        /// <returns>수평 또는 수직 입력값이 0이 아니면 true</returns>
        public bool IsOnControl()
        {
            return _joystick.Vertical != 0.0f || _joystick.Horizontal != 0.0f;
        }
    }
}
