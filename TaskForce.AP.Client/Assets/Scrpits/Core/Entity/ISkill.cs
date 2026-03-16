namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 스킬의 기본 인터페이스. 스킬 식별, 레벨 관리, 소유자 설정 등
    /// 모든 스킬이 공통으로 제공해야 하는 기능을 정의한다.
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// 스킬의 고유 식별자를 반환한다.
        /// </summary>
        /// <returns>스킬 ID 문자열.</returns>
        string GetSkillID();

        /// <summary>
        /// 스킬 아이콘의 리소스 ID를 반환한다.
        /// </summary>
        /// <returns>아이콘 ID 문자열.</returns>
        string GetIconID();

        /// <summary>
        /// 스킬의 표시 이름을 반환한다.
        /// </summary>
        /// <returns>스킬 이름 문자열.</returns>
        string GetName();

        /// <summary>
        /// 스킬의 현재 레벨을 반환한다.
        /// </summary>
        /// <returns>현재 레벨 값.</returns>
        int GetLevel();

        /// <summary>
        /// 스킬의 레벨을 설정한다.
        /// </summary>
        /// <param name="value">설정할 레벨 값.</param>
        void SetLevel(int value);

        /// <summary>
        /// 스킬의 소유 유닛을 설정한다.
        /// </summary>
        /// <param name="unit">소유자로 설정할 유닛.</param>
        void SetOwner(Unit unit);

        /// <summary>
        /// 스킬을 소유 유닛에 추가한다.
        /// </summary>
        void AddToOwner();

        /// <summary>
        /// 스킬의 레벨을 1 올린다.
        /// </summary>
        void LevelUp();
    }
}
