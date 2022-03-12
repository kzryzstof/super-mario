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

        [FormerlySerializedAs("TestOutput")]
        public float testOutput;

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

            testOutput = deltaTime;
            
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
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
