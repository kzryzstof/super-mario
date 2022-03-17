// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:03
// ==========================================================================

using NoSuchCompany.Games.SuperMario.Entities;

namespace NoSuchCompany.Games.SuperMario.Strategies.Impl
{
    #region Class

    internal sealed class SurroundObstacleStrategy : IEnemyStrategy
    {
        #region Constants

        private const float NoMovement = 0f;

        private readonly EnemySurroundings _enemySurroundings;

        private readonly GoombasBehavior _goombasBehavior;

        private readonly PlayerBehavior _playerBehavior;

        #endregion

        #region Fields

        private float _horizontalMovement;

        private bool _isBlocked;

        private bool _mustAttack;

        #endregion

        #region Constructors

        public SurroundObstacleStrategy(GoombasBehavior goombasBehavior, PlayerBehavior playerBehavior)
        {
            _goombasBehavior = goombasBehavior;
            _playerBehavior = playerBehavior;

            _enemySurroundings = EnemySurroundings.Get(_goombasBehavior);
        }

        #endregion

        #region Public Methods

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

        public bool IsDone()
        {
            return _playerBehavior._isDead || !_mustAttack || _isBlocked;
        }

        public void Prepare()
        {
            _mustAttack = _enemySurroundings.MustAttack();
            _horizontalMovement = _mustAttack ? _enemySurroundings.MoveTowardPlayer(_goombasBehavior.moveSpeed) : NoMovement;
            _isBlocked = _goombasBehavior.IsBlocked(_mustAttack, _horizontalMovement);
        }

        #endregion
    }

    #endregion
}