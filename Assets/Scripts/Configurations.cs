// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:05
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Diagnostics;

namespace NoSuchCompany.Games.SuperMario
{
    #region Class

    internal static class Configurations
    {
        #region Constants

        /// <summary>
        /// Defines the minimum position of both the camera.
        /// </summary>
        public const float MinimumLeftPositionForCamera = 4.5f;

        /// <summary>
        /// Defines the minimum position of both the player.
        /// </summary>
        public const float MinimumLeftPositionForPlayer = -6.0f;

        public static readonly LogsLevels LogsLevels =
                LogsLevels.None
                //LogsLevels.JumpController
                //LogsLevels.EnemyState
                //LogsLevels.PlayerControls
                //LogsLevels.MovingPlatforms
                //LogsLevels.PlayerRaycasting
                //| LogsLevels.RaycastingCollisions
                //LogsLevels.EnemyAi
            ;

        #endregion
    }

    #endregion
}