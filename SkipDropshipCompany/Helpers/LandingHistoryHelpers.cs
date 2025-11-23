using System.Linq;
using BepInEx.Logging;
using SkipDropshipCompany.Utils;

namespace SkipDropshipCompany.Helpers;

internal static class LandingHistoryHelpers
{
    internal static ManualLogSource Logger => SkipDropshipCompany.Logger;

    public static bool AddLandingHistory(string sceneName)
    {
        var landingHistoryManager = SkipDropshipCompany.landingHistoryManager;
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
        var landingHistoryManager = SkipDropshipCompany.landingHistoryManager;
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

    public static bool IsLastLandedOnCompany()
    {
        var landingHistoryManager = SkipDropshipCompany.landingHistoryManager;
        if (landingHistoryManager == null)
        {
            Logger.LogError("LandingHistoryManager is null.");
            return false;
        }

        var landingHistory = landingHistoryManager.GetLandingHistory();
        if (landingHistory == null)
        {
            Logger.LogError("Landing history is null.");
            return false;
        }

        var lastLandedSceneName = landingHistory.LastOrDefault();
        if (lastLandedSceneName == null)
        {
            Logger.LogDebug("Last landed scene name is null.");
            return false;
        }

        if (!RoundUtils.IsSceneNameCompany(lastLandedSceneName))
        {
            // Last landed level is not company
            Logger.LogDebug("Last landed scene is not company.");
            return false;
        }

        return true;
    }
}
