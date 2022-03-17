// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:04
// ==========================================================================

namespace NoSuchCompany.Games.SuperMario.Strategies
{
    #region Interface

    internal interface IEnemyStrategy
    {
        #region Methods

        IEnemyStrategy Apply();

        bool IsDone();

        void Prepare();

        #endregion
    }

    #endregion
}