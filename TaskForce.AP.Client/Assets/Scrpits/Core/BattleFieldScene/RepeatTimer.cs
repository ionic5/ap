using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    /// <summary>
    /// 지정된 간격으로 작업을 반복 실행하는 타이머 클래스.
    /// 지정된 횟수만큼 작업을 반복 수행한 후 자동으로 중지된다.
    /// </summary>
    public class RepeatTimer
    {
        /// <summary>내부 타이머</summary>
        private readonly Core.Timer _timer;

        /// <summary>반복 실행할 작업</summary>
        private Action _task;
        /// <summary>반복 간격 (초)</summary>
        private float _intervalSeconds;
        /// <summary>남은 반복 횟수</summary>
        private int _remainingRepeats;

        /// <summary>
        /// RepeatTimer의 생성자.
        /// </summary>
        /// <param name="timer">내부 타이머</param>
        public RepeatTimer(Core.Timer timer)
        {
            _timer = timer;
        }

        /// <summary>
        /// 반복 타이머를 시작한다.
        /// </summary>
        /// <param name="task">반복 실행할 작업</param>
        /// <param name="intervalSeconds">반복 간격 (초)</param>
        /// <param name="repeatCount">총 반복 횟수</param>
        public void Start(Action task, float intervalSeconds, int repeatCount)
        {
            _task = task;
            _intervalSeconds = intervalSeconds;
            _remainingRepeats = repeatCount;

            ExecuteStep();
        }

        private void ExecuteStep()
        {
            if (_remainingRepeats <= 0)
            {
                Stop();
                return;
            }

            _task?.Invoke();
            _remainingRepeats--;

            if (_remainingRepeats > 0)
                _timer.Start(0, _intervalSeconds, ExecuteStep);
            else
                Stop();
        }

        /// <summary>
        /// 반복 타이머를 중지하고 상태를 초기화한다.
        /// </summary>
        public void Stop()
        {
            _task = null;
            _intervalSeconds = 0.0f;
            _remainingRepeats = 0;
            _timer.Stop(0);
        }
    }
}
