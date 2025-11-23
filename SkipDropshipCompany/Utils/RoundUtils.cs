using System.Linq;
using BepInEx.Logging;

namespace SkipDropshipCompany.Utils;

internal static class RoundUtils
{
    internal static ManualLogSource Logger => SkipDropshipCompany.Logger;

    public static bool IsInOrbit()
    {
        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null)
        {
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

    public static bool IsFirstDay()
    {
        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null)
        {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return false;
        }

        var gameStats = startOfRound.gameStats;
        if (gameStats == null)
        {
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

    public static bool IsRoutingToCompany()
    {
        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null)
        {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return false;
        }

        var currentLevel = startOfRound.currentLevel;
        if (currentLevel == null)
        {
            // Invalid state
            Logger.LogError("StartOfRound.Instance.currentLevel is null.");
            return false;
        }

        var sceneName = currentLevel.sceneName;
        return IsSceneNameCompany(sceneName);
    }

    public static SelectableLevel GetLevelById(int levelId)
    {
        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null)
        {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return null;
        }

        var levels = startOfRound.levels;
        if (levels == null)
        {
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
