using BepInEx.Logging;
using HarmonyLib;
using QuickPurchaseCompany.Utils;

namespace QuickPurchaseCompany.Patches;

[HarmonyPatch(typeof(Terminal))]
internal class TerminalPatch
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    [HarmonyPatch(nameof(Terminal.SyncGroupCreditsClientRpc))]
    [HarmonyPrefix]
    public static void SyncGroupCreditsClientRpcPrefix(Terminal __instance, int newGroupCredits, ref int numItemsInShip)
    {
        if (! NetworkUtils.IsServer())
        {
            Logger.LogDebug("Not the server. Skipping instant purchase logic.");
            return;
        }

        var instantPurchaseManager = QuickPurchaseCompany.instantPurchaseManager;
        if (instantPurchaseManager == null)
        {
            Logger.LogError("InstantPurchaseManager is null.");
            return;
        }

        var orderedItemsFromTerminal = __instance.orderedItemsFromTerminal;
        if (orderedItemsFromTerminal == null)
        {
            Logger.LogError("Terminal.orderedItemsFromTerminal is null.");
            return;
        }

        Logger.LogDebug("Preparing instant purchase.");

        var result = instantPurchaseManager.PrepareInstantPurchase(
            boughtItemIndexes: orderedItemsFromTerminal
        );
        if (result == null)
        {
            Logger.LogDebug("Prepare instant purchase failed or not allowed. Skipping instant purchase logic.");
            return;
        }

        // Adjust the number of items in the dropship
        // NOTE:  `numItemsInShip` will be set to `Terminal.numberOfItemsInDropship`.
        // This is the argument name in the original game code.
        // It means `numItemsInDropship`.
        numItemsInShip = result.DropShipBoughtItemIndexes.Count;

        Logger.LogDebug(
            "Prepared instant purchase." +
            $" originalDropShipItemCount={orderedItemsFromTerminal.Count}" +
            $" preparedDropShipItemCount={numItemsInShip}" +
            $" preparedInstantPurchaseItemCount={result.InstantBoughtItemIndexes.Count}"
        );
    }

    [HarmonyPatch(nameof(Terminal.SyncGroupCreditsClientRpc))]
    [HarmonyPostfix]
    public static void SyncGroupCreditsClientRpcPostfix(Terminal __instance, int newGroupCredits, ref int numItemsInShip)
    {
        if (! NetworkUtils.IsServer())
        {
            Logger.LogDebug("Not the server. Skipping instant purchase logic.");
            return;
        }

        var instantPurchaseManager = QuickPurchaseCompany.instantPurchaseManager;
        if (instantPurchaseManager == null)
        {
            Logger.LogError("InstantPurchaseManager is null.");
            return;
        }

        Logger.LogDebug("Spawning prepared instant purchased items.");

        var result = instantPurchaseManager.SpawnPreparedInstantPurchasedItems();
        if (result == null)
        {
            Logger.LogDebug("Spawning prepared instant purchased items failed or none to spawn.");
            return;
        }

        __instance.orderedItemsFromTerminal = result.DropShipBoughtItemIndexes;

        Logger.LogDebug("Spawned all prepared instant purchased items.");
    }
}
