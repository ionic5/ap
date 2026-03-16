namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 활성 스킬을 나타내는 인터페이스. ISkill을 상속하며,
    /// 스킬 고유 속성 값을 조회하는 기능을 추가로 제공한다.
    /// </summary>
    public interface IActiveSkill : ISkill
    {
        /// <summary>
        /// 지정된 속성 ID에 해당하는 속성 값을 반환한다.
        /// </summary>
        /// <param name="attributeID">조회할 속성의 고유 식별자.</param>
        /// <returns>해당 속성 값을 담은 <see cref="Attribute"/> 객체.</returns>
        Attribute GetAttribute(string attributeID);
    }
}
