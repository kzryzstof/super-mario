// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 19:29
// ==========================================================================

using System;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Services;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{
    [RequireComponent(typeof(CharacterBehavior))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PlayerBehavior : MonoBehaviour, IPlayer
    {
        //  Private fields
        private readonly float _gravity;
        private readonly float _jumpVelocity;
        private const float JumpHeight = 4.5f;
        private const float TimeToJumpApex = 0.4f;

        private const float MoveSpeed = 10f;
        private readonly IInputManager _inputManager;
        private CharacterBehavior _characterBehavior;
        private Vector3 _velocity;
        private bool _isJumping;
        
        private float _smoothedVelocityX;
        private const float AccelerationTimeAirborne = 0.2f;
        private const float AccelerationTimeGrounded = 0.1f;
        
        //  Unity properties;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        
        //  Debug
        public float debugGravity;
        
        public PlayerBehavior()
        {
            _inputManager = new InputManager();
            
            _gravity = debugGravity = -(2f * JumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
            _jumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
        }
        
        public void Start()
        {
            _characterBehavior = GetComponent<CharacterBehavior>();
        }

        private bool CanJump()
        {
            return _characterBehavior.Collisions.Below && !_characterBehavior.Collisions.Above;
        }
        
        private bool IsJumpPressed()
        {
            return _inputManager.IsJumpPressed;
        }

        public void Update()
        {
            if (_characterBehavior.Collisions.Above || _characterBehavior.Collisions.Below)
                _velocity.y = Movements.None;
            
            Vector2 movementDirection = _inputManager.Direction;

            if (CanJump() && IsJumpPressed())
            {
                _isJumping = true;
                _velocity.y = _jumpVelocity;
            }

            float targetVelocityX = movementDirection.x * MoveSpeed;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _smoothedVelocityX, _characterBehavior.Collisions.Below ? AccelerationTimeGrounded : AccelerationTimeAirborne);
            
            _velocity.y += _gravity * Time.deltaTime;
            
            _characterBehavior.Move(_velocity * Time.deltaTime);

            ProcessAnimations();
        }

        private void ProcessAnimations()
        {
            //  Process the animations.
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
        
        public Vector2 Position => transform.position;
    }
}