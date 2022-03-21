// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 20/03/2022 @ 20:16
// ==========================================================================

namespace NoSuchCompany.Games.SuperMario.Services
{
    public interface ILevelManager
    {
        void LoadNextLevel();

        void ReloadCurrentLevel();
    }
}