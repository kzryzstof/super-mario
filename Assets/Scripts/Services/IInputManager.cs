// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 20:13
// ==========================================================================

using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Services
{
    public interface IInputManager
    {
        bool IsJumpPressed { get; }
        
        Vector2 Direction { get; }
        
        bool IsRunPressed { get; }
    }
}