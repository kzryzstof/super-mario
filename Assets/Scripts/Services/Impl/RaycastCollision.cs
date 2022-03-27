// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 25/03/2022 @ 19:38
// ==========================================================================

using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Services.Impl
{
    internal sealed class RaycastCollision : IRaycastCollision
    {
        public float Distance { get; }
        
        public Transform Transform { get; }

        public RaycastCollision(float distance, Transform transform)
        {
            Distance = distance;
            Transform = transform;
        }

        public override bool Equals(object otherObject)
        {
            var otherRaycastHit = otherObject as IRaycastCollision;

            if (otherRaycastHit is null)
                return false;
            
            return Transform.Equals(otherRaycastHit.Transform);
        }

        public override int GetHashCode()
        {
            return Transform.GetHashCode();
        }
    }
}