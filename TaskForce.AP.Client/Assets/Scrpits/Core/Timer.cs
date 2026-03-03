using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core
{
    public class Timer : IUpdatable, IDestroyable
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        private readonly ITime _time;
        private readonly ILoop _loop;
        private readonly Dictionary<int, TimerEntry> _timerEntries;
        private readonly List<int> _activeIndexes;
        private readonly List<int> _finishedIndexes;

        private bool _isDestroyed;

        private struct TimerEntry
        {
            public int Index;
            public float RemainTime;
            public Action TimeOutHandler;
        }

        public Timer(ITime time, ILoop loop)
        {
            _time = time;
            _loop = loop;
            _isDestroyed = false;

            _activeIndexes = new List<int>();
            _timerEntries = new Dictionary<int, TimerEntry>();
            _finishedIndexes = new List<int>();
        }

        public bool IsRunning(int index)
        {
            return _timerEntries.ContainsKey(index);
        }

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

        public void Stop()
        {
            _finishedIndexes.AddRange(_timerEntries.Keys);
            foreach (var idx in _finishedIndexes)
                RemoveTimerEntry(idx);
            _finishedIndexes.Clear();
        }

        public void Stop(int index)
        {
            RemoveTimerEntry(index);
        }

        private void RemoveTimerEntry(int index)
        {
            if (_timerEntries.Remove(index))
            {
                _activeIndexes.Remove(index);

                if (_timerEntries.Count == 0)
                    _loop.Remove(this);
            }
        }

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
