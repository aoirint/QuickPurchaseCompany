using BepInEx.Logging;
using HarmonyLib;

namespace QuickPurchaseCompany.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRoundPatch
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    [HarmonyPatch(nameof(StartOfRound.StartGame))]
    [HarmonyPostfix]
    public static void StartGamePostfix(StartOfRound __instance)
    {
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

    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    [HarmonyPostfix]
    public static void ResetShipPostfix(StartOfRound __instance)
    {
        var landingHistoryManager = QuickPurchaseCompany.landingHistoryManager;
        if (landingHistoryManager == null)
        {
            Logger.LogError("LandingHistoryManager is null.");
            return;
        }

        Logger.LogDebug("Clearing landing history.");
        if (! landingHistoryManager.ClearLandingHistory())
        {
            Logger.LogError("Failed to clear landing history.");
            return;
        }
        Logger.LogDebug("Cleared landing history.");
    }
}
