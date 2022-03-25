// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 23/03/2022 @ 20:04
// ==========================================================================

using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Entities
{
    public sealed class CollisionStatus
    {
        //  Private fields.
        private const float RayDistance = 0.60f;
        private const int LayerMask = 0x11111111;
        private readonly IPlayer _player;
        
        //  Public properties.
        public bool IsGrounded => Below;
        public RaycastHit2D Forward { get; private set; }
        public RaycastHit2D Below { get; private set; }
        public RaycastHit2D Above { get; private set; }
        public RaycastHit2D Behind { get; private set; }

        public CollisionStatus(IPlayer player)
        {
            _player = player;
        }

        public void Update()
        {
            Vector2 currentPosition = _player.Position;
            
            Forward = CheckForCollision(currentPosition, Vector2.right);
            Behind = CheckForCollision(currentPosition, Vector2.left);
            Above = CheckForCollision(currentPosition, Vector2.up);
            Below = CheckForCollision(currentPosition, Vector2.down);
        }

        private static RaycastHit2D CheckForCollision(Vector2 currentPosition, Vector2 direction)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(currentPosition, direction, RayDistance, LayerMask);

            if (hitResult)
            {
                Debug.DrawLine(currentPosition, hitResult.point, Color.red);
            }
            else
            {
                Debug.DrawLine(currentPosition, currentPosition + direction * RayDistance, Color.green);
            }

            return hitResult;
        }
    }
}