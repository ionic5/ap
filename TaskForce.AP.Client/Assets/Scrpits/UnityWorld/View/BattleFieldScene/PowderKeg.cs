using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class PowderKeg : PoolableObject, IPowderKeg
    {
        public event EventHandler<BatchObjectDetectedEventArgs> BatchObjectDetectedEvent;
        public event EventHandler ExplosionEvent;

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private float _watchInterval;
        [SerializeField]
        private float _minBlinkInterval;
        [SerializeField]
        private float _maxBlinkInterval;

        private bool _isWatching;
        private float _watchRadius;
        private float _lastCheckTime;
        private float _nextBlinkTime;

        protected override void CleanUp()
        {
            base.CleanUp();

            BatchObjectDetectedEvent = null;
            ExplosionEvent = null;
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, position.Y, transform.position.z);
        }

        public System.Numerics.Vector2 GetPosition()
        {
            var pos = transform.position;
            return new System.Numerics.Vector2(pos.x, pos.y);
        }

        public void Watch(float watchRadius)
        {
            _isWatching = true;
            _watchRadius = watchRadius;
            _lastCheckTime = 0.0f;

            _animator.Play("watch");
            DetermineNextBlinkTime();
        }

        public void Ignite()
        {
            _isWatching = false;

            _animator.Play("ignite");
        }

        public void OnIgniteFinished()
        {
            ExplosionEvent?.Invoke(this, EventArgs.Empty);
        }

        private void FixedUpdate()
        {
            if (!_isWatching)
                return;

            if (UnityEngine.Time.time >= _nextBlinkTime)
            {
                _animator.SetTrigger("blink");
                DetermineNextBlinkTime();
            }

            if (UnityEngine.Time.fixedTime - _lastCheckTime < _watchInterval) return;
            _lastCheckTime = UnityEngine.Time.fixedTime;

            var colliders = Physics2D.OverlapCircleAll(transform.position, _watchRadius);
            if (colliders.Length > 0)
                BatchObjectDetectedEvent?.Invoke(this, new BatchObjectDetectedEventArgs(colliders.Select(entry => entry.gameObject.name)));
        }

        private void DetermineNextBlinkTime()
        {
            _nextBlinkTime = UnityEngine.Time.time + UnityEngine.Random.Range(_minBlinkInterval, _maxBlinkInterval);
        }
    }
}
