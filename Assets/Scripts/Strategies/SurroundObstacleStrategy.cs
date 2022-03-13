// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 12/03/2022 @ 13:46
// Last author: Christophe Commeyne
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Entities;

namespace NoSuchCompany.Games.SuperMario.Strategies
{
    internal sealed class SurroundObstacleStrategy : IEnemyStrategy
    {
        private const float NoMovement = 0f;
        private readonly GoombasBehavior _goombasBehavior;
        private readonly PlayerBehavior _playerBehavior;
        private readonly EnemySurroundings _enemySurroundings;

        private float _horizontalMovement;
        private bool _isBlocked;
        private bool _mustAttack;

        public SurroundObstacleStrategy(GoombasBehavior goombasBehavior, PlayerBehavior playerBehavior)
        {
            _goombasBehavior = goombasBehavior;
            _playerBehavior = playerBehavior;
            
            _enemySurroundings = EnemySurroundings.Get(_goombasBehavior);
        }

        public void Prepare()
        {
            _mustAttack = _enemySurroundings.MustAttack(_goombasBehavior.minDistanceToAttack);
            _horizontalMovement = _mustAttack ? _enemySurroundings.MoveTowardPlayer(_goombasBehavior.moveSpeed) : NoMovement;
            _isBlocked = _goombasBehavior.IsBlocked(_mustAttack, _horizontalMovement);
        }

        public bool IsDone()
        {
            return _playerBehavior._isDead || !_mustAttack || _isBlocked;
        }

        public IEnemyStrategy Apply()
        {
            if (_isBlocked)
            {
                _goombasBehavior.Jump();
                _goombasBehavior.Move(_horizontalMovement);
            }
            else
            {
                _goombasBehavior.Move(_horizontalMovement);
            }
            
            return this;
        }
    }
}