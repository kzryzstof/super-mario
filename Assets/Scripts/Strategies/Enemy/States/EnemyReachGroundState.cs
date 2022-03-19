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
                AppLogger.Write(LogsLevels.EnemyState, $"{Enemy.Position.y} {Enemy.Position.y} Enemy is blocked: jumping over a potential obstacle.");
                Enemy.Jump();
            }

            Enemy.Move(horizontalMovement);
        }
    }
}