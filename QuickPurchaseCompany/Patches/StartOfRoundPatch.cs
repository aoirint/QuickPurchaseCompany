using BepInEx.Logging;
using HarmonyLib;
using QuickPurchaseCompany.Helpers;
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

        LandingHistoryHelpers.AddLandingHistory(sceneName: sceneName);
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

        LandingHistoryHelpers.ClearLandingHistory();
    }
}
