// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 20/03/2022 @ 19:44
// ==========================================================================

using System.Linq;
using NoSuchCompany.Games.SuperMario.Constants;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Services;
using NoSuchCompany.Games.SuperMario.Services.Impl;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{
    public class LevelBehavior : MonoBehaviour
    {
        //  Private fields.
        private readonly IRaycaster _raycaster;

        //  Unity injected properties.
        public LayerMask collisionMask;

        public LevelBehavior()
        {
            _raycaster = new Raycaster();
        }
        
        public void Start()
        {
            var boxCollider2D = GetComponent<BoxCollider2D>();
            
            _raycaster.Initialize(boxCollider2D, 3);
        }

        public void Update()
        {
            Vector3 noMovement = Vector3.zero;
            _raycaster.ApplyCollisions(ref noMovement, collisionMask);

            AppLogger.Write(LogsLevels.None, $"Collisions: {_raycaster.Collisions.LeftCollisions.Count()}");
            
            if (_raycaster.Collisions.LeftCollisions.Contains(Tags.Player))
            {
                AppLogger.Write(LogsLevels.None, $"HIT!!!");
                LevelManager.Instance.LoadNextLevel();
            }
        }
    }
}