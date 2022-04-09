// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 04/04/2022 @ 20:57
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;

namespace NoSuchCompany.Games.SuperMario.Services.Jumping
{
    /// <remarks>
    /// A cool down is useful to make sure we do not 
    /// allow the player to jump while being in the air.
    /// This situation happens when the player jumps while
    /// being close to a wall. And as soon as the player reaches
    /// the top, it gets an extra push upward.
    /// </remarks>
    internal sealed class JumpCooldown
    {
        //  Indicates how many frames to skip until cooldown is deactivated.
        private const uint FramesOnCooldown = 60;
        private uint _frameCounter;
        private bool _isJumpingInitiated;

        public bool IsActivated { get; private set; }

        public void PreUpdate()
        {
            if (!IsActivated)
            {
                //  No cooldown active. Nothing to do.
                return;
            }

            //  Counts the number of frames to determine
            //  when to stop the cooldown.
            _frameCounter++;

            if (IsStillActive())
                return;

            IsActivated = false;
            
            AppLogger.Write(LogsLevels.None, "Jump cooldown deactivated");
        }

        public void PostUpdate(ICollisions collisions)
        {
            if (GotOffGround(collisions))
            {
                //  The character is taking off the ground: let's activate the cooldown.
                _isJumpingInitiated = true;
                
                StartCooldown();
            }

            if (_isJumpingInitiated && collisions.Below)
                _isJumpingInitiated = false;
        }

        private bool GotOffGround(ICollisions collisions)
        {
            return !_isJumpingInitiated && !collisions.Below;
        }

        private bool IsStillActive()
        {
            return _frameCounter <= FramesOnCooldown;
        }

        private void StartCooldown()
        {
            if (IsActivated)
                return;
            
            IsActivated = true;
            _frameCounter = 0;
            
            AppLogger.Write(LogsLevels.None, "Jump cooldown activated");
        }
    }
}