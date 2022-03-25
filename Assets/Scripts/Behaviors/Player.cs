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
    [RequireComponent(typeof(Controller2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class Player : MonoBehaviour, IPlayer
    {
        //  Private fields
        public const float Gravity = -64f;
        private const float MoveSpeed = 10f;
        private const float JumpVelocity = 22f;
        private readonly IInputManager _inputManager;
        private Controller2D _controller2D;
        private Vector3 _velocity;
        private bool _isJumping;
        
        //  Unity properties;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        
        public Player()
        {
            _inputManager = new InputManager();
        }
        
        public void Start()
        {
            _controller2D = GetComponent<Controller2D>();
        }

        private bool CanJump()
        {
            return _controller2D.Collisions.Below;
        }
        
        private bool IsJumpPressed()
        {
            return _inputManager.IsJumpPressed;
        }

        public void Update()
        {
            if (_controller2D.Collisions.Above || _controller2D.Collisions.Below)
                _velocity.y = Movements.None;
            
            Vector2 movementDirection = _inputManager.Direction;

            if (CanJump() && IsJumpPressed())
            {
                _isJumping = true;
                _velocity.y = JumpVelocity;
            }

            _velocity.x = movementDirection.x * MoveSpeed;
            _velocity.y += Gravity * Time.deltaTime;
            
            _controller2D.Move(_velocity * Time.deltaTime);

            //  Process the animations.
            if (_isJumping)
            {
                if (_controller2D.Collisions.Below)
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