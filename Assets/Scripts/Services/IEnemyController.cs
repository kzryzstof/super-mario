// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 26/03/2022 @ 19:16
// ==========================================================================

namespace NoSuchCompany.Games.SuperMario.Services
{
    public interface IEnemyController
    {
        void StandStill();
        
        void Jump();

        void Move(float direction);
    }
}