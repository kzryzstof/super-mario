// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 19/03/2022 @ 12:56
// ==========================================================================

using System;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;

namespace NoSuchCompany.Games.SuperMario.Strategies.Enemy.States
{
    internal sealed class EnemyReachGroundState : EnemyState
    {
        public EnemyReachGroundState(IEnemy enemy, IPlayer player) : base(enemy, player)
        {
            AppLogger.Write(LogsLevels.EnemyAi, $"*** Enemy is on a different level than the player's: trying to reach ground.");
        }

        public override void Do(EnemyContext enemyContext)
        {
            bool isAtSameLevel = Math.Abs(Enemy.Position.y - Player.Position.y) < 1f;

            if (isAtSameLevel)
            {
                bool mustAttack = enemyContext.Surroundings.MustAttack();

                if (!mustAttack)
                {
                    GoToIdle(enemyContext);
                    return;
                }

                GoToPurse(enemyContext);
                return;
            }
            
            ReachGround(enemyContext);
        }

        private void ReachGround(EnemyContext enemyContext)
        {
            float horizontalMovement = enemyContext.Surroundings.MoveTowardPlayer(Enemy.MoveSpeed);

            bool isBlocked = Enemy.IsBlocked(horizontalMovement);

            if (isBlocked)
            {
                AppLogger.Write(LogsLevels.EnemyAi, $"{Enemy.Position.y} {Enemy.Position.y} Enemy is blocked: jumping over a potential obstacle.");
                
                enemyContext.Controller.Jump();
            }

            enemyContext.Controller.Move(horizontalMovement);
        }
    }
}