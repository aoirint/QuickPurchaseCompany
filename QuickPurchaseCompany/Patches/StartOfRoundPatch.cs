using BepInEx.Logging;
using HarmonyLib;
using QuickPurchaseCompany.Utils;

namespace QuickPurchaseCompany.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRoundPatch
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    [HarmonyPatch(nameof(StartOfRound.StartGame))]
    [HarmonyPostfix]
    public static void StartGamePostfix(StartOfRound __instance)
    {
        if (! NetworkUtils.IsServer())
        {
            Logger.LogDebug("Not the server. Skipping landing history addition.");
            return;
        }

        var currentLevel = __instance.currentLevel;
        if (currentLevel == null)
        {
            Logger.LogError("StartOfRound.currentLevel is null.");
            return;
        }

        var sceneName = currentLevel.sceneName;

        var landingHistoryManager = QuickPurchaseCompany.landingHistoryManager;
        if (landingHistoryManager == null)
        {
            Logger.LogError("LandingHistoryManager is null.");
            return;
        }

        Logger.LogDebug($"Adding landing history. sceneName={sceneName}");
        if (! landingHistoryManager.AddLandingHistory(sceneName: sceneName))
        {
            Logger.LogError($"Failed to add landing history. sceneName={sceneName}");
            return;
        }
        Logger.LogDebug($"Added landing history. sceneName={sceneName}");
    }

    [HarmonyPatch(nameof(StartOfRound.ChangeLevelClientRpc))]
    [HarmonyPostfix]
    public static void ChangeLevelClientRpcPostfix(StartOfRound __instance, int levelID, int newGroupCreditsAmount)
    {
        if (! NetworkUtils.IsServer())
        {
            Logger.LogDebug("Not the server. Skipping routing history addition.");
            return;
        }

        var routingHistoryManager = QuickPurchaseCompany.routingHistoryManager;
        if (routingHistoryManager == null)
        {
            Logger.LogError("RoutingHistoryManager is null.");
            return;
        }

        var level = RoundUtils.GetLevelById(levelID);
        if (level == null)
        {
            Logger.LogError($"Level not found. levelID={levelID}");
            return;
        }

        var sceneName = level.sceneName;
        Logger.LogDebug($"Adding routing history. sceneName={sceneName}");
        if (! routingHistoryManager.AddRoutingHistory(sceneName: sceneName))
        {
            Logger.LogError($"Failed to add routing history. sceneName={sceneName}");
            return;
        }

        Logger.LogDebug($"Added routing history. sceneName={sceneName}");
    }

    protected static bool ClearLandingHistory()
    {
        var landingHistoryManager = QuickPurchaseCompany.landingHistoryManager;
        if (landingHistoryManager == null)
        {
            Logger.LogError("LandingHistoryManager is null.");
            return false;
        }

        Logger.LogDebug("Clearing landing history.");
        if (! landingHistoryManager.ClearLandingHistory())
        {
            Logger.LogError("Failed to clear landing history.");
            return false;
        }

        Logger.LogDebug("Cleared landing history.");
        return true;
    }

    protected static bool ClearRoutingHistory()
    {
        var routingHistoryManager = QuickPurchaseCompany.routingHistoryManager;
        if (routingHistoryManager == null)
        {
            Logger.LogError("RoutingHistoryManager is null.");
            return false;
        }

        Logger.LogDebug("Clearing routing history.");
        if (! routingHistoryManager.ClearRoutingHistory())
        {
            Logger.LogError("Failed to clear routing history.");
            return false;
        }

        Logger.LogDebug("Cleared routing history.");
        return true;
    }

    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    [HarmonyPostfix]
    public static void ResetShipPostfix(StartOfRound __instance)
    {
        if (! NetworkUtils.IsServer())
        {
            Logger.LogDebug("Not the server. Skipping landing history clear.");
            return;
        }

        ClearLandingHistory();
        ClearRoutingHistory();
    }
}
