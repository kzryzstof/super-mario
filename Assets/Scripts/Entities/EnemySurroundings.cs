// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 12/03/2022 @ 10:54
// Last author: Christophe Commeyne
// ==========================================================================

using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Entities
{
    public sealed class EnemySurroundings
    {
        private readonly GoombasBehavior _goombasBehavior;
        private readonly PlayerBehavior _playerBehavior;
        
        private EnemySurroundings(GoombasBehavior goombasBehavior, PlayerBehavior playerBehavior)
        {
            _goombasBehavior = goombasBehavior;
            _playerBehavior = playerBehavior;
        }
        
        public static EnemySurroundings Get(GoombasBehavior goombasBehavior)
        {
            var playerBehavior = MonoBehaviour.FindObjectOfType<PlayerBehavior>();

            return new EnemySurroundings(goombasBehavior, playerBehavior);
        }

        public bool MustAttack(float minDistanceToAttack)
        {
            if (_playerBehavior._isDead)
                return false;
            
            Vector2 distanceFromPlayer = GetDistanceFromPlayer();
            
            return distanceFromPlayer.magnitude < minDistanceToAttack;
        }

        public float MoveTowardPlayer(float moveSpeed)
        {
            Vector2 distanceFromPlayer = GetDistanceFromPlayer();

            float horizontalAxis = distanceFromPlayer.normalized.x > 0f ? 1f : -1f;
            float deltaTime = Time.fixedDeltaTime;
            return horizontalAxis * moveSpeed * deltaTime;
        }
        
        public bool MustJump()
        {
            Vector2 playerPosition = _playerBehavior.transform.position;
            Vector2 goombasPosition = _goombasBehavior.transform.position;

            return playerPosition.y > (goombasPosition.y + 1f);
        }

        private Vector2 GetDistanceFromPlayer()
        {
            Vector2 playerPosition = _playerBehavior.transform.position;
            Vector2 goombasPosition = _goombasBehavior.transform.position;
            
            return playerPosition - goombasPosition;
        }
        
    }
}