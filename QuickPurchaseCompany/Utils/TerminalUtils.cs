using System.Linq;
using BepInEx.Logging;
using UnityEngine;

namespace QuickPurchaseCompany.Utils;

internal static class TerminalUtils
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    private static Terminal cachedTerminal;

    public static Terminal GetTerminal()
    {
        if (cachedTerminal != null)
        {
            return cachedTerminal;
        }

        Terminal terminal = Object.FindObjectOfType<Terminal>();
        if (terminal == null)
        {
            // Invalid state
            Logger.LogError("Failed to find Terminal instance in the scene.");
            return null;
        }

        cachedTerminal = terminal;

        return terminal;
    }

    public static Item GetBuyableItemByIndex(int index)
    {
        var terminal = GetTerminal();
        if (terminal == null)
        {
            // Invalid state
            Logger.LogError("Terminal is null.");
            return null;
        }

        var buyableItemsList = terminal.buyableItemsList;
        if (buyableItemsList == null)
        {
            // Invalid state
            Logger.LogError("Terminal.buyableItemsList is null.");
            return null;
        }

        return buyableItemsList.ElementAtOrDefault(index);
    }
}
