// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 20:27
// ==========================================================================

using System.Collections.Generic;

namespace NoSuchCompany.Games.SuperMario.Entities
{
    public interface ICollisions
    {
        public bool Above { get; }
        
        public bool Below { get; }
        
        public bool Left { get; }
        
        public bool Right { get; }
        
        public IEnumerable<string> AboveCollisions { get; }

        public IEnumerable<string> BelowCollisions { get; }

        public IEnumerable<string> LeftCollisions { get; }

        public IEnumerable<string> RightCollisions { get; }
    }
}