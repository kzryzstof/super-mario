// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:04
// ==========================================================================

using System;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;

namespace NoSuchCompany.Games.SuperMario.Strategies.Impl
{
    #region Class

    internal sealed class ReachGroundStrategy : IEnemyStrategy
    {
        #region Constants

        private readonly EnemySurroundings _enemySurroundings;

        private readonly GoombasBehavior _goombasBehavior;

        private readonly float _horizontalMovement;

        private readonly PlayerBehavior _playerBehavior;

        #endregion

        #region Fields

        private bool _isAtSameLevel;

        private bool _isBlocked;

        private bool _mustAttack;

        #endregion

        #region Constructors

        public ReachGroundStrategy(GoombasBehavior goombasBehavior, PlayerBehavior playerBehavior)
        {
            _goombasBehavior = goombasBehavior;
            _playerBehavior = playerBehavior;

            _enemySurroundings = EnemySurroundings.Get(_goombasBehavior);
            _horizontalMovement = _enemySurroundings.MoveTowardPlayer(_goombasBehavior.moveSpeed);
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

            _goombasBehavior.Move(_horizontalMovement);

            return this;
        }

        public bool IsDone()
        {
            return _playerBehavior._isDead || _isAtSameLevel || !_mustAttack;
        }

        public void Prepare()
        {
            _mustAttack = _enemySurroundings.MustAttack();
            _isAtSameLevel = Math.Abs(_goombasBehavior.transform.position.y - _playerBehavior.transform.position.y) < 1f;
            _isBlocked = _goombasBehavior.IsBlocked(true, _horizontalMovement);
        }

        #endregion
    }

    #endregion
}