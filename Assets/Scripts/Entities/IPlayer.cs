// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 19/03/2022 @ 13:02
// ==========================================================================

using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Entities
{
    public interface IPlayer
    {
        Vector2 Position { get; }

        void OnEnemyAttacked();
    }
}