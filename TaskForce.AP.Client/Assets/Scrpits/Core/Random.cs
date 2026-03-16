using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 다양한 타입의 난수 생성 기능을 제공하는 유틸리티 클래스.
    /// 정수, 실수, 2D 좌표 난수 생성 및 배열 셔플을 지원한다.
    /// </summary>
    public class Random
    {
        /// <summary>내부적으로 사용하는 시스템 난수 생성기 인스턴스.</summary>
        private static readonly System.Random _random;

        /// <summary>
        /// 정적 생성자. 시스템 난수 생성기를 초기화한다.
        /// </summary>
        static Random()
        {
            _random = new System.Random();
        }

        /// <summary>
        /// 지정된 범위 내에서 float 난수를 반환한다.
        /// </summary>
        /// <param name="a">범위의 한쪽 끝 값.</param>
        /// <param name="b">범위의 다른 쪽 끝 값.</param>
        /// <returns>a와 b 사이의 float 난수.</returns>
        public float Next(float a, float b)
        {
            return (float)NextDouble(a, b);
        }

        /// <summary>
        /// 0.0 이상 1.0 미만의 double 난수를 반환한다.
        /// </summary>
        /// <returns>0.0 이상 1.0 미만의 double 난수.</returns>
        public double NextDouble()
        {
            return _random.NextDouble();
        }

        /// <summary>
        /// 지정된 범위 내에서 double 난수를 반환한다. a와 b 중 작은 값이 최솟값, 큰 값이 최댓값으로 사용된다.
        /// </summary>
        /// <param name="a">범위의 한쪽 끝 값.</param>
        /// <param name="b">범위의 다른 쪽 끝 값.</param>
        /// <returns>min(a,b) 이상 max(a,b) 미만의 double 난수.</returns>
        public double NextDouble(double a, double b)
        {
            var min = Math.Min(a, b);
            var max = Math.Max(a, b);
            return _random.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// 지정된 범위 내에서 float 난수를 반환한다.
        /// </summary>
        /// <param name="a">범위의 한쪽 끝 값.</param>
        /// <param name="b">범위의 다른 쪽 끝 값.</param>
        /// <returns>a와 b 사이의 float 난수.</returns>
        public float NextFloat(float a, float b)
        {
            return (float)NextDouble(a, b);
        }

        /// <summary>
        /// 0 이상 지정된 최댓값 미만의 정수 난수를 반환한다.
        /// </summary>
        /// <param name="maxValue">반환 값의 배타적 상한.</param>
        /// <returns>0 이상 maxValue 미만의 정수 난수.</returns>
        public int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        /// <summary>
        /// 지정된 범위 내에서 정수 난수를 반환한다.
        /// </summary>
        /// <param name="minValue">반환 값의 포함적 하한.</param>
        /// <param name="maxValue">반환 값의 배타적 상한.</param>
        /// <returns>minValue 이상 maxValue 미만의 정수 난수.</returns>
        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        /// <summary>
        /// 중심점으로부터 지정된 반경 이내의 임의 2D 위치를 반환한다.
        /// </summary>
        /// <param name="center">원의 중심 좌표.</param>
        /// <param name="radius">최대 반경.</param>
        /// <returns>중심으로부터 radius 이내의 무작위 2D 좌표.</returns>
        public Vector2 NextPosition(Vector2 center, float radius)
        {
            return NextPosition(center, 0.0f, radius);
        }

        /// <summary>
        /// 중심점으로부터 최소 반경과 최대 반경 사이의 도넛 영역 내 임의 2D 위치를 반환한다.
        /// 균등 분포를 위해 반경의 제곱 공간에서 샘플링한다.
        /// </summary>
        /// <param name="center">원의 중심 좌표.</param>
        /// <param name="minRadius">최소 반경.</param>
        /// <param name="maxRadius">최대 반경.</param>
        /// <returns>도넛 영역 내의 무작위 2D 좌표.</returns>
        public Vector2 NextPosition(Vector2 center, float minRadius, float maxRadius)
        {
            float theta = Next(0f, 2f * (float)Math.PI);

            float minRSq = minRadius * minRadius;
            float maxRSq = maxRadius * maxRadius;
            float r = (float)Math.Sqrt(Next(minRSq, maxRSq));

            float x = (float)(center.X + r * Math.Cos(theta));
            float y = (float)(center.Y + r * Math.Sin(theta));

            return new Vector2(x, y);
        }

        /// <summary>
        /// Fisher-Yates 알고리즘을 사용하여 배열의 요소를 무작위로 섞는다.
        /// </summary>
        /// <typeparam name="T">배열 요소의 타입.</typeparam>
        /// <param name="array">섞을 대상 배열.</param>
        public void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = _random.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
