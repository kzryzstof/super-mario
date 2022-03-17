// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:05
// ==========================================================================

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NoSuchCompany.Games.SuperMario.Entities
{
    #region Class

    public sealed class EnemySurroundings
    {
        #region Constants

        private readonly GoombasBehavior _goombasBehavior;

        private readonly PlayerBehavior _playerBehavior;

        #endregion

        #region Constructors

        private EnemySurroundings(GoombasBehavior goombasBehavior, PlayerBehavior playerBehavior)
        {
            _goombasBehavior = goombasBehavior;
            _playerBehavior = playerBehavior;
        }

        #endregion

        #region Public Methods

        public static EnemySurroundings Get(GoombasBehavior goombasBehavior)
        {
            var playerBehavior = Object.FindObjectOfType<PlayerBehavior>();

            return new EnemySurroundings(goombasBehavior, playerBehavior);
        }

        public bool IsAtSameLevel()
        {
            return Math.Abs(_goombasBehavior.transform.position.y - _playerBehavior.transform.position.y) < 1f;
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
            if (_playerBehavior._isDead)
                return false;

            Vector2 distanceFromPlayer = GetDistanceFromPlayer();

            return distanceFromPlayer.magnitude < _goombasBehavior.minDistanceToAttack;
        }

        public bool MustJump()
        {
            Vector2 playerPosition = _playerBehavior.transform.position;
            Vector2 goombasPosition = _goombasBehavior.transform.position;

            return playerPosition.y > goombasPosition.y + 1f;
        }

        #endregion

        #region Private Methods

        private Vector2 GetDistanceFromPlayer()
        {
            Vector2 playerPosition = _playerBehavior.transform.position;
            Vector2 goombasPosition = _goombasBehavior.transform.position;

            return playerPosition - goombasPosition;
        }

        #endregion
    }

    #endregion
}