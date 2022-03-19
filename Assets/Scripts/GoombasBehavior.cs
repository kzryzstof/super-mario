// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:05
// ==========================================================================

using System;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Extensions;
using NoSuchCompany.Games.SuperMario.Strategies.Enemy;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario
{
    #region Class

    public class GoombasBehavior : MonoBehaviour, IEnemy
    {
        private const float NoMovement = 0f;
        
        //  Defines field (public for debuggability in the editor)
        public bool _jumpTriggered;

        public bool _isGrounded;

        public bool _isWinner;

        public bool _isRightSideBlocked;

        public bool _isLeftSideBlocked;

        public string _collidedWith;

        public float _horizontalMovement;

        public string _name;

        //  Defines configurable properties.
        public float jumpForce;

        public float moveSpeed;

        public Rigidbody2D characterRigidbody;

        public Transform groundCheck;

        public Transform leftSideCheck;

        public Transform rightSideCheck;

        public float checkRadius;

        public LayerMask collisionLayers;

        public Animator goombasAnimator;

        public SpriteRenderer spriteRenderer;

        public float minDistanceToAttack;

        private EnemyContext _enemyContext;

        private Vector3 _velocity = Vector3.zero;

        public float MoveSpeed => moveSpeed;

        public Vector2 Position => transform.position;
        
        public GoombasBehavior()
        {
            _jumpTriggered = false;
        }

        public void Update()
        {
            if (_isWinner)
            {
                _horizontalMovement = 0f;
                return;
            }

            var currentPlayer = FindObjectOfType<PlayerBehavior>();

            if (currentPlayer == null)
                return;
            
            _enemyContext ??= new EnemyContext(this, currentPlayer);
            _enemyContext.Do();
        }

        public void FixedUpdate()
        {
            //  Reserved for physics. No inputs check.

            InternalMove(_horizontalMovement);
        }

        public EnemySurroundings GetSurroundings()
        {
            return EnemySurroundings.Get(this);
        }

        public void StandStill()
        {
            Move(NoMovement);
        }

        private void OnCollisionEnter2D(Collision2D otherCollision2D)
        {
            if (otherCollision2D.gameObject.CompareTag(Tags.Player))
            {
                _isWinner = true;
                var playerBehavior = otherCollision2D.transform.GetComponent<PlayerBehavior>();
                playerBehavior.Kill();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _enemyContext != null && _enemyContext.IsAttacking ? Color.red : Color.green;

            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
            Gizmos.DrawWireSphere(leftSideCheck.position, checkRadius);
            Gizmos.DrawWireSphere(rightSideCheck.position, checkRadius);
            Gizmos.DrawWireSphere(transform.position, minDistanceToAttack);
        }

        public void Jump()
        {
            _isGrounded = IsGrounded();

            if (_isGrounded)
                _jumpTriggered = true;
        }

        public void Move(float horizontalMovement)
        {
            _horizontalMovement = horizontalMovement;
        }

        public bool IsBlocked(float horizontalMovement)
        {
            _isLeftSideBlocked = false;
            _isRightSideBlocked = false;

            if (!horizontalMovement.IsMoving())
                return false;

            if (horizontalMovement < -0.1f && IsLeftSideBlocked())
            {
                _isLeftSideBlocked = true;
                _isRightSideBlocked = IsRightSideBlocked();
                return true;
            }

            if (horizontalMovement > 0.1f && IsRightSideBlocked())
            {
                _isRightSideBlocked = true;
                _isLeftSideBlocked = IsLeftSideBlocked();
                return true;
            }

            return false;
        }

        public bool IsGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, checkRadius, collisionLayers);
        }

        public bool IsLeftSideBlocked()
        {
            return Physics2D.OverlapCircle(leftSideCheck.position, checkRadius, collisionLayers);
        }

        public bool IsRightSideBlocked()
        {
            return Physics2D.OverlapCircle(rightSideCheck.position, checkRadius, collisionLayers);
        }

        private void InternalMove(float horizontalMovement)
        {
            const float SmoothTime = 0.05f;

            Vector3 targetVelocity = new Vector2(horizontalMovement, characterRigidbody.velocity.y);

            characterRigidbody.velocity = Vector3.SmoothDamp(characterRigidbody.velocity, targetVelocity, ref _velocity, SmoothTime);

            if (_jumpTriggered)
            {
                characterRigidbody.AddForce(new Vector2(0f, jumpForce));
                _jumpTriggered = false;
            }

            Flip(characterRigidbody.velocity.x);

            goombasAnimator.SetFloat("Speed", Math.Abs(characterRigidbody.velocity.x));
        }

        private void Flip(float characterVelocity)
        {
            spriteRenderer.flipX = characterVelocity < -0.1f;
        }
    }

    #endregion
}