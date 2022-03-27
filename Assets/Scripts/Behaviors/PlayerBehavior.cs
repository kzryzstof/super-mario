// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 19:29
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Services;
using NoSuchCompany.Games.SuperMario.Services.Impl;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{
    [RequireComponent(typeof(CharacterBehavior))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PlayerBehavior : MonoBehaviour, IPlayer
    {
        //  Constants
        private readonly float _gravity;
        private readonly float _maximumJumpVelocity;
        private readonly float _minimumJumpVelocity;
        private const float MaximumJumpHeight = 4.5f;
        private const float MinimumJumpHeight = 1f;
        private const float TimeToJumpApex = 0.4f;
        private const float MoveSpeed = 10f;
        private const float AccelerationTimeAirborne = 0.2f;
        private const float AccelerationTimeGrounded = 0.1f;

        //  Private fields
        private readonly IInputManager _inputManager;
        private CharacterBehavior _characterBehavior;
        private Vector3 _velocity;
        private bool _isJumping;
        private bool _isEnemyAttacked;
        private float _smoothedVelocityX;
        
        //  Unity properties;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        
        //  Properties
        public Vector2 Position => transform.position;

        //  Debug
        public float debugGravity;
        
        public PlayerBehavior()
        {
            _inputManager = new PlayerInputManager();
            
            _gravity = debugGravity = -(2f * MaximumJumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
            _maximumJumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
            _minimumJumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(_gravity) * MinimumJumpHeight);
        }
        
        public void Start()
        {
            _characterBehavior = GetComponent<CharacterBehavior>();
        }

        public void Update()
        {
            if (_characterBehavior.Collisions.Above || _characterBehavior.Collisions.Below)
                _velocity.y = Movements.None;

            Vector2 movementDirection = _inputManager.Direction;

            if (InitiateJump())
            {
                _isJumping = true;
                _isEnemyAttacked = false;
                _velocity.y = _maximumJumpVelocity;
            }

            if (AbortJump())
            {
                if (_velocity.y > _minimumJumpVelocity)
                    _velocity.y = _minimumJumpVelocity;
            }

            UpdateVelocity(movementDirection);

            MovePlayer();

            ProcessAnimations();
        }

        public void OnEnemyAttacked()
        {
            _isEnemyAttacked = true;
        }
        
        private void MovePlayer()
        {
            _characterBehavior.Move(_velocity * Time.deltaTime);
        }

        private void UpdateVelocity(Vector2 movementDirection)
        {
            float targetVelocityX = movementDirection.x * MoveSpeed;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _smoothedVelocityX, _characterBehavior.Collisions.Below ? AccelerationTimeGrounded : AccelerationTimeAirborne);

            _velocity.y += _gravity * Time.deltaTime;
        }

        private bool InitiateJump()
        {
            return (CanJump() && IsJumpPressed()) || _isEnemyAttacked;
        }
        
        private bool AbortJump()
        {
            return !CanJump() && !IsJumpPressed();
        }
        
        private bool CanJump()
        {
            return _characterBehavior.Collisions.Below && !_characterBehavior.Collisions.Above;
        }
        
        private bool IsJumpPressed()
        {
            return _inputManager.IsJumpPressed;
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
        }

        private void Flip(float characterVelocity)
        {
            if (Mathf.Abs(characterVelocity) < 0.3f)
                return;
            
            spriteRenderer.flipX = characterVelocity < -0.1f;
        }
    }
}