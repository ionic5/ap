namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 업데이트 루프를 관리하는 인터페이스.
    /// <see cref="IUpdatable"/> 객체를 등록하거나 제거하여 매 프레임 갱신 대상을 제어한다.
    /// </summary>
    public interface ILoop
    {
        /// <summary>
        /// 업데이트 루프에 갱신 대상 객체를 추가한다.
        /// </summary>
        /// <param name="updatable">매 프레임 갱신할 객체.</param>
        void Add(IUpdatable updatable);

        /// <summary>
        /// 업데이트 루프에서 갱신 대상 객체를 제거한다.
        /// </summary>
        /// <param name="updatable">제거할 갱신 대상 객체.</param>
        void Remove(IUpdatable updatable);
    }
}
