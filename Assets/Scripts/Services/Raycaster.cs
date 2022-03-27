// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 25/03/2022 @ 19:11
// ==========================================================================

using System;
using System.Collections.Generic;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Services
{
    internal sealed class Raycaster : IRaycaster
    {
        //  Private fields.
        private readonly Collisions _collisions;
        private readonly List<string> _debugRaycasts;
        private readonly float? _fixedRayLength = null;
        private const float SkinWidth = 0.015f;
        private int _horizontalRayCount;
        private int _verticalRayCount;
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;
        private const int DefaultRayCount = 2;
        private BoxCollider2D _boxCollider2D;
        private RaycastOrigins _raycastOrigins;
        
        public ICollisions Collisions => _collisions;

        public Raycaster(float? fixedRayLength = null)
        {
            _collisions = new Collisions();
            _raycastOrigins = new RaycastOrigins();
            _debugRaycasts = new List<string>();
            _fixedRayLength = fixedRayLength;
        }

        public void Initialize(BoxCollider2D boxCollider2D, int rayCounts = DefaultRayCount)
        {
            _horizontalRayCount = _verticalRayCount = rayCounts;
            
            _boxCollider2D = boxCollider2D;
            
            CalculateRaySpacing();
        }
        
        public IEnumerable<IRaycastCollision> FindVerticalHitsOnly(Vector2 objectVelocity, LayerMask collisionMask)
        {
            UpdateRaycastOrigins();

            var raycastHits = new HashSet<IRaycastCollision>();
            
            float verticalDirection = Mathf.Sign(objectVelocity.y);
            float rayLength = _fixedRayLength ?? Mathf.Abs(objectVelocity.y) + SkinWidth;

            for (var verticalRayId = 0; verticalRayId < _verticalRayCount; verticalRayId++)
            {
                //  Based on the direction of the object, computes the point once the velocity is applied.
                Vector2 rayOrigin = verticalDirection == Directions.Downward ? _raycastOrigins.BottomLeft : _raycastOrigins.TopLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * verticalRayId);

                //  Draw a ray to see if the player hits something along the way.
                RaycastHit2D isCollision = Physics2D.Raycast(rayOrigin, Vector2.up * verticalDirection, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * verticalDirection * (_fixedRayLength ?? rayLength), isCollision ? Color.red : Color.green);

                if (!isCollision)
                    continue;
                
                var raycastHit = new RaycastCollision(isCollision.distance - SkinWidth, isCollision.transform);

                if (raycastHits.Contains(raycastHit))
                    continue;

                raycastHits.Add(raycastHit);
            }

            return raycastHits;
        }

        public void ApplyCollisions(ref Vector3 objectVelocity, LayerMask collisionMask)
        {
            UpdateRaycastOrigins();

            _collisions.ResetAll();
            
            ProcessHorizontalCollisions(ref objectVelocity, collisionMask);
            
            ProcessVerticalCollisions(ref objectVelocity, collisionMask);
        }
        
        private void CalculateRaySpacing()
        {
            Bounds bounds = GetExpandedBounds();

            _horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
            _verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);

            _horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
            _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
        }
        
        private void UpdateRaycastOrigins()
        {
            Bounds bounds = GetExpandedBounds();

            _raycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            _raycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
            _raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
            _raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
        }
        
        private Bounds GetExpandedBounds()
        {
            Bounds bounds = _boxCollider2D.bounds;
            bounds.Expand(SkinWidth * -2f);
            
            return bounds;
        }

        private void ProcessVerticalCollisions(ref Vector3 objectVelocity, LayerMask collisionMask)
        {
            if (objectVelocity.y == Movements.None)
                return;
            
            float verticalDirection = Mathf.Sign(objectVelocity.y);
            float rayLength = Mathf.Abs(objectVelocity.y) + SkinWidth;

            _debugRaycasts.Clear();
            _debugRaycasts.Add($"Velocity direction (Y) = {verticalDirection}");

            for (var verticalRayId = 0; verticalRayId < _verticalRayCount; verticalRayId++)
            {
                //  Based on the direction of the player, computes the origin position of the ray after the velocity is applied.
                Vector2 rayOrigin = verticalDirection == Directions.Downward ? _raycastOrigins.BottomLeft : _raycastOrigins.TopLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * verticalRayId + objectVelocity.x);
                
                //  Draw a ray from this origin below or up to see if the object hits something along the way.
                RaycastHit2D hasCollided = Physics2D.Raycast(rayOrigin, Vector2.up * verticalDirection, rayLength, collisionMask);
                
                Debug.DrawRay(rayOrigin, Vector2.up * verticalDirection * rayLength, hasCollided ? Color.red : Color.green);

                string collisionInfo = hasCollided ? hasCollided.rigidbody?.name : "N/A";
                _debugRaycasts.Add($"[{rayOrigin.x}, {rayOrigin.y}]: {collisionInfo}");
                
                if (!hasCollided)
                    continue;

                //  There is something along the way. Let's readjust the velocity to
                //  not go too far and stop at the obstacle.
                float verticalVelocity = (hasCollided.distance - SkinWidth) * verticalDirection;

                objectVelocity.y = verticalVelocity;
                   
                //  Readjust the rayLength for the other remaining rays. If there is an
                //  obstacle detected by the other ray but further down, we still want 
                //  to keep the velocity for the closest obstacle.
                rayLength = hasCollided.distance;
                
                _collisions.SetVerticalCollisions(verticalDirection);
            }

            AppLogger.Write(LogsLevels.PlayerRaycasting, $"{string.Join(" - ", _debugRaycasts)}");
        }
        
        private void ProcessHorizontalCollisions(ref Vector3 objectVelocity, LayerMask collisionMask)
        {
            if (objectVelocity.x == Movements.None)
                return;
            
            float horizontalDirection = Mathf.Sign(objectVelocity.x);
            float rayLength = Mathf.Abs(objectVelocity.x) + SkinWidth;
            
            _debugRaycasts.Clear();
            _debugRaycasts.Add($"Velocity (X) = {objectVelocity.x} ({horizontalDirection})");

            for (var horizontalRayId = 0; horizontalRayId < _horizontalRayCount; horizontalRayId++)
            {
                //  Based on the direction of the player, computes the point once the velocity is applied.
                Vector2 rayOrigin = horizontalDirection == Directions.Left ? _raycastOrigins.BottomLeft : _raycastOrigins.BottomRight;
                rayOrigin += Vector2.up * (_horizontalRaySpacing * horizontalRayId);
                
                //  Draw a ray to see if the player hits something along the way.
                RaycastHit2D hasCollided = Physics2D.Raycast(rayOrigin, Vector2.right * horizontalDirection, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.right * horizontalDirection * 1f, hasCollided ? Color.red : Color.green);

                if (!hasCollided)
                    continue;
                    
                _debugRaycasts.Add($"[{rayOrigin.x}, {rayOrigin.y}]: {hasCollided}");
                
                float horizontalVelocity = (hasCollided.distance - SkinWidth) * horizontalDirection;

                //  There is something along the way. Let's readjust the velocity to
                //  not go too far and stop at the obstacle.
                objectVelocity.x = horizontalVelocity;
                    
                //  Readjust the rayLength for the other remaining rays. If there is an
                //  obstacle detected by the other ray but further down, we still want 
                //  to keep the velocity for the closest obstacle.
                rayLength = hasCollided.distance;
                
                _collisions.SetHorizontalCollisions(horizontalDirection);
            }
            
            AppLogger.Write(LogsLevels.PlayerRaycasting, $"{string.Join(" - ", _debugRaycasts)}");
        }
        
    }
}