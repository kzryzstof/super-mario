using System.Collections.Generic;
using System.Linq;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Services;
using NoSuchCompany.Games.SuperMario.Services.Impl;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{
    public class SurpriseBoxBehavior : MonoBehaviour
    {
        //  Constants
        private static readonly Vector2 HitDetection = new Vector2(0f, -0.5f);
        private readonly IRaycaster _raycaster;

        //  Private fields
        private bool _isHit;
        
        //  Unity DI
        public Animator animator;
        public LayerMask collisionMask;
        
        public SurpriseBoxBehavior()
        {
            _raycaster = new Raycaster();
        }

        public void Start()
        {
            var boxCollider2D = GetComponent<BoxCollider2D>();
            _raycaster.Initialize(boxCollider2D, 3);
            
            animator.SetBool(Animations.IsActive, true);
        }

        public void Update()
        {
            Vector2 hitDetection = HitDetection;

            IEnumerable<IRaycastCollision> collisions = _raycaster.FindVerticalHitsOnly(hitDetection, collisionMask, 0.05f);

            if (!collisions.Any() || _isHit)
                return;

            AppLogger.Write(LogsLevels.None, "HIT!");
            
            _isHit = true;
            animator.SetBool(Animations.IsActive, false);
        }
    }
}
