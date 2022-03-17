// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 16/03/2022 @ 22:05
// ==========================================================================

using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace NoSuchCompany.Games.SuperMario.Diagnostics
{
    #region Class

    public static class AppLogger
    {
        #region Constants

        private static readonly LogsLevels EnabledLogs = Configurations.LogsLevels;

        #endregion

        #region Public Methods

        [Conditional("DEBUG")]
        public static void Write(LogsLevels logsLevels, string message)
        {
            if (!IsEnabled(logsLevels))
                return;

            Debug.Log(message);
        }

        #endregion

        #region Private Methods

        private static bool IsEnabled(LogsLevels logsLevels)
        {
            return EnabledLogs.HasFlag(logsLevels);
        }

        #endregion
    }

    #endregion
}