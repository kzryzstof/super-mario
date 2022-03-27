// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 19/03/2022 @ 12:55
// ==========================================================================

using System;
using System.Collections.Generic;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;

namespace NoSuchCompany.Games.SuperMario.Strategies.Enemy.States
{
    internal sealed class EnemyPursueState : EnemyState
    {
        private readonly Random _randomGenerator;
        private const int MaxPositions = 30;
        private readonly List<float> _previousPositions;
        private bool _isStagnating;
        
        public EnemyPursueState(IEnemy enemy, IPlayer player) : base(enemy, player)
        {
            _previousPositions = new List<float>();
            _randomGenerator = new Random();
        }

        public override void Do(EnemyContext enemyContext)
        {
            bool mustAttack = enemyContext.Surroundings.MustAttack();

            if (!mustAttack)
            {
                bool isAtSameLevel = Math.Abs(Enemy.Position.y - Player.Position.y) < 1f;

                if (isAtSameLevel)
                {
                    GoToIdle(enemyContext);
                    return;
                }

                GoToReach(enemyContext);
                return;
            }

            float horizontalMovement = enemyContext.Surroundings.MoveTowardPlayer(Enemy.MoveSpeed);

            JumpOverObstacleIfNeeded(enemyContext, horizontalMovement);

            ScarePlayerIfNeeded(enemyContext);

            if (_isStagnating)
            {
                GoToIdle(enemyContext);
                return;
            }
            
            AttackPlayer(enemyContext, horizontalMovement);
        }

        private void AttackPlayer(EnemyContext enemyContext, float horizontalMovement)
        {
            enemyContext.Controller.Move(horizontalMovement);

            TrackNewPosition(Enemy.Position.x, horizontalMovement);
        }

        private void JumpOverObstacleIfNeeded(EnemyContext enemyContext, float horizontalMovement)
        {
            bool isEnemyBlocked = Enemy.IsBlocked(horizontalMovement);
            
            if (isEnemyBlocked)
            {
                AppLogger.Write(LogsLevels.EnemyAi, $"{Enemy.Position.y} {Enemy.Position.y} Enemy is blocked: jumping over a potential obstacle.");
                enemyContext.Controller.Jump();
            }
        }

        private void ScarePlayerIfNeeded(EnemyContext enemyContext)
        {
            if (!Enemy.SupportsJumpScare)
                return;
            
            bool isPlayerClose = Math.Abs(Enemy.Position.x - Player.Position.x) < 2f;

            if (isPlayerClose && ShouldJump())
                enemyContext.Controller.Jump();
        }

        private void TrackNewPosition(float latestPosition, float latestMovement)
        {
            if (_previousPositions.Count == MaxPositions)
                _previousPositions.RemoveAt(_previousPositions.Count - 1);

            _previousPositions.Insert(0, latestPosition);

            UpdateIsStagnating(latestPosition);
        }

        private void UpdateIsStagnating(float latestPosition)
        {
            //if (_previousPositions.Count < 2)
            //{
                _isStagnating = false;
            //     return;
            // }
            //
            // float maxValue = _previousPositions
            //     .Select(position => latestPosition - position)
            //     .Max();
            //
            // _isStagnating = maxValue < 0.5f;
            //
            // AppLogger.Write(LogsLevels.EnemyAi, $"Is stagnating? {_isStagnating} ({maxValue})");
        }

        private bool ShouldJump()
        {
            const double Threshold = 0.42d;
            return _randomGenerator.NextDouble() < Threshold;
        }
    }
}