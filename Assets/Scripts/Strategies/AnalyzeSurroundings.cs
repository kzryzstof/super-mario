// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:04
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Strategies.Impl;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Strategies
{
    #region Class

    /// <summary>
    /// Decides what to do based on the enemy's surroundings.
    /// Potential outcomes:
    /// - Stay idle;
    /// - Pursuit mode try to reach the Player
    /// - Try to avoid the obstacle?
    /// </summary>
    internal sealed class AnalyzeSurroundings
    {
        #region Constants

        private readonly GoombasBehavior _goombasBehavior;

        private readonly PlayerBehavior _playerBehavior;

        #endregion

        #region Fields

        private IEnemyStrategy _currentStrategy;

        private EnemySurroundings _enemySurroundings;

        #endregion

        #region Properties

        public bool MustAttack { get; private set; }

        #endregion

        #region Constructors

        public AnalyzeSurroundings(GoombasBehavior goombasBehavior)
        {
            _goombasBehavior = goombasBehavior;
            _playerBehavior = Object.FindObjectOfType<PlayerBehavior>();

            _currentStrategy = new NoStrategy(_goombasBehavior);
        }

        #endregion

        #region Public Methods

        public void Process()
        {
            _currentStrategy.Prepare();

            if (!_currentStrategy.IsDone())
            {
                _currentStrategy = _currentStrategy.Apply();
                return;
            }

            _enemySurroundings ??= EnemySurroundings.Get(_goombasBehavior);
            MustAttack = _enemySurroundings.MustAttack();

            //  The method analyzes the current surroundings and decide a strategy.

            if (!MustAttack)
            {
                //AppLogger.Write(LogsLevels.EnemyStrategy, $"[{_goombasBehavior._name}] Goomba is staying idle.");
                _currentStrategy = new NoStrategy(_goombasBehavior);
            }
            else
            {
                if (!_enemySurroundings.IsAtSameLevel())
                {
                    AppLogger.Write(LogsLevels.EnemyStrategy, $"[{_goombasBehavior._name}] Goomba is trying to reach the player's level.");
                    _currentStrategy = new ReachGroundStrategy(_goombasBehavior, _playerBehavior);
                }
                else
                {
                    AppLogger.Write(LogsLevels.EnemyStrategy, $"[{_goombasBehavior._name}] Goomba is pursuing the player!");
                    _currentStrategy = new PursuePlayerStrategy(_goombasBehavior, _playerBehavior);
                }
            }

            _currentStrategy = _currentStrategy.Apply();
        }

        #endregion
    }

    #endregion
}