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
        /// Defines the minimum position of both the camera and the player.
        /// </summary>
        public const float MinimumLeftPosition = 4.5f;

        public static readonly LogsLevels LogsLevels =
                LogsLevels.EnemyStrategy
            ;

        #endregion
    }

    #endregion
}