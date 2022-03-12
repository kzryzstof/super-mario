using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoSuchCompany.Games.SuperMario
{
    public class GoombasBehavior : MonoBehaviour
    {
        private Vector3 _velocity = Vector3.zero;

        private float _horizontalMovement;

        [FormerlySerializedAs("JumpForce")]
        public float jumpForce;

        [FormerlySerializedAs("MoveSpeed")]
        public float moveSpeed;

        [FormerlySerializedAs("PlayerGameObject")]
        public GameObject playerGameObject;

        [FormerlySerializedAs("CharacterRigidbody")]
        public Rigidbody2D characterRigidbody;

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

        [FormerlySerializedAs("GoombasAnimator")]
        public Animator goombasAnimator;

        [FormerlySerializedAs("SpriteRenderer")]
        public SpriteRenderer spriteRenderer;

        [FormerlySerializedAs("MustAttack")]
        public bool mustAttack;

        [FormerlySerializedAs("MustJump")]
        public bool mustJump;

        [FormerlySerializedAs("Distance")]
        public Vector2 distance;

        [FormerlySerializedAs("MinimumDistanceToAttack")]
        public float minDistanceToAttack;
        
        [FormerlySerializedAs("ContactWith")]
        public string contactWith;

        [FormerlySerializedAs("IsWinner")]
        public bool isWinner;

        public GoombasBehavior()
        {
            isJumping = false;
        }

        public void Update()
        {
            if (isWinner)
            {
                _horizontalMovement = 0f;
                return;
            }
            
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);

            Vector2 playerPosition = playerGameObject.transform.position;
            Vector2 goombasPosition = characterRigidbody.position;
            
            distance = playerPosition - goombasPosition;
            mustAttack = distance.magnitude < minDistanceToAttack;

            if (mustAttack)
            {
                float horizontalAxis = distance.normalized.x > 0f ? 1f : -1f;
                float deltaTime = Time.fixedDeltaTime;
                _horizontalMovement = horizontalAxis * moveSpeed * deltaTime;
            }
            else
            {
                _horizontalMovement = 0f;
            }

            mustJump = playerPosition.y > (goombasPosition.y + 1f);
            
            if (mustJump && isGrounded)
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

            Vector3 targetVelocity = new Vector2(horizontalMovement, characterRigidbody.velocity.y);

            characterRigidbody.velocity = Vector3.SmoothDamp(characterRigidbody.velocity, targetVelocity, ref _velocity, SmoothTime);

            if (isJumping)
            {
                characterRigidbody.AddForce(new Vector2(0f, jumpForce));
                isJumping = false;
            }

            Flip(characterRigidbody.velocity.x);

            goombasAnimator.SetFloat("Speed", Math.Abs(characterRigidbody.velocity.x));
            goombasAnimator.SetBool("IsJumping", !isGrounded);
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
            if (otherCollision2D.gameObject.CompareTag("Player"))
            {
                isWinner = true;
                var playerBehavior = otherCollision2D.transform.GetComponent<MovePlayerBehavior>();
                playerBehavior.Kill();
            }
        }
    }
}
