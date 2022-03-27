// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 25/03/2022 @ 19:11
// ==========================================================================

using System.Collections.Generic;
using NoSuchCompany.Games.SuperMario.Entities;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Services
{
    public interface IRaycaster
    {
        ICollisions Collisions { get; }

        void Initialize(BoxCollider2D boxCollider2D, int rayCounts);

        IEnumerable<IRaycastCollision> FindVerticalHitsOnly(Vector2 objectVelocity, LayerMask collisionMask, float? fixedRayLength = null);

        void ApplyCollisions(ref Vector3 objectVelocity, LayerMask collisionMask);
    }
}