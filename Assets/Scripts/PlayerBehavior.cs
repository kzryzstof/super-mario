using System;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario
{
    public class PlayerBehavior : MonoBehaviour
    {
        private Vector3 _velocity = Vector3.zero;
        private float _horizontalMovement;
        public bool _jumpTriggered;
        public bool _isGrounded;
        public bool _isDead;
        
        public float jumpForce;

        public float moveSpeed;

        public Rigidbody2D playerRigidbody;

        public Transform groundCheck;

        public float groundCheckRadius;

        public LayerMask collisionLayers;

        public Animator playerAnimator;

        public SpriteRenderer spriteRenderer;

        public PlayerBehavior()
        {
            _jumpTriggered = false;
        }

        public void Update()
        {
            if (_isDead)
            {
                _horizontalMovement = 0f;
                return;
            }
            
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);

            float horizontalAxis = Input.GetAxis("Horizontal");
            float deltaTime = Time.fixedDeltaTime;
            
            _horizontalMovement = horizontalAxis * moveSpeed * deltaTime;
            
            if ((Input.GetButtonDown("Jump") || Input.GetKeyDown("joystick button 0")) && _isGrounded)
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

            Vector3 targetVelocity = new Vector2(horizontalMovement, playerRigidbody.velocity.y);

            playerRigidbody.velocity = Vector3.SmoothDamp(playerRigidbody.velocity, targetVelocity, ref _velocity, SmoothTime);

            if (_jumpTriggered)
            {
                playerRigidbody.AddForce(new Vector2(0f, jumpForce));
                _jumpTriggered = false;
            }
            
            Flip(playerRigidbody.velocity.x);
            
            playerAnimator.SetFloat("Speed", Math.Abs(playerRigidbody.velocity.x));
            playerAnimator.SetBool("IsJumping", !_isGrounded);
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

        public void Kill()
        {
            _isDead = true;
            playerAnimator.SetBool("IsDead", true);
        }

    }
}
