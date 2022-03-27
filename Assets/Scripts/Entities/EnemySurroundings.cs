// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:05
// ==========================================================================

using System;
using NoSuchCompany.Games.SuperMario.Behaviors;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NoSuchCompany.Games.SuperMario.Entities
{
    #region Class

    public sealed class EnemySurroundings
    {
        #region Constants

        private readonly IEnemy _enemy;
        private readonly IPlayer _player;

        #endregion

        #region Constructors

        private EnemySurroundings(IEnemy enemy, IPlayer player)
        {
            _enemy = enemy;
            _player = player;
        }

        #endregion

        #region Public Methods

        public static EnemySurroundings Get(IEnemy enemy)
        {
            var player = Object.FindObjectOfType<PlayerBehavior>();

            return new EnemySurroundings(enemy, player);
        }

        public bool IsAtSameLevel()
        {
            return Math.Abs(_enemy.Position.y - _player.Position.y) < 1f;
        }

        public float MoveTowardPlayer(float moveSpeed)
        {
            Vector2 distanceFromPlayer = GetDistanceFromPlayer();

            float horizontalAxis = distanceFromPlayer.normalized.x > 0f ? 1f : -1f;
            float deltaTime = Time.fixedDeltaTime;
            return horizontalAxis * moveSpeed * deltaTime;
        }

        public bool MustAttack()
        {
            //if (_player._isDead)
            //    return false;

            Vector2 distanceFromPlayer = GetDistanceFromPlayer();

            return distanceFromPlayer.magnitude < _enemy.MinimumDistanceToAttack;
        }

        public bool MustJump()
        {
            Vector2 playerPosition = _player.Position;
            Vector2 enemyPosition = _enemy.Position;

            return playerPosition.y > enemyPosition.y + 1f;
        }

        #endregion

        #region Private Methods

        private Vector2 GetDistanceFromPlayer()
        {
            Vector2 playerPosition = _player.Position;
            Vector2 enemyPosition = _enemy.Position;

            return playerPosition - enemyPosition;
        }

        #endregion
    }

    #endregion
}