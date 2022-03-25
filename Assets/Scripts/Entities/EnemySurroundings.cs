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

        private readonly GoombasBehavior _goombasBehavior;

        private readonly IPlayer _player;

        #endregion

        #region Constructors

        private EnemySurroundings(GoombasBehavior goombasBehavior, IPlayer player)
        {
            _goombasBehavior = goombasBehavior;
            _player = player;
        }

        #endregion

        #region Public Methods

        public static EnemySurroundings Get(GoombasBehavior goombasBehavior)
        {
            var player = Object.FindObjectOfType<Player>();

            return new EnemySurroundings(goombasBehavior, player);
        }

        public bool IsAtSameLevel()
        {
            return Math.Abs(_goombasBehavior.transform.position.y - _player.Position.y) < 1f;
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

            return distanceFromPlayer.magnitude < _goombasBehavior.minDistanceToAttack;
        }

        public bool MustJump()
        {
            Vector2 playerPosition = _player.Position;
            Vector2 goombasPosition = _goombasBehavior.transform.position;

            return playerPosition.y > goombasPosition.y + 1f;
        }

        #endregion

        #region Private Methods

        private Vector2 GetDistanceFromPlayer()
        {
            Vector2 playerPosition = _player.Position;
            Vector2 goombasPosition = _goombasBehavior.transform.position;

            return playerPosition - goombasPosition;
        }

        #endregion
    }

    #endregion
}