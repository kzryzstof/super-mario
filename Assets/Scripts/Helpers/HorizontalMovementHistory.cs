// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:05
// ==========================================================================

using System;
using System.Collections.Generic;

namespace NoSuchCompany.Games.SuperMario.Helpers
{
    #region Class

    internal sealed class HorizontalMovementHistory
    {
        #region Constants

        private const int MaxHistory = 120;

        private readonly List<float> _lastMovements;

        #endregion

        #region Fields

        private float _accumulatedMovement;

        private float _lastMovement;

        //  Keeps track of how much progress has been made in direction.
        private int _movementCounts;

        #endregion

        #region Constructors

        public HorizontalMovementHistory()
        {
            _lastMovements = new List<float>();
        }

        #endregion

        #region Public Methods

        public void Add(float newMovement)
        {
            if (Math.Sign(_lastMovement) != Math.Sign(newMovement))
                Reset();

            _movementCounts++;
            _accumulatedMovement += newMovement;
            _lastMovement = newMovement;
        }

        public void Reset()
        {
            _accumulatedMovement = 0f;
            _movementCounts = 0;
        }

        #endregion
    }

    #endregion
}