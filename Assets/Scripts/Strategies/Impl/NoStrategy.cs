// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:04
// ==========================================================================

namespace NoSuchCompany.Games.SuperMario.Strategies.Impl
{
    #region Class

    internal sealed class NoStrategy : IEnemyStrategy
    {
        #region Constants

        private const float NoMovement = 0f;

        private readonly GoombasBehavior _goombasBehavior;

        #endregion

        #region Constructors

        public NoStrategy(GoombasBehavior goombasBehavior)
        {
            _goombasBehavior = goombasBehavior;
        }

        #endregion

        #region Public Methods

        public IEnemyStrategy Apply()
        {
            _goombasBehavior.Move(NoMovement);
            return this;
        }

        public bool IsDone()
        {
            return true;
        }

        public void Prepare()
        {
        }

        #endregion
    }

    #endregion
}