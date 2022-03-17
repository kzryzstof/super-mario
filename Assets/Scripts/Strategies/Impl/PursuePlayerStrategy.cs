// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:04
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;

namespace NoSuchCompany.Games.SuperMario.Strategies.Impl
{
    #region Class

    internal sealed class PursuePlayerStrategy : IEnemyStrategy
    {
        #region Constants

        private const int MaxPositions = 30;

        private const float NoMovement = 0f;

        private readonly EnemySurroundings _enemySurroundings;

        private readonly GoombasBehavior _goombasBehavior;

        private readonly PlayerBehavior _playerBehavior;

        #endregion

        #region Fields

        private float _horizontalMovement;

        private bool _isAtSameLevel;

        private bool _isBlocked;

        private bool _isStagnating;

        private bool _mustAttack;

        private readonly List<float> _previousPositions;

        #endregion

        #region Constructors

        public PursuePlayerStrategy(GoombasBehavior goombasBehavior, PlayerBehavior playerBehavior)
        {
            _goombasBehavior = goombasBehavior;
            _playerBehavior = playerBehavior;

            _previousPositions = new List<float>();
            _enemySurroundings = EnemySurroundings.Get(_goombasBehavior);
        }

        #endregion

        #region Public Methods

        public IEnemyStrategy Apply()
        {
            if (_isBlocked)
            {
                AppLogger.Write(LogsLevels.EnemyStrategy, $"{_goombasBehavior.transform.position.y} {_playerBehavior.transform.position.y} Goomba is blocked: jumping over a potential obstacle.");
                _goombasBehavior.Jump();
            }

            if (!_isStagnating)
            {
                _goombasBehavior.Move(_horizontalMovement);

                AppLogger.Write(LogsLevels.EnemyStrategy, $"{_goombasBehavior.transform.position.y} {_playerBehavior.transform.position.y} Goomba is heading over to the Player. Movement: {_horizontalMovement}. Position ({_goombasBehavior.transform.position.x}, {_goombasBehavior.transform.position.y})");
                TrackNewPosition(_goombasBehavior.transform.position.x, _horizontalMovement);

                return this;
            }

            return new NoStrategy(_goombasBehavior);
        }

        public bool IsDone()
        {
            return _playerBehavior._isDead || !_mustAttack;
        }

        public void Prepare()
        {
            _isAtSameLevel = Math.Abs(_goombasBehavior.transform.position.y - _playerBehavior.transform.position.y) < 1f;
            _mustAttack = _enemySurroundings.MustAttack();
            _horizontalMovement = _mustAttack ? _enemySurroundings.MoveTowardPlayer(_goombasBehavior.moveSpeed) : NoMovement;
            _isBlocked = _goombasBehavior.IsBlocked(_mustAttack, _horizontalMovement);
        }

        #endregion

        #region Private Methods

        private void TrackNewPosition(float latestPosition, float latestMovement)
        {
            if (_previousPositions.Count == MaxPositions)
                _previousPositions.RemoveAt(_previousPositions.Count - 1);

            _previousPositions.Insert(0, latestPosition);

            UpdateIsStagnating(latestPosition, latestMovement);
        }

        private void UpdateIsStagnating(float latestPosition, float latestMovement)
        {
            if (_previousPositions.Count < 2)
            {
                _isStagnating = false;
                return;
            }

            float maxValue = _previousPositions
                .Select(position => latestPosition - position)
                .Max();

            _isStagnating = maxValue < 0.5f;

            AppLogger.Write(LogsLevels.EnemyStrategy, $"Is stagnating? {_isStagnating} ({maxValue})");
        }

        #endregion
    }

    #endregion
}