// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 19/03/2022 @ 12:53
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Strategies.Enemy.States;

namespace NoSuchCompany.Games.SuperMario.Strategies.Enemy
{
    internal abstract class EnemyState
    {
        protected IEnemy Enemy { get; }

        protected IPlayer Player { get; }
        
        protected EnemyState(IEnemy enemy, IPlayer player)
        {
            Enemy = enemy;
            Player = player;
        }
        
        public virtual void Do(EnemyContext enemyContext)
        {   
            
        }

        protected void GoToIdle(EnemyContext enemyContext)
        {
            enemyContext.CurrentState = new EnemyIdleState(Enemy, Player);
            enemyContext.CurrentState.Do(enemyContext);
        }

        protected void GoToPurse(EnemyContext enemyContext)
        {
            enemyContext.CurrentState = new EnemyPursueState(Enemy, Player);
            enemyContext.CurrentState.Do(enemyContext);
        }
        
        protected void GoToReach(EnemyContext enemyContext)
        {
            enemyContext.CurrentState = new EnemyReachGroundState(Enemy, Player);
            enemyContext.CurrentState.Do(enemyContext);
        }
    }
}