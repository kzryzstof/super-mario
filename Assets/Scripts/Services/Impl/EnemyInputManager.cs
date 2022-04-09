// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 26/03/2022 @ 19:12
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Services.Impl
{
    internal sealed class EnemyInputManager : IInputManager, IEnemyController
    {
        private bool isLeftPressed;
        private bool isRightPressed;
        private bool isJumpPressed;

        private bool IsLeftPressed
        {
            get
            {
                if (!isLeftPressed)
                    return false;

                isLeftPressed = false;
                return true;
            }
        }

        private bool IsRightPressed
        {
            get
            {
                if (!isRightPressed)
                    return false;

                isRightPressed = false;
                return true;
            }
        }

        public bool IsJumpPressed
        {
            get
            {
                if (!isJumpPressed)
                    return false;

                isJumpPressed = false;
                return true;
            }
        }

        public Vector2 Direction => new Vector2(IsLeftPressed ? -1f : IsRightPressed ? 1f : 0f, 0f);

        public bool IsRunPressed { get; }

        public void StandStill()
        {
            isLeftPressed = isRightPressed = false;
            isJumpPressed = false;
        }
        
        public void Jump()
        {
            isJumpPressed = true;
        }

        public void Move(float direction)
        {
            float actualDirection = Mathf.Sign(direction);
            
            isLeftPressed = actualDirection == Directions.Left;
            isRightPressed = actualDirection == Directions.Right;
            
            AppLogger.Write(LogsLevels.EnemyAi, $"Requested direction: Left? {isLeftPressed} Right? {isRightPressed} ({direction:F2})");
        }
    }
}