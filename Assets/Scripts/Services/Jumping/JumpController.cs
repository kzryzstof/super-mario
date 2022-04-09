// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 06/04/2022 @ 19:11
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Behaviors;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Extensions;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Services.Jumping
{
    internal sealed class JumpController
    {
        private readonly IInputManager _inputManager;
        private readonly JumpCooldown _jumpCooldown;
        private CharacterBehavior _characterBehavior;
        private bool _useJumpingAnimation;
        private bool _canJumpAgain = true;
        private bool _isEnemyAttacked;
        private bool _isCollidingAbove;
        private bool _isCollidingBelow;
        
        private readonly float _gravity;
        private readonly float _maximumJumpVelocity;
        private readonly float _minimumJumpVelocity;
        private const float MaximumJumpHeight = 4.0f;
        private const float MinimumJumpHeight = 1f;
        private const float TimeToJumpApex = 0.36f;

        public bool IsJumping => _useJumpingAnimation;
        
        public JumpController(IInputManager inputManager)
        {
            _inputManager = inputManager;
            _jumpCooldown = new JumpCooldown();
            
            _gravity = -(2f * MaximumJumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
            _maximumJumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
            _minimumJumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(_gravity) * MinimumJumpHeight);
        }

        public void Initialize(CharacterBehavior characterBehavior)
        {
            _characterBehavior = characterBehavior;
        }

        public void Update(ref Vector3 velocity)
        {
            _jumpCooldown.PreUpdate();

            //  Makes a copy of the current collisions
            GetCurrentCollisions();
            
            //  Reset the velocity depending on the direction & collisions.
            CheckCollisions(ref velocity);

            if (!_isCollidingAbove)
            {
                if (CanInitiateJump())
                {
                    _useJumpingAnimation = true;
                    _isEnemyAttacked = false;
                    velocity.y = _maximumJumpVelocity;
                }

                if (AbortJump())
                {
                    if (velocity.y > _minimumJumpVelocity)
                        velocity.y = _minimumJumpVelocity;
                }
            }

            ApplyGravity(ref velocity);
        }
        
        public void PostUpdate()
        {
            if (_useJumpingAnimation)
            {
                _jumpCooldown.PostUpdate(_characterBehavior.Collisions);

                if (_characterBehavior.Collisions.Below)
                {
                    _useJumpingAnimation = false;
                }
                else
                {
                    _canJumpAgain = false;
                }
            }
            
            if (_characterBehavior.Collisions.Below && !_canJumpAgain && !_inputManager.IsJumpPressed)
                _canJumpAgain = true;
        }

        public void OnEnemyAttacked()
        {
            _isEnemyAttacked = true;
        }
        
        /// <summary>
        /// Makes a copy of all the actual collisions.
        /// </summary>
        private void GetCurrentCollisions()
        {
            _isCollidingBelow = _characterBehavior.Collisions.Below;
            _isCollidingAbove = _characterBehavior.Collisions.Above;
        }

        /// <summary>
        /// Alters the velocity based on the collisions (above or below).
        /// </summary>
        /// <param name="velocity"></param>
        private void CheckCollisions(ref Vector3 velocity)
        {
            if (_isCollidingAbove)
            {
                //  Character can't continue ascending if it is colliding with an obstacle above it.
                velocity.y = velocity.IsGoingUp() ? Movements.None : velocity.y;
                return;
            }

            if (_isCollidingBelow)
            {
                //  Character can't continue descending if it is colliding with an obstacle below it.
                velocity.y = velocity.IsGoingDown() ? Movements.None : velocity.y;
            }
        }

        private void ApplyGravity(ref Vector3 velocity)
        {
            velocity.y += _gravity * Time.deltaTime;
        }
        
        private bool CanInitiateJump()
        {
            bool isPlayerGrounded = IsPlayerGrounded();
            bool isJumpPressed = IsJumpPressed();
            bool isEnemyAttacked = _isEnemyAttacked;
            
            bool canInitiateJump =
            (
                isPlayerGrounded && 
                isJumpPressed && 
                !_jumpCooldown.IsActivated && 
                _canJumpAgain
            ) || isEnemyAttacked;

            if (canInitiateJump)
                AppLogger.Write(LogsLevels.JumpController, $"canJump = {isPlayerGrounded} | isJumpPressed = {isJumpPressed} | isEnemyAttacked = {isEnemyAttacked} | jumpCooldown = {_jumpCooldown.IsActivated}");

            return canInitiateJump;
        }
        
        private bool AbortJump()
        {
            return !IsPlayerGrounded() && !IsJumpPressed();
        }
        
        private bool IsPlayerGrounded()
        {
            return _isCollidingBelow && !_isCollidingAbove;
        }
        
        private bool IsJumpPressed()
        {
            return _inputManager.IsJumpPressed;
        }
    }
}