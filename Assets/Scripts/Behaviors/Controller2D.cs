// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 19:34
// ==========================================================================

using System;
using System.Collections.Generic;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class Controller2D : MonoBehaviour
    {
        //  Private fields.
        private const float SkinWidth = 0.01f;
        private const int DefaultRayCount = 2;
        private BoxCollider2D _boxCollider2D;
        private RaycastOrigins _raycastOrigins;
        private int _horizontalRayCount = DefaultRayCount;
        private int _verticalRayCount = DefaultRayCount;
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;
        private readonly Collisions _collisions;
        private readonly List<string> _debugRaycasts;
        
        //  Unity injected properties.
        public LayerMask collisionMask;
        
        //  Public properties. 
        public ICollisions Collisions => _collisions;

        public Controller2D()
        {
            _collisions = new Collisions();
            _raycastOrigins = new RaycastOrigins();
            _debugRaycasts = new List<string>();
        }
        
        public void Start()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        public void Move(Vector3 velocity)
        {
            CalculateRaySpacing();

            _collisions.ResetAll();
            
            UpdateRaycastOrigins();

            ProcessHorizontalCollisions(ref velocity);
            ProcessVerticalCollisions(ref velocity);
            
            transform.Translate(velocity);
        }
        
        private void ProcessVerticalCollisions(ref Vector3 velocity)
        {
            if (velocity.y == Movements.None)
                return;
            
            float verticalDirection = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

            _debugRaycasts.Clear();
            _debugRaycasts.Add($"Velocity (Y) = {velocity.y} ({verticalDirection})");

            for (var verticalRayId = 0; verticalRayId < _verticalRayCount; verticalRayId++)
            {
                //  Based on the direction of the player, computes the point once the velocity is applied.
                Vector2 rayOrigin = verticalDirection == Directions.Downward ? _raycastOrigins.BottomLeft : _raycastOrigins.TopLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * verticalRayId + velocity.x);
                
                //  Draw a ray to see if the player hits something along the way.
                RaycastHit2D isCollision = Physics2D.Raycast(rayOrigin, Vector2.up * verticalDirection, rayLength, collisionMask);
                
                Debug.DrawRay(rayOrigin, Vector2.up * verticalDirection * 2f, isCollision ? Color.red : Color.green);

                string collisionInfo = isCollision ? isCollision.rigidbody?.name : "N/A";
                _debugRaycasts.Add($"[{rayOrigin.x}, {rayOrigin.y}]: {collisionInfo}");
                
                if (!isCollision)
                    continue;

                //  There is something along the way. Let's readjust the velocity to
                //  not go too far and stop at the obstacle.
                
                //  TODO What happens if we hit our head?
                velocity.y = (isCollision.distance - SkinWidth) * verticalDirection;
                    
                //  Readjust the rayLength for the other remaining rays. If there is an
                //  obstacle detected by the other ray but further down, we still want 
                //  to keep the velocity for the closest obstacle.
                rayLength = isCollision.distance;
                
                _collisions.SetVerticalCollisions(verticalDirection);
            }
            
            AppLogger.Write(LogsLevels.PlayerRaycasting, $"{string.Join(" - ", _debugRaycasts)}");
        }
        
        private void ProcessHorizontalCollisions(ref Vector3 velocity)
        {
            if (velocity.x == Movements.None)
                return;
            
            float horizontalDirection = Mathf.Sign(velocity.x);
            float rayLength = Mathf.Abs(velocity.x) + SkinWidth;
            
            _debugRaycasts.Clear();
            _debugRaycasts.Add($"Velocity (X) = {velocity.x} ({horizontalDirection})");

            for (var horizontalRayId = 0; horizontalRayId < _horizontalRayCount; horizontalRayId++)
            {
                //  Based on the direction of the player, computes the point once the velocity is applied.
                Vector2 rayOrigin = horizontalDirection == Directions.Left ? _raycastOrigins.BottomLeft : _raycastOrigins.BottomRight;
                rayOrigin += Vector2.up * (_horizontalRaySpacing * horizontalRayId);
                
                //  Draw a ray to see if the player hits something along the way.
                RaycastHit2D isCollision = Physics2D.Raycast(rayOrigin, Vector2.right * horizontalDirection, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.right * horizontalDirection * 2f, isCollision ? Color.red : Color.green);

                string collisionInfo = isCollision ? isCollision.collider?.name : "N/A";
                _debugRaycasts.Add($"[{rayOrigin.x}, {rayOrigin.y}]: {collisionInfo}");
                
                if (!isCollision)
                    continue;

                //  There is something along the way. Let's readjust the velocity to
                //  not go too far and stop at the obstacle.
                velocity.x = (isCollision.distance - SkinWidth) * horizontalDirection;
                    
                //  Readjust the rayLength for the other remaining rays. If there is an
                //  obstacle detected by the other ray but further down, we still want 
                //  to keep the velocity for the closest obstacle.
                rayLength = isCollision.distance;
                
                _collisions.SetHorizontalCollisions(horizontalDirection);
            }
            
            AppLogger.Write(LogsLevels.PlayerRaycasting, $"{string.Join(" - ", _debugRaycasts)}");
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
    }
}