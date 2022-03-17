using System;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Extensions;
using NoSuchCompany.Games.SuperMario.Helpers;
using NoSuchCompany.Games.SuperMario.Strategies;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario
{
    public class GoombasBehavior : MonoBehaviour
    {
        private readonly HorizontalMovementHistory _horizontalMovementHistory;

        private AnalyzeSurroundings _analyzeSurroundings;
        
        private Vector3 _velocity = Vector3.zero;
        
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
        
        public GoombasBehavior()
        {
            _jumpTriggered = false;
            _horizontalMovementHistory = new HorizontalMovementHistory();
        }

        public void Update()
        {
            if (_isWinner)
            {
                _horizontalMovement = 0f;
                return;
            }

            _analyzeSurroundings ??= new AnalyzeSurroundings(this);
            _analyzeSurroundings.Process();
        }
        
        public void FixedUpdate()
        {
            //  Reserved for physics. No inputs check.

            InternalMove(_horizontalMovement);
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
        
        public bool IsBlocked(bool mustAttack, float horizontalMovement)
        {
            _isLeftSideBlocked = false;
            _isRightSideBlocked = false;

            if (!mustAttack)
                return false;

            if (!horizontalMovement.IsMoving())
                return false;

            if (horizontalMovement < -0.1f && IsLeftSideBlocked())
            {
                _isLeftSideBlocked = true;
                _isRightSideBlocked = IsRightSideBlocked();
                return true;
            }
            else if (horizontalMovement > 0.1f && IsRightSideBlocked())
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

        private void OnDrawGizmos()
        {
            Gizmos.color = _analyzeSurroundings != null && _analyzeSurroundings.MustAttack ? Color.red : Color.green;
            
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
            Gizmos.DrawWireSphere(leftSideCheck.position, checkRadius);
            Gizmos.DrawWireSphere(rightSideCheck.position, checkRadius);
            Gizmos.DrawWireSphere(transform.position, minDistanceToAttack);
        }
        
        private void OnCollisionEnter2D(Collision2D otherCollision2D)
        {
            if (otherCollision2D.gameObject.CompareTag(Constants.Tags.Player))
            {
                _isWinner = true;
                var playerBehavior = otherCollision2D.transform.GetComponent<PlayerBehavior>();
                playerBehavior.Kill();
            }
        }
    }
}
