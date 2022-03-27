// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 20/03/2022 @ 20:11
// ==========================================================================

using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace NoSuchCompany.Games.SuperMario.Services.Impl
{
    //  I need to find a way to inject this. Does unity has a DI?
    internal sealed class LevelManager : ILevelManager
    {
        private static ILevelManager s_instance;

        private static readonly List<string> Levels = new List<string>
        {
            "Level 1-1",
            "Level 1-2"
        };
        
        public static ILevelManager Instance
        {
            get
            {
                if (s_instance is null)
                    s_instance = new LevelManager();

                return s_instance;
            }
        }

        private int _currentLevel;

        private LevelManager()
        {
            _currentLevel = 0;
        }

        public void LoadNextLevel()
        {
            _currentLevel++;

            LoadCurrentLevel();
        }
        
        public void ReloadCurrentLevel()
        {
            LoadCurrentLevel();
        }

        private void LoadCurrentLevel()
        {
            SceneManager.LoadScene(Levels[_currentLevel]);
        }
    }
}