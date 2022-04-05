// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 20/03/2022 @ 19:44
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Services.Impl;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{
    public class LevelBehavior : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D otherCollider2D)
        {
            AppLogger.Write(LogsLevels.None, $"Touched! {otherCollider2D.tag}");
            
            if (otherCollider2D.CompareTag(Tags.Player))
            {
                LevelManager.Instance.LoadNextLevel();
                return;
            }
        }
    }
}