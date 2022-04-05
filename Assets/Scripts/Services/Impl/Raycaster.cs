// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 25/03/2022 @ 19:11
// ==========================================================================

using System.Collections.Generic;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Services.Impl
{
    internal sealed class Raycaster : IRaycaster
    {
        //  Private fields.
        private readonly Collisions _collisions;
        private readonly List<string> _debugRaycasts;
        private const float SkinWidth = 0.015f;
        private int _horizontalRayCount;
        private int _verticalRayCount;
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;
        private const int DefaultRayCount = 2;
        private BoxCollider2D _boxCollider2D;
        private RaycastOrigins _raycastOrigins;
        
        public ICollisions Collisions => _collisions;

        public Raycaster()
        {
            _collisions = new Collisions();
            _raycastOrigins = new RaycastOrigins();
            _debugRaycasts = new List<string>();
        }

        public void Initialize(BoxCollider2D boxCollider2D, int rayCounts = DefaultRayCount)
        {
            _horizontalRayCount = _verticalRayCount = rayCounts;
            
            _boxCollider2D = boxCollider2D;
            
            CalculateRaySpacing();
        }
        
        public IEnumerable<IRaycastCollision> FindVerticalHitsOnly(Vector2 objectVelocity, LayerMask collisionMask, float? fixedRayLength = null)
        {
            UpdateRaycastOrigins();

            //  Avoid collision with itself during the raycasting phase.
            _boxCollider2D.enabled = false;
            
            var raycastHits = new HashSet<IRaycastCollision>();
            
            float verticalDirection = Mathf.Sign(objectVelocity.y);
            float rayLength = fixedRayLength ?? Mathf.Abs(objectVelocity.y) + SkinWidth;

            for (var verticalRayId = 0; verticalRayId < _verticalRayCount; verticalRayId++)
            {
                //  Based on the direction of the object, computes the point once the velocity is applied.
                Vector2 rayOrigin = verticalDirection == Directions.Downward ? _raycastOrigins.BottomLeft : _raycastOrigins.TopLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * verticalRayId);

                //  Draw a ray to see if the player hits something along the way.
                RaycastHit2D isCollision = Physics2D.Raycast(rayOrigin, Vector2.up * verticalDirection, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * verticalDirection * (fixedRayLength ?? rayLength), isCollision ? Color.red : Color.green);

                if (!isCollision)
                    continue;
                
                var raycastHit = new RaycastCollision(isCollision.distance - SkinWidth, isCollision.transform);

                if (raycastHits.Contains(raycastHit))
                    continue;

                raycastHits.Add(raycastHit);
            }

            _boxCollider2D.enabled = true;
            
            return raycastHits;
        }
        
        public void ApplyCollisions(ref Vector3 objectVelocity, LayerMask collisionMask)
        {
            UpdateRaycastOrigins();

            _collisions.ResetAll();
            
            //  Avoid collision with itself during the raycast-ing phase.
            _boxCollider2D.enabled = false;
            
            ProcessHorizontalCollisions(ref objectVelocity, collisionMask);
            
            ProcessVerticalCollisions(ref objectVelocity, collisionMask);
            
            _boxCollider2D.enabled = true;
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
            float rayLength = Mathf.Abs(objectVelocity.y) + SkinWidth;
            float bottomRayLength = rayLength;
            float topRayLength = rayLength;

            _debugRaycasts.Clear();

            for (var rayId = 0; rayId < _verticalRayCount; rayId++)
            {
                //  Based on the direction of the player, computes the origin position of the ray after the velocity is applied.
                Vector2 nextPosition = Vector2.right * (_verticalRaySpacing * rayId + objectVelocity.x);
                
                Vector2 bottomRay = _raycastOrigins.BottomLeft + nextPosition;
                Vector2 topRay = _raycastOrigins.TopLeft + nextPosition;
                
                //  Draw a ray from this origin below or up to see if the object hits something along the way.
                RaycastHit2D bottomCollision = Physics2D.Raycast(bottomRay, Vector2.up * Directions.Downward, bottomRayLength, collisionMask);
                RaycastHit2D topCollision = Physics2D.Raycast(topRay, Vector2.up * Directions.Upward, topRayLength, collisionMask);
                
                Debug.DrawRay(bottomRay, Vector2.up * Directions.Downward * 0.5f, bottomCollision ? Color.red : Color.green);
                Debug.DrawRay(topRay, Vector2.up * Directions.Upward * 0.5f, topCollision ? Color.red : Color.green);

                if (bottomCollision)
                {
                    if (Mathf.Sign(objectVelocity.y) == Directions.Downward)
                    {
                        //  There is something along the way. Let's readjust the velocity to
                        //  not go too far and stop at the obstacle.
                        float verticalVelocity = (bottomCollision.distance - SkinWidth) * Directions.Downward;
                        objectVelocity.y = verticalVelocity;
                    }
                    
                    //  Readjust the rayLength for the other remaining rays. If there is an
                    //  obstacle detected by the other ray but further down, we still want 
                    //  to keep the velocity for the closest obstacle.
                    bottomRayLength = bottomCollision.distance;
                    
                    _collisions.SetBottomCollisions(bottomCollision.transform.tag);
                }

                if (topCollision)
                {
                    if (Mathf.Sign(objectVelocity.y) == Directions.Upward)
                    {
                        //  There is something along the way. Let's readjust the velocity to
                        //  not go too far and stop at the obstacle.
                        float verticalVelocity = (topCollision.distance - SkinWidth) * Directions.Upward;
                        objectVelocity.y = verticalVelocity;
                    }
                    
                    //  Readjust the rayLength for the other remaining rays. If there is an
                    //  obstacle detected by the other ray but further down, we still want 
                    //  to keep the velocity for the closest obstacle.
                    topRayLength = topCollision.distance;
                    
                    _collisions.SetTopCollisions(topCollision.transform.tag);
                }
            }

            AppLogger.Write(LogsLevels.PlayerRaycasting, $"{string.Join(" - ", _debugRaycasts)}");
        }
        
        private void ProcessHorizontalCollisions(ref Vector3 objectVelocity, LayerMask collisionMask)
        {
            float rayLength = Mathf.Abs(objectVelocity.x) + SkinWidth;
            float leftRayLength = rayLength;
            float rightRayLength = rayLength;

            _debugRaycasts.Clear();

            for (var rayId = 0; rayId < _horizontalRayCount; rayId++)
            {
                //  Based on the direction of the player, computes the point once the velocity is applied.
                Vector2 nextPosition = Vector2.up * (_horizontalRaySpacing * rayId);
                Vector2 leftRay = _raycastOrigins.BottomLeft + nextPosition;
                Vector2 rightRay = _raycastOrigins.BottomRight + nextPosition;
                    
                //  Draw a ray to see if the player hits something along the way.
                RaycastHit2D leftCollision = Physics2D.Raycast(leftRay, Vector2.right * Directions.Left, leftRayLength, collisionMask);
                RaycastHit2D rightCollision = Physics2D.Raycast(rightRay, Vector2.right * Directions.Right, rightRayLength, collisionMask);

                Debug.DrawRay(leftRay, Vector2.right * Directions.Left * 1f, leftCollision ? Color.red : Color.green);
                Debug.DrawRay(rightRay, Vector2.right * Directions.Right * 1f, rightCollision ? Color.red : Color.green);

                if (leftCollision)
                {
                    if (Mathf.Sign(objectVelocity.x) == Directions.Left)
                    {
                        //  There is something along the way. Let's readjust the velocity to
                        //  not go too far and stop at the obstacle.
                        objectVelocity.x = (leftCollision.distance - SkinWidth) * Directions.Left;
                    }
                    
                    //  Readjust the rayLength for the other remaining rays. If there is an
                    //  obstacle detected by the other ray but further down, we still want 
                    //  to keep the velocity for the closest obstacle.
                    leftRayLength = leftCollision.distance;
                    
                    _collisions.SetLeftCollisions(leftCollision.transform.tag);
                }

                if (rightCollision)
                {
                    if (Mathf.Sign(objectVelocity.x) == Directions.Right)
                    {
                        //  There is something along the way. Let's readjust the velocity to
                        //  not go too far and stop at the obstacle.
                        objectVelocity.x = (rightCollision.distance - SkinWidth) * Directions.Right;
                    }
                    
                    //  Readjust the rayLength for the other remaining rays. If there is an
                    //  obstacle detected by the other ray but further down, we still want 
                    //  to keep the velocity for the closest obstacle.
                    rightRayLength = rightCollision.distance;
                    
                    _collisions.SetRightCollisions(rightCollision.transform.tag);
                }
            }
            
            AppLogger.Write(LogsLevels.PlayerRaycasting, $"{string.Join(" - ", _debugRaycasts)}");
        }
        
    }
}