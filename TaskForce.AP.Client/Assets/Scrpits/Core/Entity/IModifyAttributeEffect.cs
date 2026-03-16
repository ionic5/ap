namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 속성 변경 효과를 나타내는 인터페이스. 속성 저장소에 효과를 적용하고,
    /// 동일 유형의 효과끼리 병합할 수 있는 기능을 정의한다.
    /// </summary>
    public interface IModifyAttributeEffect
    {
        /// <summary>
        /// 효과의 적용 순서를 반환한다. 낮은 값이 먼저 적용된다.
        /// </summary>
        /// <returns>적용 순서 값.</returns>
        int GetApplyOrder();

        /// <summary>
        /// 속성 저장소에 이 효과를 적용하여 속성 값을 변경한다.
        /// </summary>
        /// <param name="store">효과를 적용할 속성 저장소.</param>
        void Apply(AttributeStore store);

        /// <summary>
        /// 주어진 효과와 병합 가능한지 판별한다.
        /// </summary>
        /// <param name="effect">병합 가능 여부를 확인할 대상 효과.</param>
        /// <returns>병합 가능하면 true, 아니면 false.</returns>
        bool CanMerge(IModifyAttributeEffect effect);

        /// <summary>
        /// 주어진 효과를 이 효과에 병합한다.
        /// </summary>
        /// <param name="effect">병합할 대상 효과.</param>
        void Merge(IModifyAttributeEffect effect);

        /// <summary>
        /// 이 효과의 복제본을 생성하여 반환한다.
        /// </summary>
        /// <returns>복제된 <see cref="IModifyAttributeEffect"/> 인스턴스.</returns>
        IModifyAttributeEffect Clone();
    }
}
