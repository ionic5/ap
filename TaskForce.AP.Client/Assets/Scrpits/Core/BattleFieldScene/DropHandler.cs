using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 유닛 사망 시 소울(Soul) 드롭을 처리하는 클래스.
    /// 유닛 생성 이벤트를 구독하여 해당 유닛이 사망하면 확률적으로 소울을 생성한다.
    /// </summary>
    public class DropHandler
    {
        /// <summary>소울 생성 팩토리</summary>
        private readonly SoulFactory _soulFactory;
        /// <summary>난수 생성기</summary>
        private readonly Core.Random _random;
        /// <summary>게임 데이터 저장소</summary>
        private readonly GameDataStore _gameDataStore;

        /// <summary>
        /// DropHandler의 생성자.
        /// </summary>
        /// <param name="soulFactory">소울 생성 팩토리</param>
        /// <param name="random">난수 생성기</param>
        /// <param name="gameDataStore">게임 데이터 저장소</param>
        public DropHandler(SoulFactory soulFactory, Random random, GameDataStore gameDataStore)
        {
            _soulFactory = soulFactory;
            _random = random;
            _gameDataStore = gameDataStore;
        }

        /// <summary>
        /// 유닛 생성 이벤트 핸들러. 생성된 유닛의 사망 이벤트를 구독한다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="args">생성된 유닛 정보를 담은 이벤트 인자</param>
        public void OnUnitCreatedEvent(object sender, CreatedEventArgs<Unit> args)
        {
            var newUnit = args.CreatedObject;

            EventHandler<DiedEventArgs> hdlr = null;
            hdlr = (sender, args) =>
            {
                OnUnitDiedEvent(sender, args);
                newUnit.DiedEvent -= hdlr;
            };
            newUnit.DiedEvent += hdlr;
        }

        /// <summary>
        /// 유닛 사망 이벤트 핸들러. 드롭 확률에 따라 소울을 생성하고 사망 위치에 배치한다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="args">사망한 유닛 정보를 담은 이벤트 인자</param>
        public void OnUnitDiedEvent(object sender, DiedEventArgs args)
        {
            var dropRate = _gameDataStore.GetSoulDropRate();
            if (_random.Next(0.0f, 100.0f) >= dropRate)
                return;

            var soul = _soulFactory.Create(1);
            soul.SetPosition(args.DiedTarget.GetPosition());
        }
    }
}
