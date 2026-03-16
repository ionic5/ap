using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 인덱스 기반의 다중 타이머를 관리하는 클래스.
    /// 각 타이머는 지정된 시간이 경과하면 콜백을 호출하며, 업데이트 루프에 자동 등록/해제된다.
    /// </summary>
    public class Timer : IUpdatable, IDestroyable
    {
        /// <summary>
        /// 타이머 객체가 파괴될 때 발생하는 이벤트.
        /// </summary>
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        /// <summary>시간 정보를 제공하는 인터페이스.</summary>
        private readonly ITime _time;

        /// <summary>업데이트 루프 관리 인터페이스.</summary>
        private readonly ILoop _loop;

        /// <summary>인덱스를 키로 하여 타이머 항목을 저장하는 딕셔너리.</summary>
        private readonly Dictionary<int, TimerEntry> _timerEntries;

        /// <summary>현재 활성 상태인 타이머 인덱스 목록.</summary>
        private readonly List<int> _activeIndexes;

        /// <summary>현재 프레임에서 만료된 타이머 인덱스 목록.</summary>
        private readonly List<int> _finishedIndexes;

        /// <summary>타이머가 파괴되었는지 여부.</summary>
        private bool _isDestroyed;

        /// <summary>
        /// 개별 타이머의 상태 정보를 담는 내부 구조체.
        /// </summary>
        private struct TimerEntry
        {
            /// <summary>타이머를 식별하는 인덱스.</summary>
            public int Index;

            /// <summary>남은 시간(초).</summary>
            public float RemainTime;

            /// <summary>시간 만료 시 호출되는 콜백.</summary>
            public Action TimeOutHandler;
        }

        /// <summary>
        /// <see cref="Timer"/>의 새 인스턴스를 생성한다.
        /// </summary>
        /// <param name="time">델타 타임 등 시간 정보를 제공하는 인스턴스.</param>
        /// <param name="loop">타이머를 업데이트 루프에 등록/해제하기 위한 루프 관리자.</param>
        public Timer(ITime time, ILoop loop)
        {
            _time = time;
            _loop = loop;
            _isDestroyed = false;

            _activeIndexes = new List<int>();
            _timerEntries = new Dictionary<int, TimerEntry>();
            _finishedIndexes = new List<int>();
        }

        /// <summary>
        /// 지정된 인덱스의 타이머가 현재 실행 중인지 확인한다.
        /// </summary>
        /// <param name="index">확인할 타이머 인덱스.</param>
        /// <returns>실행 중이면 true, 아니면 false.</returns>
        public bool IsRunning(int index)
        {
            return _timerEntries.ContainsKey(index);
        }

        /// <summary>
        /// 지정된 인덱스로 타이머를 시작한다. 이미 존재하는 인덱스이면 타이머를 재설정한다.
        /// 첫 번째 타이머가 시작될 때 자동으로 업데이트 루프에 등록된다.
        /// </summary>
        /// <param name="index">타이머를 식별하는 인덱스.</param>
        /// <param name="time">타이머 만료까지의 시간(초).</param>
        /// <param name="timeOutHandler">만료 시 호출할 콜백 (선택 사항).</param>
        public void Start(int index, float time, Action timeOutHandler = null)
        {
            if (_isDestroyed) return;

            if (!_timerEntries.ContainsKey(index))
            {
                _activeIndexes.Add(index);

                if (_timerEntries.Count == 0)
                    _loop.Add(this);
            }

            _timerEntries[index] = new TimerEntry
            {
                Index = index,
                RemainTime = time,
                TimeOutHandler = timeOutHandler
            };
        }

        /// <summary>
        /// 모든 활성 타이머를 중지하고 제거한다.
        /// </summary>
        public void Stop()
        {
            _finishedIndexes.AddRange(_timerEntries.Keys);
            foreach (var idx in _finishedIndexes)
                RemoveTimerEntry(idx);
            _finishedIndexes.Clear();
        }

        /// <summary>
        /// 지정된 인덱스의 타이머를 중지하고 제거한다.
        /// </summary>
        /// <param name="index">중지할 타이머 인덱스.</param>
        public void Stop(int index)
        {
            RemoveTimerEntry(index);
        }

        /// <summary>
        /// 타이머 항목을 내부 컬렉션에서 제거한다. 모든 타이머가 제거되면 업데이트 루프에서 해제된다.
        /// </summary>
        /// <param name="index">제거할 타이머 인덱스.</param>
        private void RemoveTimerEntry(int index)
        {
            if (_timerEntries.Remove(index))
            {
                _activeIndexes.Remove(index);

                if (_timerEntries.Count == 0)
                    _loop.Remove(this);
            }
        }

        /// <summary>
        /// 매 프레임 호출되어 모든 활성 타이머의 남은 시간을 감소시킨다.
        /// 만료된 타이머는 제거 후 등록된 콜백을 호출한다.
        /// </summary>
        public void Update()
        {
            var deltaTime = _time.GetDeltaTime();

            for (int i = 0; i < _activeIndexes.Count; i++)
            {
                int key = _activeIndexes[i];
                var entry = _timerEntries[key];

                entry.RemainTime -= deltaTime;

                if (entry.RemainTime <= 0.0f)
                    _finishedIndexes.Add(key);
                else
                    _timerEntries[key] = entry;
            }

            if (_finishedIndexes.Count > 0)
            {
                for (int i = 0; i < _finishedIndexes.Count; i++)
                {
                    int idx = _finishedIndexes[i];
                    if (_timerEntries.TryGetValue(idx, out var entry))
                    {
                        RemoveTimerEntry(idx);
                        entry.TimeOutHandler?.Invoke();
                    }
                }
                _finishedIndexes.Clear();
            }
        }

        /// <summary>
        /// 타이머를 파괴하고 모든 타이머 항목 및 리소스를 정리한다.
        /// 파괴 이벤트를 발생시킨 뒤 업데이트 루프에서 해제된다.
        /// </summary>
        public void Destroy()
        {
            if (_isDestroyed) return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            if (_timerEntries.Count > 0)
            {
                _loop.Remove(this);
                _timerEntries.Clear();
            }

            _activeIndexes.Clear();
            _finishedIndexes.Clear();
        }
    }
}
