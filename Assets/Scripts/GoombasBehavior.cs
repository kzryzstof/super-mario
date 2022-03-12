using System;
using NoSuchCompany.Games.SuperMario.Entities;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario
{
    public class GoombasBehavior : MonoBehaviour
    {
        private const float NoMovement = 0f;
        private EnemySurroundings _enemySurroundings;
        
        private Vector3 _velocity = Vector3.zero;
        private float _horizontalMovement;
        
        //  Defines field (public for debuggability in the editor)
        public bool _jumpTriggered;
        public bool _isGrounded;
        public bool _isWinner;

        //  Defines configurable properties.
        public float jumpForce;
        public float moveSpeed;
        public Rigidbody2D characterRigidbody;
        public Transform groundCheck;
        public float groundCheckRadius;
        public LayerMask collisionLayers;
        public Animator goombasAnimator;
        public SpriteRenderer spriteRenderer;
        public float minDistanceToAttack;
        
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

            _enemySurroundings ??= EnemySurroundings.Get(this);
            
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);

            _horizontalMovement = _enemySurroundings.MustAttack(minDistanceToAttack) ? _enemySurroundings.MoveTowardPlayer(moveSpeed) : NoMovement;

            if (_enemySurroundings.MustJump() && _isGrounded)
                _jumpTriggered = true;
        }

        public void FixedUpdate()
        {
            //  Reserved for physics. No inputs check.

            Move(_horizontalMovement);
        }

        private void Move(float horizontalMovement)
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
            goombasAnimator.SetBool("IsJumping", !_isGrounded);
        }

        private void Flip(float characterVelocity)
        {
            spriteRenderer.flipX = characterVelocity < -0.1f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
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
