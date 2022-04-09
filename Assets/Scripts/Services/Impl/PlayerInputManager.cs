// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 20:13
// ==========================================================================

using UnityEngine;
using UnityEngine.InputSystem;

namespace NoSuchCompany.Games.SuperMario.Services.Impl
{
    internal sealed class PlayerInputManager : IInputManager
    {
        public bool IsLeftPressed => Gamepad.current.dpad.left.isPressed;
        
        public bool IsRightPressed => Gamepad.current.dpad.right.isPressed;

        public bool IsJumpPressed => Gamepad.current.buttonSouth.isPressed;

        public bool IsRunPressed => Gamepad.current.buttonWest.isPressed;
        
        public Vector2 Direction => new Vector2(IsLeftPressed ? -1f : IsRightPressed ? 1f : 0f, 0f);
    }
}