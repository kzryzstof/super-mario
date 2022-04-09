// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 19:29
// ==========================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Extensions;
using NoSuchCompany.Games.SuperMario.Services;
using NoSuchCompany.Games.SuperMario.Services.Impl;
using NoSuchCompany.Games.SuperMario.Services.Jumping;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{
    [RequireComponent(typeof(CharacterBehavior))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PlayerBehavior : MonoBehaviour, IPlayer
    {
        //  Constants
        private readonly JumpController _jumpController;
        private const float MoveSpeed = 5f;
        private const float AccelerationTimeAirborne = 0.2f;
        private const float AccelerationTimeGrounded = 0.1f;

        //  Private fields
        private readonly IInputManager _inputManager;
        private CharacterBehavior _characterBehavior;
        private Vector3 _velocity;
        private float _smoothedVelocityX;
        private bool _isDead;
        
        //  Unity properties;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        
        //  Properties
        public Vector2 Position => transform.position;

        public PlayerBehavior()
        {
            _inputManager = new PlayerInputManager();
            _jumpController = new JumpController(_inputManager);
        }
        
        public void Start()
        {
            _characterBehavior = GetComponent<CharacterBehavior>();
            _jumpController.Initialize(_characterBehavior);
        }
        
        public void Update()
        {
            _jumpController.Update(ref _velocity);
            
            if (IsAttacked())
                DieAsync().FireAndForget();

            MovePlayer();
            
            _jumpController.PostUpdate();

            UpdateAnimations();
        }

        public void OnEnemyAttacked()
        {
            _jumpController.OnEnemyAttacked();
        }
        
        private async Task DieAsync()
        {
            if (_isDead)
                return;
            
            _velocity = Vector3.zero;
            _isDead = true;
            _characterBehavior.Kill(false);
            
            await Task.Delay(TimeSpan.FromSeconds(2d)).ConfigureAwait(true);
                
            LevelManager.Instance.ReloadCurrentLevel();
        }
        
        private void MovePlayer()
        {
            if (_isDead)
                return;
                    
            Vector2 movementDirection = _inputManager.Direction;
            bool isRunning = _inputManager.IsRunPressed;
            float runningFactor = isRunning ? 1.8f : 1f;
            
            _velocity.x = Mathf.SmoothDamp(_velocity.x, movementDirection.x * runningFactor * MoveSpeed, ref _smoothedVelocityX, _characterBehavior.Collisions.Below ? AccelerationTimeGrounded : AccelerationTimeAirborne);

            _characterBehavior.Move(_velocity * Time.deltaTime);
        }

        private void UpdateAnimations()
        {
            Flip(_velocity.x);
            animator.SetFloat(Animations.Speed, Mathf.Abs(_velocity.x));
            animator.SetBool(Animations.IsJumping, _jumpController.IsJumping);
            animator.SetBool(Animations.IsDead, _isDead);
        }

        private void Flip(float characterVelocity)
        {
            if (Mathf.Abs(characterVelocity) < 0.3f)
                return;
            
            spriteRenderer.flipX = characterVelocity < -0.1f;
        }
        
        private bool IsAttacked()
        {
            return
                _characterBehavior.Collisions.LeftCollisions.Any(tag => string.Equals(tag, Tags.Enemy))
                || _characterBehavior.Collisions.RightCollisions.Any(tag => string.Equals(tag, Tags.Enemy))
                || _characterBehavior.Collisions.AboveCollisions.Any(tag => string.Equals(tag, Tags.Enemy));
        }
    }
}