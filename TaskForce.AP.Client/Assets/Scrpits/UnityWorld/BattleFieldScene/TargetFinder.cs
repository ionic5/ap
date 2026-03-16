using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    /// <summary>
    /// 전장에서 대상(ITarget) 오브젝트를 관리하고, 반경/부채꼴/ID 기반 탐색 기능을 제공하는 클래스.
    /// 대상의 생성, 사망, 파괴 이벤트를 수신하여 내부 목록을 자동으로 관리한다.
    /// </summary>
    public class TargetFinder : ITargetFinder
    {
        /// <summary>관리 중인 대상 목록</summary>
        private readonly List<ITarget> _targets;

        /// <summary>
        /// TargetFinder 인스턴스를 생성한다.
        /// </summary>
        public TargetFinder()
        {
            _targets = new List<ITarget>();
        }

        /// <summary>
        /// 대상 생성 이벤트 핸들러. 생성된 유닛을 내부 목록에 등록하고 사망/파괴 이벤트를 구독한다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="args">생성된 유닛 정보를 포함하는 이벤트 인자</param>
        public void OnTargetCreatedEvent(object sender, CreatedEventArgs<Unit> args)
        {
            var target = args.CreatedObject;

            _targets.Add(target);
            target.DiedEvent += OnTargetDiedEvent;
            target.DestroyEvent += OnDestroyTargetEvent;
        }

        /// <summary>
        /// 지정된 중심점으로부터 최소/최대 반경 사이에 있는 대상을 탐색한다.
        /// AABB 사전 검사 후 거리 제곱 비교로 정밀 판정한다.
        /// </summary>
        /// <param name="center">탐색 중심 좌표</param>
        /// <param name="minRadius">최소 탐색 반경</param>
        /// <param name="maxRadius">최대 탐색 반경</param>
        /// <param name="predicate">추가 필터 조건</param>
        /// <returns>조건에 부합하는 대상 컬렉션</returns>
        public IEnumerable<ITarget> FindInRadius(Vector2 center, float minRadius, float maxRadius, Func<ITarget, bool> predicate)
        {
            float cx = center.X;
            float cy = center.Y;
            float minSqr = minRadius * minRadius;
            float maxSqr = maxRadius * maxRadius;

            var detected = _targets.Where(predicate).Where(obj =>
            {
                var pos = obj.GetPosition();
                float dx = pos.X - cx;
                float dy = pos.Y - cy;

                if (dx < -maxRadius || dx > maxRadius || dy < -maxRadius || dy > maxRadius)
                    return false;

                float distSqr = dx * dx + dy * dy;

                return distSqr >= minSqr && distSqr <= maxSqr;
            });

            return detected;

        }

        /// <summary>
        /// 지정된 중심점과 반경 내에 있는 대상을 탐색한다. 최소 반경은 0으로 설정된다.
        /// </summary>
        /// <param name="center">탐색 중심 좌표</param>
        /// <param name="radius">탐색 반경</param>
        /// <param name="predicate">추가 필터 조건</param>
        /// <returns>조건에 부합하는 대상 컬렉션</returns>
        public IEnumerable<ITarget> FindInRadius(System.Numerics.Vector2 center, float radius, Func<ITarget, bool> predicate)
        {
            return FindInRadius(center, 0.0f, radius, predicate);
        }

        /// <summary>
        /// 지정된 중심점에서 특정 방향의 부채꼴 영역 내 대상을 탐색한다.
        /// 먼저 반경 내 대상을 탐색한 후, 내적(dot product)으로 각도 범위를 필터링한다.
        /// </summary>
        /// <param name="center">부채꼴 중심 좌표</param>
        /// <param name="direction">부채꼴 방향 벡터</param>
        /// <param name="degree">부채꼴 전체 각도 (도 단위)</param>
        /// <param name="radius">부채꼴 반경</param>
        /// <param name="predicate">추가 필터 조건</param>
        /// <returns>조건에 부합하는 대상 컬렉션</returns>
        public IEnumerable<ITarget> FindInSector(System.Numerics.Vector2 center, System.Numerics.Vector2 direction, float degree, float radius, Func<ITarget, bool> predicate)
        {
            var inRadius = FindInRadius(center, radius, predicate);

            var dirNorm = System.Numerics.Vector2.Normalize(direction);
            var cosThreshold = Math.Cos(degree * 0.5f * (Math.PI / 180.0));

            return inRadius.Where(obj =>
            {
                var toTargetNorm = System.Numerics.Vector2.Normalize(obj.GetPosition() - center);
                float cosValue = System.Numerics.Vector2.Dot(dirNorm, toTargetNorm);
                return cosValue >= cosThreshold;
            });
        }

        /// <summary>
        /// 모든 대상의 이벤트 구독을 해제하고 내부 목록을 초기화한다.
        /// </summary>
        public void Destory()
        {
            for (int i = _targets.Count - 1; i >= 0; i--)
                RemoveTarget(_targets[i]);

            _targets.Clear();
        }

        /// <summary>
        /// 뷰 ID로 대상을 검색한다.
        /// </summary>
        /// <param name="objectID">검색할 오브젝트 ID</param>
        /// <returns>일치하는 대상. 없으면 null</returns>
        public ITarget FindByViewID(string objectID)
        {
            return _targets.FirstOrDefault(entry => entry.GetViewID() == objectID);
        }


        /// <summary>
        /// 복수의 뷰 ID로 대상을 검색한다.
        /// </summary>
        /// <param name="objectIDs">검색할 오브젝트 ID 컬렉션</param>
        /// <param name="predicate">추가 필터 조건</param>
        /// <returns>일치하는 대상 컬렉션</returns>
        public IEnumerable<ITarget> FindByViewIDs(IEnumerable<string> objectIDs, Func<ITarget, bool> predicate)
        {
            var ids = new HashSet<string>(objectIDs);
            return _targets.Where(entry => ids.Contains(entry.GetViewID())).Where(predicate);
        }

        /// <summary>
        /// 대상 파괴 이벤트 핸들러. 파괴된 대상을 내부 목록에서 제거한다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="args">파괴 대상 정보를 포함하는 이벤트 인자</param>
        public void OnDestroyTargetEvent(object sender, DestroyEventArgs args)
        {
            var target = _targets.FirstOrDefault(entry => entry == args.TargetObject);
            if (target != null)
                RemoveTarget(target);
        }

        /// <summary>
        /// 대상 사망 이벤트 핸들러. 사망한 대상을 내부 목록에서 제거한다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="args">사망 대상 정보를 포함하는 이벤트 인자</param>
        public void OnTargetDiedEvent(object sender, DiedEventArgs args)
        {
            var target = _targets.FirstOrDefault(entry => entry == args.DiedTarget);
            if (target == null)
                return;

            RemoveTarget(target);
        }

        /// <summary>
        /// 대상의 이벤트 구독을 해제하고 내부 목록에서 제거한다.
        /// </summary>
        /// <param name="target">제거할 대상</param>
        private void RemoveTarget(ITarget target)
        {
            if (!_targets.Contains(target))
                return;

            target.DestroyEvent -= OnDestroyTargetEvent;
            target.DiedEvent -= OnTargetDiedEvent;

            _targets.Remove(target);
        }
    }
}
