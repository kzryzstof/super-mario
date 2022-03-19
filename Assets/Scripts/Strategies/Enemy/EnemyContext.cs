// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 19/03/2022 @ 12:51
// ==========================================================================

using System;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Strategies.Enemy.States;

namespace NoSuchCompany.Games.SuperMario.Strategies.Enemy
{
    internal sealed class EnemyContext
    {
        private EnemyState _currentState;

        public EnemyState CurrentState
        {
            get => _currentState;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(CurrentState), $"State can't be null");

                _currentState = value;
            }
        }
        
        public EnemySurroundings Surroundings { get; }

        public bool IsAttacking => CurrentState.GetType() != typeof(EnemyIdleState);
        
        public EnemyContext(IEnemy enemy, IPlayer player)
        {
            Surroundings = enemy.GetSurroundings();
            CurrentState = new EnemyIdleState(enemy, player);
        }

        public void Do()
        {
            CurrentState.Do(this);    
        }
    }
}