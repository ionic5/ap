using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    /// <summary>
    /// 전장에서 영혼(Soul) 오브젝트를 관리하고 반경 기반 탐색 기능을 제공하는 클래스.
    /// ISoulFinder 인터페이스를 구현하며, 영혼의 생성/파괴 이벤트를 수신하여 내부 목록을 관리한다.
    /// </summary>
    public class SoulFinder : ISoulFinder
    {
        /// <summary>관리 중인 영혼 목록</summary>
        private readonly List<Soul> _souls;

        /// <summary>
        /// SoulFinder 인스턴스를 생성한다.
        /// </summary>
        public SoulFinder()
        {
            _souls = new List<Soul>();
        }

        /// <summary>
        /// 지정된 중심점과 반경 내에 있는 모든 영혼을 탐색하여 결과 리스트에 추가한다.
        /// </summary>
        /// <param name="center">탐색 중심 좌표</param>
        /// <param name="radius">탐색 반경</param>
        /// <param name="results">탐색 결과가 추가될 리스트</param>
        /// <returns>탐색된 영혼의 수</returns>
        public int FindRadius(Vector2 center, float radius, List<Soul> results)
        {
            var detected = FindInRadius(center, radius, (entry) => true);
            results.AddRange(detected);
            return detected.Count();
        }

        /// <summary>
        /// 지정된 중심점과 반경 내에서 조건에 부합하는 영혼을 탐색한다.
        /// AABB 사전 검사 후 거리 제곱 비교로 정밀 판정한다.
        /// </summary>
        /// <param name="center">탐색 중심 좌표</param>
        /// <param name="radius">탐색 반경</param>
        /// <param name="predicate">추가 필터 조건</param>
        /// <returns>조건에 부합하는 영혼 컬렉션</returns>
        public IEnumerable<Soul> FindInRadius(System.Numerics.Vector2 center, float radius, Func<Soul, bool> predicate)
        {
            float cx = center.X;
            float cy = center.Y;
            float r = radius;
            float rSqr = r * r;

            var detected = _souls.Where(predicate).Where(obj =>
            {
                var pos = obj.GetPosition();
                float dx = pos.X - cx;
                float dy = pos.Y - cy;

                if (dx < -r || dx > r || dy < -r || dy > r)
                    return false;

                return dx * dx + dy * dy <= rSqr;
            });

            return detected;
        }

        /// <summary>
        /// 영혼 생성 이벤트 핸들러. 생성된 영혼을 내부 목록에 등록하고 파괴 이벤트를 구독한다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="args">생성된 영혼 정보를 포함하는 이벤트 인자</param>
        public void OnSoulCreatedEvent(object sender, CreatedEventArgs<Soul> args)
        {
            var soul = args.CreatedObject;

            _souls.Add(soul);
            soul.DestroyEvent += OnDestorySoulEvent;
        }

        /// <summary>
        /// 영혼 파괴 이벤트 핸들러. 파괴된 영혼을 내부 목록에서 제거한다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="args">파괴 대상 정보를 포함하는 이벤트 인자</param>
        private void OnDestorySoulEvent(object sender, DestroyEventArgs args)
        {
            var soul = _souls.FirstOrDefault(entry => entry == args.TargetObject);
            if (soul == null)
                return;

            soul.DestroyEvent -= OnDestorySoulEvent;
            _souls.Remove(soul);
        }

        /// <summary>
        /// 모든 영혼의 이벤트 구독을 해제하고 내부 목록을 초기화한다.
        /// </summary>
        public void Destroy()
        {
            foreach (var soul in _souls)
                soul.DestroyEvent -= OnDestorySoulEvent;

            _souls.Clear();
        }
    }
}
