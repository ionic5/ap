using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 소울(Soul) 객체를 생성하는 팩토리 클래스.
    /// 소울 뷰를 생성하고 소울 객체를 초기화한 후 생성 이벤트를 발생시킨다.
    /// </summary>
    public class SoulFactory
    {
        /// <summary>소울 생성 시 발생하는 이벤트</summary>
        public event EventHandler<CreatedEventArgs<Soul>> SoulCreatedEvent;

        /// <summary>소울 뷰 생성 팩토리 함수</summary>
        private readonly Func<View.BattleFieldScene.ISoul> _createSoul;

        /// <summary>
        /// SoulFactory의 생성자.
        /// </summary>
        /// <param name="createSoul">소울 뷰 생성 팩토리 함수</param>
        public SoulFactory(Func<View.BattleFieldScene.ISoul> createSoul)
        {
            _createSoul = createSoul;
        }

        /// <summary>
        /// 지정된 레벨의 소울을 생성한다.
        /// </summary>
        /// <param name="power">소울의 레벨(파워)</param>
        /// <returns>생성된 소울 객체</returns>
        public Soul Create(int power)
        {
            var view = _createSoul();

            var soul = new Soul(view, power);

            SoulCreatedEvent.Invoke(this, new CreatedEventArgs<Soul>(soul));

            return soul;
        }
    }
}
