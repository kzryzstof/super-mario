using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoSuchCompany.Games.SuperMario
{
    public class MovePlayerBehavior : MonoBehaviour
    {
        private Vector3 _velocity = Vector3.zero;
        private float _horizontalMovement;
        
        [FormerlySerializedAs("JumpForce")]
        public float jumpForce;

        [FormerlySerializedAs("MoveSpeed")]
        public float moveSpeed;

        [FormerlySerializedAs("PlayerRigidbody")]
        public Rigidbody2D playerRigidbody;

        [FormerlySerializedAs("IsJumping")]
        public bool isJumping;

        [FormerlySerializedAs("IsGrounded")]
        public bool isGrounded;

        [FormerlySerializedAs("GroundCheck")]
        public Transform groundCheck;

        [FormerlySerializedAs("GroundCheckRadius")]
        public float groundCheckRadius;

        [FormerlySerializedAs("CollisionLayers")]
        public LayerMask collisionLayers;

        [FormerlySerializedAs("PlayerAnimator")]
        public Animator playerAnimator;

        [FormerlySerializedAs("SpriteRenderer")]
        public SpriteRenderer spriteRenderer;
        
        public MovePlayerBehavior()
        {
            isJumping = false;
        }

        public void Update()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);

            float horizontalAxis = Input.GetAxis("Horizontal");
            float deltaTime = Time.fixedDeltaTime;
            
            _horizontalMovement = horizontalAxis * moveSpeed * deltaTime;
            
            if (Input.GetButtonDown("Jump") && isGrounded)
                isJumping = true;
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

            if (isJumping)
            {
                playerRigidbody.AddForce(new Vector2(0f, jumpForce));
                isJumping = false;
            }
            
            Flip(playerRigidbody.velocity.x);
            
            playerAnimator.SetFloat("Speed", Math.Abs(playerRigidbody.velocity.x));
            playerAnimator.SetBool("IsJumping", !isGrounded);
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
    }
}
