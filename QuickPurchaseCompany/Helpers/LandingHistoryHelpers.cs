using BepInEx.Logging;

namespace QuickPurchaseCompany.Helpers;

internal static class LandingHistoryHelpers
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    public static bool AddLandingHistory(string sceneName)
    {
        var landingHistoryManager = QuickPurchaseCompany.landingHistoryManager;
        if (landingHistoryManager == null)
        {
            Logger.LogError("LandingHistoryManager is null.");
            return false;
        }

        Logger.LogDebug($"Adding landing history. sceneName={sceneName}");
        if (!landingHistoryManager.AddLandingHistory(sceneName: sceneName))
        {
            Logger.LogError($"Failed to add landing history. sceneName={sceneName}");
            return false;
        }

        Logger.LogDebug($"Added landing history. sceneName={sceneName}");
        return true;
    }

    public static bool ClearLandingHistory()
    {
        var landingHistoryManager = QuickPurchaseCompany.landingHistoryManager;
        if (landingHistoryManager == null)
        {
            Logger.LogError("LandingHistoryManager is null.");
            return false;
        }

        Logger.LogDebug("Clearing landing history.");
        if (!landingHistoryManager.ClearLandingHistory())
        {
            Logger.LogError("Failed to clear landing history.");
            return false;
        }

        Logger.LogDebug("Cleared landing history.");
        return true;
    }
}
