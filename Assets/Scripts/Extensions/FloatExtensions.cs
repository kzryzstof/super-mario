// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 12/03/2022 @ 13:20
// Last author: Christophe Commeyne
// ==========================================================================

namespace NoSuchCompany.Games.SuperMario.Extensions
{
    internal static class FloatExtensions
    {
        private const float NoMovement = 0f;

        public static bool IsMoving(this float value)
        {
            return value > NoMovement;
        }
    }
}