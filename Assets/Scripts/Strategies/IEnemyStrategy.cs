// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 12/03/2022 @ 13:45
// Last author: Christophe Commeyne
// ==========================================================================

namespace NoSuchCompany.Games.SuperMario.Strategies
{
    internal interface IEnemyStrategy
    {
        void Prepare();
        
        bool IsDone();

        IEnemyStrategy Apply();
    }
}