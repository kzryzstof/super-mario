// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:04
// ==========================================================================

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoSuchCompany.Games.SuperMario
{
    #region Class

    public class CameraBehavior : MonoBehaviour
    {
        [FormerlySerializedAs("PlayerGameObject")]
        public GameObject playerGameObject;

        [FormerlySerializedAs("TimeOffset")]
        public float timeOffset;

        [FormerlySerializedAs("PositionOffset")]
        public Vector3 positionOffset;

        private Vector3 _velocity;

        public void Update()
        {
            Vector3 result = Vector3.SmoothDamp
            (
                transform.position,
                playerGameObject.transform.position + positionOffset,
                ref _velocity,
                timeOffset
            );

            transform.position = new Vector3(Math.Max(Configurations.MinimumLeftPositionForCamera, result.x), transform.position.y, transform.position.z);
        }
    }

    #endregion
}