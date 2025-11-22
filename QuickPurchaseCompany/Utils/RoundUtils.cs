using System.Linq;
using BepInEx.Logging;

namespace QuickPurchaseCompany.Utils;

internal static class RoundUtils
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    public static bool IsInOrbit()
    {
        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null) {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return false;
        }

        if (!startOfRound.inShipPhase)
        {
            // Landed
            return false;
        }

        return true;
    }

    public static bool IsFirstDayOrbit()
    {
        if (!IsInOrbit())
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

    public static bool IsSceneNameCompany(string sceneName)
    {
        Logger.LogDebug($"IsSceneNameCompany? sceneName={sceneName}");
        return sceneName == "CompanyBuilding";
    }

    public static bool IsLandedOnCompany()
    {
        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null) {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return false;
        }

        if (startOfRound.inShipPhase)
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
        return IsSceneNameCompany(sceneName);
    }

    public static SelectableLevel GetLevelById(int levelId)
    {
        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null) {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return null;
        }

        var levels = startOfRound.levels;
        if (levels == null) {
            // Invalid state
            Logger.LogError("StartOfRound.Instance.levels is null.");
            return null;
        }

        var level = levels.ElementAtOrDefault(levelId);
        if (level == null)
        {
            Logger.LogError($"Level not found. levelId={levelId}");
            return null;
        }

        return level;
    }
}
