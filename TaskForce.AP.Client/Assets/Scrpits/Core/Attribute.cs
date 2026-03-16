using System;

namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 문자열, 정수, 실수 값을 동시에 보관할 수 있는 다형 속성 구조체.
    /// 하나의 값을 여러 타입으로 변환하여 조회할 수 있다.
    /// </summary>
    public struct Attribute
    {
        /// <summary>문자열 형태로 저장된 속성 값.</summary>
        private readonly string _stringValue;

        /// <summary>정수 형태로 저장된 속성 값.</summary>
        private readonly int _intValue;

        /// <summary>실수 형태로 저장된 속성 값.</summary>
        private readonly float _floatValue;

        /// <summary>
        /// 문자열 값으로 속성을 초기화한다.
        /// </summary>
        /// <param name="stringValue">저장할 문자열 값.</param>
        public Attribute(string stringValue)
        {
            _stringValue = stringValue;
            _intValue = 0;
            _floatValue = 0.0f;
        }

        /// <summary>
        /// 정수 값으로 속성을 초기화한다. 실수 및 문자열 표현도 함께 생성된다.
        /// </summary>
        /// <param name="value">저장할 정수 값.</param>
        public Attribute(int value)
        {
            _intValue = value;
            _floatValue = value;
            _stringValue = value.ToString();
        }

        /// <summary>
        /// 실수 값으로 속성을 초기화한다. 정수(내림) 및 문자열 표현도 함께 생성된다.
        /// </summary>
        /// <param name="value">저장할 실수 값.</param>
        public Attribute(float value)
        {
            _floatValue = value;
            _intValue = (int)Math.Floor(value);
            _stringValue = value.ToString();
        }

        /// <summary>
        /// 속성 값을 실수(float)로 반환한다.
        /// </summary>
        /// <returns>실수 형태의 속성 값.</returns>
        public float AsFloat()
        {
            return _floatValue;
        }

        /// <summary>
        /// 속성 값을 정수(int)로 반환한다.
        /// </summary>
        /// <returns>정수 형태의 속성 값.</returns>
        public int AsInt()
        {
            return _intValue;
        }

        /// <summary>
        /// 속성 값을 문자열(string)로 반환한다.
        /// </summary>
        /// <returns>문자열 형태의 속성 값.</returns>
        public string AsString()
        {
            return _stringValue;
        }
    }
}
