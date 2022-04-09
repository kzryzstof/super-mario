// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:05
// ==========================================================================

using System;

namespace NoSuchCompany.Games.SuperMario.Diagnostics
{
    #region Enum

    [Flags]
    public enum LogsLevels
    {
        None = 0,

        EnemyState = 1,
        
        PlayerControls = 2,
        
        MovingPlatforms = 4,
        
        PlayerRaycasting = 8,
        
        EnemyAi = 16,
        
        RaycastingCollisions = 32,
        
        JumpController = 64,
    }

    #endregion
}