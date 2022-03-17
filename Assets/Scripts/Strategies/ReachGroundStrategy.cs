// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 12/03/2022 @ 20:50
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using NoSuchCompany.Games.SuperMario.Entities;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Strategies
{
    internal sealed class ReachGroundStrategy : IEnemyStrategy
    {
        private readonly GoombasBehavior _goombasBehavior;
        private readonly PlayerBehavior _playerBehavior;
        private readonly EnemySurroundings _enemySurroundings;

        private readonly float _horizontalMovement;
        private bool _isBlocked;
        private bool _isAtSameLevel;
        private bool _mustAttack;
        
        public ReachGroundStrategy(GoombasBehavior goombasBehavior, PlayerBehavior playerBehavior)
        {
            _goombasBehavior = goombasBehavior;
            _playerBehavior = playerBehavior;

            _enemySurroundings = EnemySurroundings.Get(_goombasBehavior);
            _horizontalMovement = _enemySurroundings.MoveTowardPlayer(_goombasBehavior.moveSpeed);
        }
        
        public bool IsDone()
        {
            return _playerBehavior._isDead  || _isAtSameLevel || !_mustAttack;
        }

        public void Prepare()
        {
            _mustAttack = _enemySurroundings.MustAttack();
            _isAtSameLevel = Math.Abs(_goombasBehavior.transform.position.y - _playerBehavior.transform.position.y) < 1f;
            _isBlocked = _goombasBehavior.IsBlocked(true, _horizontalMovement);
        }

        public IEnemyStrategy Apply()
        {
            if (_isBlocked)
            {
                Debug.Log($"{_goombasBehavior.transform.position.y} {_playerBehavior.transform.position.y} Goomba is blocked: jumping over a potential obstacle.");
                _goombasBehavior.Jump();
            }

            _goombasBehavior.Move(_horizontalMovement);
                
            return this;
        }
    }
}