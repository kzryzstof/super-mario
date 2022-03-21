// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 21/03/2022 @ 19:46
// ==========================================================================

namespace NoSuchCompany.Games.SuperMario.Entities
{
    public enum ScrollingMode
    {
        /// <summary>
        /// Indicates that the platform can get offscreen and reappear at the other end.
        /// </summary>
        Continuous,
        
        /// <summary>
        /// Indicates that the platform movement is bound by a min & max offsets.
        /// </summary>
        BackAndForth
    }
}