// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 25/03/2022 @ 19:38
// ==========================================================================

using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Services
{
    public interface IRaycastCollision
    {
        /// <summary>
        /// Gets the distance between the object and the obstacle hit by the raycast.
        /// </summary>
        float Distance { get; }
        
        /// <summary>
        /// Gets the <see cref="Transform"/> of the obstacle hit by the raycast.
        /// </summary>
        Transform Transform { get; }
    }
}