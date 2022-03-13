// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 12/03/2022 @ 13:47
// Last author: Christophe Commeyne
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Entities;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Strategies
{
    /// <summary>
    /// Decides what to do based on the enemy's surroundings.
    /// Potential outcomes:
    ///     - Stay idle;
    ///     - Pursuit mode try to reach the Player
    ///     - Try to avoid the obstacle?
    /// </summary>
    internal sealed class AnalyzeSurroundings
    {
        private readonly GoombasBehavior _goombasBehavior;
        private readonly PlayerBehavior _playerBehavior;
        private IEnemyStrategy _currentStrategy;
        private EnemySurroundings _enemySurroundings;

        public AnalyzeSurroundings(GoombasBehavior goombasBehavior)
        {
            _goombasBehavior = goombasBehavior;
            _playerBehavior = MonoBehaviour.FindObjectOfType<PlayerBehavior>();

            _currentStrategy = new NoStrategy();
        }

        public void Process()
        {
            _currentStrategy.Prepare();

            if (!_currentStrategy.IsDone())
            {
                _currentStrategy = _currentStrategy.Apply();
                return;
            }

            _enemySurroundings ??= EnemySurroundings.Get(_goombasBehavior);

            //  The method analyzes the current surroundings and decide a strategy.

            if (!_enemySurroundings.IsAtSameLevel())
            {
                _currentStrategy = new ReachGroundStrategy(_goombasBehavior, _playerBehavior);    
            }
            else
            {
                bool mustAttack = _enemySurroundings.MustAttack(_goombasBehavior.minDistanceToAttack);

                if (mustAttack)
                    _currentStrategy = new PursuePlayerStrategy(_goombasBehavior, _playerBehavior);
                else
                    _currentStrategy = new NoStrategy();
            }

            _currentStrategy = _currentStrategy.Apply();
            
        }
    }
}