// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 06/04/2022 @ 20:00
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Constants;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Extensions
{
    internal static class Vector3Extensions
    {
        public static bool IsGoingUp(this Vector3 velocity)
        {
            return Mathf.Sign(velocity.y) == Directions.Upward;
        }
        
        public static bool IsGoingDown(this Vector3 velocity)
        {
            return Mathf.Sign(velocity.y) == Directions.Downward;
        }
    }
}