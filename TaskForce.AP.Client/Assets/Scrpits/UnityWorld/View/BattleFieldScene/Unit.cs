using System;
using System.Collections.Generic;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.UnityWorld.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class Unit : PoolableObject, Core.View.BattleFieldScene.IUnit, IDestroyable, IFollowable
    {
        public event EventHandler DieAnimationFinishedEvent;
        public event EventHandler MoveDirectionChangedEvent;

        [SerializeField]
        private Rigidbody2D _rigidbody2D;
        [SerializeField]
        private float _arrivalThreshold;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private GameObject _effectAreaCenter;
        [SerializeField]
        private float _pathUpdateThreshold;
        [SerializeField]
        private float _pathUpdateCooltime;

        public PathFinder PathFinder;
        public Func<FloatingTextAnimator> CreateFloatingTextAnimator;
        public Core.Timer Timer;

        private Vector2 _velocity;
        private float _speed;
        private System.Numerics.Vector2 _position;
        private System.Numerics.Vector2 _direction;
        private System.Numerics.Vector2 _moveDirection;

        private int _pathIndex;
        private Vector3 _pathPoint;
        private Vector3 _destination;
        private bool _updatePathRequired;
        private List<Vector2> _path;
        private bool _isDestinationSetted;

        private IReadOnlyDictionary<UnitMotionID, string> _clipNameMap;

        private void Awake()
        {
            _velocity = new Vector2();
            _destination = Vector3.zero;
            _path = new List<Vector2>();
            _pathPoint = new Vector3();
            _position = new System.Numerics.Vector2();
            _pathIndex = 0;
            _speed = 0.0f;
            _updatePathRequired = false;
            _isDestinationSetted = false;
            _isDestroyed = false;

            var clipNameMap = new Dictionary<UnitMotionID, string>
            {
                { UnitMotionID.Attack, "attack" },
                { UnitMotionID.Stand, "idle" },
                { UnitMotionID.Die, "die" },
                { UnitMotionID.Move, "walk" },
                { UnitMotionID.Cast, "cast" }
            };
            _clipNameMap = clipNameMap;
        }

        public void MoveTo(System.Numerics.Vector2 position, float speed)
        {
            StopMove();

            var destination = new Vector3(position.X, position.Y, transform.position.z);
            if (!_isDestinationSetted)
            {
                _isDestinationSetted = true;
                _destination = destination;

                UpdatePath();
            }
            else if (Vector3.Distance(_destination, destination) > _pathUpdateThreshold)
            {
                _destination = destination;

                if (Timer.IsRunning(0))
                    _updatePathRequired = true;
                else
                    UpdatePath();
            }

            _speed = speed;

            _rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        }

        private void UpdatePath()
        {
            Timer.Stop(0);
            Timer.Start(0, _pathUpdateCooltime, () =>
            {
                if (!_updatePathRequired)
                    return;
                UpdatePath();
            });

            _path = PathFinder.FindPath(_rigidbody2D.position, _destination);
            _pathIndex = 0;
        }

        public void Move(System.Numerics.Vector2 velocity)
        {
            StopMoveTo();

            _velocity.x = velocity.X;
            _velocity.y = velocity.Y;

            _rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        }

        private void StopMoveTo()
        {
            _destination = Vector3.zero;
            _path.Clear();
            _pathIndex = 0;
            _speed = 0.0f;
            _pathPoint = new Vector3();
            _updatePathRequired = false;
            _isDestinationSetted = false;
            Timer.Stop(0);
        }

        private void StopMove()
        {
            _velocity = Vector2.zero;
        }

        public void Stop()
        {
            StopMove();
            StopMoveTo();

            _rigidbody2D.constraints |= RigidbodyConstraints2D.FreezePosition;
        }

        private void FixedUpdate()
        {
            if (_path.Count > 0)
                FollowPath();
            else if (_velocity != Vector2.zero)
                ApplyVelocity(_velocity);
        }

        private void FollowPath()
        {
            if (_pathIndex >= _path.Count)
                return;

            Vector2 pathPoint = _path[_pathIndex];
            _pathPoint.x = pathPoint.x;
            _pathPoint.y = pathPoint.y;
            _pathPoint.z = transform.position.z;

            Vector2 direction = (_pathPoint - transform.position).normalized;
            ApplyVelocity(direction * _speed);

            if (Vector3.Distance(transform.position, _pathPoint) <= _arrivalThreshold)
            {
                _pathIndex++;
                if (_pathIndex >= _path.Count)
                    if (_updatePathRequired)
                        UpdatePath();
                    else
                        Stop();
            }
        }

        private void ApplyVelocity(Vector2 velocity)
        {
            if (_rigidbody2D.linearVelocity.normalized != velocity.normalized)
                MoveDirectionChangedEvent?.Invoke(this, EventArgs.Empty);

            _rigidbody2D.linearVelocity = velocity;
        }

        private void PlayAnimation(string clipName, float duration = 1.0f, bool forceRestart = false)
        {
            AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(0);

            bool isSameClip = currentState.IsName(clipName);
            if (isSameClip && !forceRestart)
                return;

            ApplyDirection(_animator, _direction);

            _animator.speed = 1.0f / duration;
            _animator.Play(clipName, 0, 0f);
        }

        public System.Numerics.Vector2 GetPosition()
        {
            _position.X = _rigidbody2D.position.x;
            _position.Y = _rigidbody2D.position.y;

            return _position;
        }

        public void PlayDamageAnimation(int damage)
        {
            var animator = CreateFloatingTextAnimator.Invoke();
            var offset = _effectAreaCenter.transform.localPosition;
            animator.Follow(this, new System.Numerics.Vector2(offset.x, offset.y));
            animator.PlayDamageAnimation(damage);
            animator.BringToTop();

            EventHandler hdlr = null;
            hdlr = (sender, args) =>
            {
                animator.AnimationFinishedEvent -= hdlr;
                animator.Destroy();
            };
            animator.AnimationFinishedEvent += hdlr;
        }

        protected override void CleanUp()
        {
            base.CleanUp();

            DieAnimationFinishedEvent = null;
            PathFinder = null;
            CreateFloatingTextAnimator = null;
            Timer.Destroy();
            Timer = null;
        }

        public void OnDieAnimationFinished()
        {
            DieAnimationFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            _rigidbody2D.position = new Vector2(position.X, position.Y);
        }

        public System.Numerics.Vector2 GetDirection()
        {
            return _direction;
        }

        public System.Numerics.Vector2 GetMoveDirection()
        {
            var velocity = _rigidbody2D.linearVelocity;
            float length = MathF.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);

            if (length > 0f)
            {
                _moveDirection.X = velocity.x / length;
                _moveDirection.Y = velocity.y / length;
            }
            else
            {
                _moveDirection = System.Numerics.Vector2.Zero;
            }

            return _moveDirection;
        }

        public void SetDirection(System.Numerics.Vector2 direction)
        {
            _direction = direction;

            ApplyDirection(_animator, _direction);
        }

        private void ApplyDirection(Animator animator, System.Numerics.Vector2 direction)
        {
            float dim2X = (float)Math.Round(direction.X);
            float dim2Y = (float)Math.Round(direction.Y);
            animator.SetFloat("dim2X", dim2X);
            animator.SetFloat("dim2Y", dim2Y);

            float dim1X = (float)Math.Round((direction.X + 1f) * 0.5f);
            animator.SetFloat("dim1X", dim1X);
        }

        public void PlayMotion(UnitMotionID motionID, System.Numerics.Vector2 direction, float playTime, bool forceRestart)
        {
            SetDirection(direction);
            PlayAnimation(_clipNameMap[motionID], playTime, forceRestart);
        }

        public void PlayMotion(UnitMotionID motionID)
        {
            PlayMotion(motionID, _direction, 1.0f, false);
        }

        public string GetObjectID()
        {
            return gameObject.name;
        }

        public void PlayHealAnimation(int healAmount)
        {
            var animator = CreateFloatingTextAnimator.Invoke();
            var offset = _effectAreaCenter.transform.localPosition;
            animator.Follow(this, new System.Numerics.Vector2(offset.x, offset.y));
            animator.PlayDamageAnimation(healAmount);
            animator.BringToTop();

            EventHandler hdlr = null;
            hdlr = (sender, args) =>
            {
                animator.AnimationFinishedEvent -= hdlr;
                animator.Destroy();
            };
            animator.AnimationFinishedEvent += hdlr;
        }
    }
}