// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 19:29
// ==========================================================================

using System.Linq;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Services.Impl;
using NoSuchCompany.Games.SuperMario.Strategies.Enemy;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{
    [RequireComponent(typeof(CharacterBehavior))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class EnemyBehavior : MonoBehaviour, IEnemy
    {
        //  Constants
        private readonly float _gravity;
        private readonly float _jumpVelocity;
        private const float JumpHeight = 2.5f;
        private const float TimeToJumpApex = 0.4f;
        private readonly EnemyInputManager _inputManager;
        private const float AccelerationTimeAirborne = 0.2f;
        private const float AccelerationTimeGrounded = 0.1f;

        //  Private fields
        private EnemyContext _enemyContext;
        private CharacterBehavior _characterBehavior;
        private Vector3 _velocity;
        private bool _isJumping;
        private bool _isDead;
        private float _smoothedVelocityX;
        private IPlayer _player;
        
        //  Unity properties;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        public float minDistanceToAttack;
        public float moveSpeed;
        public bool supportsJumpScare;
        
        //  Properties.
        public float MoveSpeed => moveSpeed;
        public float MinimumDistanceToAttack => minDistanceToAttack;
        public Vector2 Position => transform.position;
        public bool SupportsJumpScare => supportsJumpScare;
        
        //  Debug
        public float debugGravity;
        
        public EnemyBehavior()
        {
            _inputManager = new EnemyInputManager();
            
            _gravity = debugGravity = -(2f * JumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
            _jumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
        }
        
        public void Start()
        {
            _characterBehavior = GetComponent<CharacterBehavior>();
        }

        public void Update()
        {
            if (false == IsPlayerReady())
                return;
            
            if (_isDead)
                return;
            
            //  Finds out what to do and updates the input manager.
            _enemyContext.Think();

            //  Determines the velocity based on the requested inputs.
            if (_characterBehavior.Collisions.Above || _characterBehavior.Collisions.Below)
                _velocity.y = Movements.None;

            if (IsAttacked())
                Die();

            if (IsJumpRequested())
                TriggerJump();

            UpdateHorizontalVelocity();
            UpdateVerticalVelocity();
            
            //  Adjusts the velocity based on the collisions and move the character.
            Move();

            //  Reflects the motion in the animation.
            ProcessAnimations();
        }

        public EnemySurroundings GetSurroundings()
        {
            return EnemySurroundings.Get(this);
        }

        public bool IsBlocked(float horizontalMovement)
        {
            return Mathf.Sign(horizontalMovement) switch
            {
                Directions.Left => _characterBehavior.Collisions.Left,
                Directions.Right => _characterBehavior.Collisions.Right,
                _ => false
            };
        }

        private void Move()
        {
            _characterBehavior.Move(_velocity * Time.deltaTime);
        }

        private void UpdateVerticalVelocity()
        {
            _velocity.y += _gravity * Time.deltaTime;
        }

        private void UpdateHorizontalVelocity()
        {
            Vector2 movementDirection = !_isDead ? _inputManager.Direction : Vector2.zero;

            float targetVelocityX = movementDirection.x * MoveSpeed;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _smoothedVelocityX, _characterBehavior.Collisions.Below ? AccelerationTimeGrounded : AccelerationTimeAirborne);
        }

        private void TriggerJump()
        {
            _isJumping = true;
            _velocity.y = _jumpVelocity;
        }

        private bool IsJumpRequested()
        {
            return CanJump() && IsJumpPressed();
        }

        private void Die()
        {
            _velocity.x = Movements.None;
            _isDead = true;
            _characterBehavior.Kill();
            _player.OnEnemyAttacked();
        }

        private bool IsAttacked()
        {
            return _characterBehavior.Collisions.AboveCollisions.Any(tag => string.Equals(tag, Tags.Player));
        }
        
        private bool IsPlayerReady()
        {
            _player = FindObjectOfType<PlayerBehavior>();

            if (_player == null)
                return false;

            _enemyContext ??= new EnemyContext(this, _player, _inputManager);
            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _enemyContext != null && _enemyContext.IsAttacking ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, minDistanceToAttack);
        }
        
        private void ProcessAnimations()
        {
            if (_isJumping)
            {
                if (_characterBehavior.Collisions.Below)
                    _isJumping = false;

                animator.SetBool("IsJumping", _isJumping);
            }

            Flip(_velocity.x);
            animator.SetFloat("Speed", Mathf.Abs(_velocity.x));
            animator.SetBool("IsDead", _isDead);
        }

        private void Flip(float characterVelocity)
        {
            if (Mathf.Abs(characterVelocity) < 0.3f)
                return;
            
            spriteRenderer.flipX = characterVelocity < -0.1f;
        }

        private bool CanJump()
        {
            return _characterBehavior.Collisions.Below && !_characterBehavior.Collisions.Above;
        }
        
        private bool IsJumpPressed()
        {
            return _inputManager.IsJumpPressed;
        }
    }
}