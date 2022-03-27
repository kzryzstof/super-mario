// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 19/03/2022 @ 12:53
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;

namespace NoSuchCompany.Games.SuperMario.Strategies.Enemy.States
{
    internal sealed class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(IEnemy enemy, IPlayer player) : base(enemy, player)
        {
            AppLogger.Write(LogsLevels.EnemyAi, $"*** Enemy is Idle");
        }

        public override void Do(EnemyContext enemyContext)
        {
            bool mustAttack = enemyContext.Surroundings.MustAttack();

            if (mustAttack == false)
            {
                enemyContext.Controller.StandStill();
                return;
            }

            bool isSameLevelAsPlayer = enemyContext.Surroundings.IsAtSameLevel();

            if (isSameLevelAsPlayer == false)
            {
                enemyContext.CurrentState = new EnemyReachGroundState(Enemy, Player);
                enemyContext.CurrentState.Do(enemyContext);
                return;
            }
            
            enemyContext.CurrentState = new EnemyPursueState(Enemy, Player);
            enemyContext.CurrentState.Do(enemyContext);
        }
    }
}