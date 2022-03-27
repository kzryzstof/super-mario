// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 20:26
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Constants;

namespace NoSuchCompany.Games.SuperMario.Entities
{
    internal sealed class Collisions : ICollisions
    {
        public bool Above { get; set; }
        
        public bool Below { get; set; }
        
        public bool Left { get; set; }
        
        public bool Right { get; set; }

        public bool IsCollidingExceptBelow => Left || Above || Right;

        public void ResetAll()
        {
            Above = Below = false;
            Left = Right = false;
        }

        public void SetHorizontalCollisions(float direction)
        {
            Left = direction == Directions.Left;
            Right = direction == Directions.Right;
        }
        
        public void SetVerticalCollisions(float direction)
        {
            Above = direction == Directions.Upward;
            Below = direction == Directions.Downward;
        }
    }
}