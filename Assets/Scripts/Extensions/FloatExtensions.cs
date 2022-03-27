// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:05
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Constants;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Extensions
{
    #region Class

    internal static class FloatExtensions
    {
        #region Constants

        private const float NoMovement = 0f;

        #endregion

        #region Public Methods

        public static bool IsMoving(this float value)
        {
            return value > NoMovement;
        }

        #endregion
    }

    #endregion
}