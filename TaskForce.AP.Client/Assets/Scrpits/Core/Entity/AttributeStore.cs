using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 속성(Attribute)을 키-값 쌍으로 저장하고 관리하는 저장소 클래스.
    /// </summary>
    public class AttributeStore
    {
        /// <summary>속성 ID를 키로, Attribute를 값으로 저장하는 딕셔너리.</summary>
        private readonly Dictionary<string, Attribute> _attributes;

        /// <summary>
        /// AttributeStore의 생성자. 빈 속성 딕셔너리를 초기화한다.
        /// </summary>
        public AttributeStore()
        {
            _attributes = new Dictionary<string, Attribute>();
        }

        /// <summary>
        /// 지정된 ID에 속성 값을 설정한다. 이미 존재하는 경우 덮어쓴다.
        /// </summary>
        /// <param name="id">속성의 고유 식별자.</param>
        /// <param name="attribute">설정할 속성 값.</param>
        public void Set(string id, Attribute attribute)
        {
            _attributes[id] = attribute;
        }

        /// <summary>
        /// 지정된 ID에 해당하는 속성 값을 반환한다. 존재하지 않으면 기본 Attribute를 반환한다.
        /// </summary>
        /// <param name="id">조회할 속성의 고유 식별자.</param>
        /// <returns>해당 속성 값 또는 기본 <see cref="Attribute"/> 객체.</returns>
        public Attribute Get(string id)
        {
            if (_attributes.TryGetValue(id, out var value))
                return value;
            return new Attribute();
        }

        /// <summary>
        /// 저장된 모든 속성을 제거한다.
        /// </summary>
        public void Clear()
        {
            _attributes.Clear();
        }

        /// <summary>
        /// 현재 저장소의 모든 속성을 다른 AttributeStore에 복사한다.
        /// </summary>
        /// <param name="other">속성을 복사할 대상 저장소.</param>
        public void CopyTo(AttributeStore other)
        {
            foreach (var entry in _attributes)
                other.Set(entry.Key, entry.Value);
        }
    }
}
