using System;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario
{
    public class GoombasBehavior : MonoBehaviour
    {
        private Vector3 _velocity = Vector3.zero;
        private float _horizontalMovement;
        private PlayerBehavior _playerBehavior;
        
        public bool _jumpTriggered;
        public bool _isGrounded;
        public bool _mustAttack;
        public bool _mustJump;
        public Vector2 _distanceFromPlayer;
        public bool _isWinner;

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

            _playerBehavior ??= FindObjectOfType<PlayerBehavior>();
            
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);

            Vector2 playerPosition = _playerBehavior.transform.position;
            Vector2 goombasPosition = characterRigidbody.position;
            
            _distanceFromPlayer = playerPosition - goombasPosition;
            _mustAttack = _distanceFromPlayer.magnitude < minDistanceToAttack;

            if (_mustAttack)
            {
                float horizontalAxis = _distanceFromPlayer.normalized.x > 0f ? 1f : -1f;
                float deltaTime = Time.fixedDeltaTime;
                _horizontalMovement = horizontalAxis * moveSpeed * deltaTime;
            }
            else
            {
                _horizontalMovement = 0f;
            }

            _mustJump = playerPosition.y > (goombasPosition.y + 1f);
            
            if (_mustJump && _isGrounded)
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
            if (otherCollision2D.gameObject.CompareTag("Player"))
            {
                _isWinner = true;
                var playerBehavior = otherCollision2D.transform.GetComponent<PlayerBehavior>();
                playerBehavior.Kill();
            }
        }
    }
}
