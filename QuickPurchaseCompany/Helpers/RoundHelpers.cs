using System.Linq;
using BepInEx.Logging;
using QuickPurchaseCompany.Utils;

namespace QuickPurchaseCompany.Helpers;

internal static class RoundHelpers
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    public static bool IsFirstDayOrbit()
    {
        if (! RoundUtils.IsInOrbit())
        {
            // Landed
            return false;
        }

        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null) {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return false;
        }

        var gameStats = startOfRound.gameStats;
        if (gameStats == null) {
            // Invalid state
            Logger.LogError("StartOfRound.Instance.gameStats is null.");
            return false;
        }

        var daysSpent = gameStats.daysSpent;
        Logger.LogDebug($"daysSpent={daysSpent}");

        return daysSpent == 0;
    }

    public static bool IsLandedOnCompany()
    {
        if (RoundUtils.IsInOrbit())
        {
            // In orbit
            return false;
        }

        var roundManager = RoundManager.Instance;
        if (roundManager == null) {
            // Invalid state
            Logger.LogError("RoundManager.Instance is null.");
            return false;
        }

        // Current selected level in orbit / Current landed level
        var currentLevel = roundManager.currentLevel;
        if (currentLevel == null) {
            // Invalid state
            Logger.LogError("RoundManager.Instance.currentLevel is null.");
            return false;
        }

        var sceneName = currentLevel.sceneName;
        return RoundUtils.IsSceneNameCompany(sceneName);
    }

    public static bool IsInOrbitAndLastLandedOnCompanyAndRoutingToCompany()
    {
        if (! RoundUtils.IsInOrbit())
        {
            // Landed
            Logger.LogDebug("Not in orbit.");
            return false;
        }

        var landingHistoryManager = QuickPurchaseCompany.landingHistoryManager;
        if (landingHistoryManager == null)
        {
            Logger.LogError("LandingHistoryManager is null.");
            return false;
        }

        var landingHistory = landingHistoryManager.GetLandingHistory();
        if (landingHistory == null)
        {
            Logger.LogError("Last landed scene name is null.");
            return false;
        }

        var lastLandedSceneName = landingHistory.LastOrDefault();
        if (lastLandedSceneName == null)
        {
            Logger.LogError("Last landed scene name is null.");
            return false;
        }

        if (! RoundUtils.IsSceneNameCompany(lastLandedSceneName))
        {
            // Last landed level is not company
            Logger.LogDebug("Last landed scene is not company.");
            return false;
        }

        if (! RoundUtils.IsRoutingToCompany())
        {
            // Not routing to company
            Logger.LogDebug("Not routing to company.");
            return false;
        }

        return true;
    }
}
