using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class Sheep : PoolableObject, IMissile
    {
        public event EventHandler ArrivedDestinationEvent;
        public event EventHandler<Core.View.HitEventArgs> HitEvent;

        private Vector3 _destination;
        private bool _hasDestination;

        [SerializeField]
        private Rigidbody2D _rigidbody2D;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private float _arrivalThreshold;

        private void Awake()
        {
            _isDestroyed = false;
            _hasDestination = false;
            _destination = Vector3.zero;
        }

        protected override void CleanUp()
        {
            base.CleanUp();

            ArrivedDestinationEvent = null;
        }

        public void MoveTo(System.Numerics.Vector2 destination, float speed)
        {
            _hasDestination = true;
            _destination = new UnityEngine.Vector3(destination.X, destination.Y, transform.position.z);
            var direction = (_destination - transform.position).normalized;
            _rigidbody2D.linearVelocity = direction * speed;

            _spriteRenderer.flipX = direction.x < 0f;
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            gameObject.transform.position = new Vector3(position.X, position.Y, gameObject.transform.position.z);
        }

        private void Update()
        {
            if (!_hasDestination)
                return;

            if (!IsArrived())
                return;

            _hasDestination = false;
            _rigidbody2D.linearVelocity = Vector2.zero;
            _destination = Vector3.zero;

            ArrivedDestinationEvent?.Invoke(this, EventArgs.Empty);
        }

        private bool IsArrived()
        {
            Vector2 diff = _destination - (Vector3)_rigidbody2D.position;
            float sqrDist = diff.sqrMagnitude;
            return sqrDist < _arrivalThreshold * _arrivalThreshold;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HitEvent?.Invoke(this, new Core.View.HitEventArgs { ObjectID = other.gameObject.name });
        }

        public System.Numerics.Vector2 GetPosition()
        {
            var pos = gameObject.transform.position;
            return new System.Numerics.Vector2(pos.x, pos.y);
        }
    }
}