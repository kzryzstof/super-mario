// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 12/03/2022 @ 12:50
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using System.Collections.Generic;

namespace NoSuchCompany.Games.SuperMario.Helpers
{
    internal sealed class HorizontalMovementHistory
    {
        private const int MaxHistory = 120;

        //  Keeps track of how much progress has been made in direction.
        private int _movementCounts = 0;
        private float _accumulatedMovement = 0f;
        private float _lastMovement = 0f;
        private readonly List<float> _lastMovements;

        public HorizontalMovementHistory()
        {
            _lastMovements = new List<float>();
        }
        
        public void Reset()
        {
            _accumulatedMovement = 0f;
            _movementCounts = 0;
        }

        public void Add(float newMovement)
        {
            if (Math.Sign(_lastMovement) != Math.Sign(newMovement))
            {
                Reset();
            }

            _movementCounts++;
            _accumulatedMovement += newMovement;
            _lastMovement = newMovement;
        }
    }
}