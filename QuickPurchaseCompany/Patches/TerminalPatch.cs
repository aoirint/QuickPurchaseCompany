using HarmonyLib;

namespace QuickPurchaseCompany.Patches;

[HarmonyPatch(typeof(Terminal))]
internal class TerminalPatch
{
    [HarmonyPatch("SyncGroupCreditsClientRpc")]
    [HarmonyPrefix]
    public static void Prefix(Terminal __instance, int newGroupCredits, ref int numItemsInShip)
    {
    }

    [HarmonyPatch("SyncGroupCreditsClientRpc")]
    [HarmonyPostfix]
    public static void Postfix(Terminal __instance, int newGroupCredits, ref int numItemsInShip)
    {
    }
}
