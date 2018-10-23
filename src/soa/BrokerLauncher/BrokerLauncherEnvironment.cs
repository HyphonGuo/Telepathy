﻿namespace Microsoft.Hpc.Scheduler.Session.Internal.BrokerLauncher
{
    internal static class BrokerLauncherEnvironment
    {
        internal static bool Standalone => string.IsNullOrEmpty(BrokerLauncherSettings.Default.SessionAddress);
    }
}
