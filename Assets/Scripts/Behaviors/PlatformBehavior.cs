// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 25/03/2022 @ 19:04
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Services;
using NoSuchCompany.Games.SuperMario.Services.Impl;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{ 
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class PlatformBehavior : MonoBehaviour
    {
        //  Private fields.
        private const ushort DefaultRayCount = 6;
        private const float DefaultRayLength = 0.5f;
        private readonly IRaycaster _raycaster;
        private readonly Guid _id;
        private BoxCollider2D _boxCollider2D;
        
        //  Unity injected properties.
        public Vector2 move;
        public LayerMask characterMask;
        public int rayCount;
        public ScrollingMode scrollingMode;
        public float minOffset;
        public float maxOffset;
        
        public PlatformBehavior()
        {
            _id = Guid.NewGuid();
            _raycaster = new Raycaster();
        }
        
        public void Start()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _raycaster.Initialize(_boxCollider2D, rayCount == 0 ? DefaultRayCount : rayCount);
        }

        public void Update()
        {
            ProcessReset();
            
            //Vector3 platformVelocity = move * Time.deltaTime;
            Vector3 platformVelocity = MovePlatform();
            
            MovePassengers(platformVelocity);
            
            AppLogger.Write(LogsLevels.MovingPlatforms, $"Before: [{transform.position.x},{transform.position.y}] ([{platformVelocity.x},{platformVelocity.y}]))");
            
            transform.Translate(platformVelocity);
            
            AppLogger.Write(LogsLevels.MovingPlatforms, $"After: [{transform.position.x},{transform.position.y}]");
        }

        private void MovePassengers(Vector2 platformVelocity)
        {
            //if (platformVelocity.x == Movements.None && platformVelocity.y == Movements.None)
           //     return;
            
            List<IRaycastCollision> raycastHits = _raycaster.FindVerticalHitsOnly(platformVelocity, characterMask, DefaultRayLength).ToList();

            float platformDirectionX = Mathf.Sign(platformVelocity.x);
            float platformDirectionY = Mathf.Sign(platformVelocity.y);
            
            foreach (IRaycastCollision raycastHit in raycastHits)
            {
                var translateX = 0f;
                var translateY = 0f;

                if (platformVelocity.y != Movements.None)
                    translateY = platformDirectionY == Directions.Upward ? platformVelocity.y - raycastHit.Distance * platformDirectionY : 0f;

                if (platformVelocity.x != Movements.None)
                    translateX = platformVelocity.x;
                
                raycastHit.Transform.Translate(new Vector2(translateX, translateY));
            }
            
            /*  Used to push the 
            if (platformVelocity.x != 0f)
            {
                foreach (IRaycastCollision raycastHit in raycastHits)
                {
                    float translateX = platformVelocity.x - raycastHit.Distance * platformDirectionY;;
                    float translateY = Movements.None;
                    
                    raycastHit.Transform.Translate(new Vector2(translateX, translateY));
                }
            }
            */
            
        }

        private bool _hasReset;

        private void ProcessReset()
        {
            if (!_hasReset)
                return;

            _boxCollider2D.enabled = true;
            _hasReset = false;
        }
        
        private Vector3 ResetPosition(float translateX, float translateY)
        {
            _boxCollider2D.enabled = false;
            _hasReset = true;
            return new Vector3(translateX, translateY);
        }
        
        private Vector3 MovePlatform()
        {
            Vector3 platformVelocity = move * Time.deltaTime;

            if (scrollingMode == ScrollingMode.Continuous)
            {
                if (Mathf.Sign(move.y) == Directions.Downward)
                {
                    if (transform.position.y < minOffset)
                        platformVelocity = ResetPosition(Movements.None, maxOffset - minOffset);
                }
                else
                {
                    if (transform.position.y > maxOffset)
                        platformVelocity = ResetPosition(Movements.None, minOffset - maxOffset);
                }
            }

            return platformVelocity;
        }
    }
}