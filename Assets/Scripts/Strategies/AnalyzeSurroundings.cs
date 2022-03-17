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

        public bool MustAttack { get; private set; }
        
        public AnalyzeSurroundings(GoombasBehavior goombasBehavior)
        {
            _goombasBehavior = goombasBehavior;
            _playerBehavior = MonoBehaviour.FindObjectOfType<PlayerBehavior>();

            _currentStrategy = new NoStrategy(_goombasBehavior);
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
            MustAttack = _enemySurroundings.MustAttack();
            
            //  The method analyzes the current surroundings and decide a strategy.
            
            if (!MustAttack)
            {
                Debug.Log($"[{_goombasBehavior._name}] Goomba is staying idle.");
                _currentStrategy = new NoStrategy(_goombasBehavior);
            }
            else
            {
                if (!_enemySurroundings.IsAtSameLevel())
                {
                    Debug.Log($"[{_goombasBehavior._name}] Goomba is trying to reach the player's level.");
                    _currentStrategy = new ReachGroundStrategy(_goombasBehavior, _playerBehavior);
                }
                else
                {
                    Debug.Log($"[{_goombasBehavior._name}] Goomba is pursuing the player!");
                    _currentStrategy = new PursuePlayerStrategy(_goombasBehavior, _playerBehavior);
                }
            }

            _currentStrategy = _currentStrategy.Apply();
        }
    }
}