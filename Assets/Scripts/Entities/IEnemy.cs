// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 19/03/2022 @ 13:02
// ==========================================================================

using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Entities
{
    public interface IEnemy
    {
        float MoveSpeed { get; }
        
        float MinimumDistanceToAttack { get; }

        Vector2 Position { get; }
        
        bool SupportsJumpScare { get; }

        EnemySurroundings GetSurroundings();
        
        bool IsBlocked(float horizontalMovement);
    }
}