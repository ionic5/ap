using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class Explosion : PoolableObject, IExplosion
    {
        [SerializeField]
        private Animator _animator;

        public event EventHandler ExplosionFinishedEvent;
        public event EventHandler<BatchObjectHitEventArgs> BatchObjectHitEvent;

        void IExplosion.Start(float explosionRadius)
        {
            _animator.Play("explosion");

            var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            BatchObjectHitEvent?.Invoke(this, new BatchObjectHitEventArgs(colliders.Select(entry => entry.gameObject.name)));
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, position.Y, transform.position.z);
        }

        public void OnExplosionFinished()
        {
            ExplosionFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        protected override void CleanUp()
        {
            base.CleanUp();

            ExplosionFinishedEvent = null;
            BatchObjectHitEvent = null;
        }
    }
}
