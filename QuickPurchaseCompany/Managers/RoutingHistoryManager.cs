using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;

namespace QuickPurchaseCompany.Managers;

internal class RoutingHistoryManager
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    const int ROUTING_HISTORY_SIZE = 1;

    private List<string> routingEntries = new List<string>();

    public bool AddRoutingHistory(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Logger.LogError("Scene name is null or empty. Cannot add to routing history.");
            return false;
        }

        routingEntries.Add(sceneName);

        // Keep only the last `ROUTING_HISTORY_SIZE` entries
        routingEntries = routingEntries.TakeLast(ROUTING_HISTORY_SIZE).ToList();

        Logger.LogDebug($"Updated routing history. routingEntries={string.Join(", ", routingEntries)}");

        return true;
    }

    public List<string> GetRoutingHistory()
    {
        return routingEntries.ToList();
    }

    public bool ClearRoutingHistory()
    {
        routingEntries.Clear();
        return true;
    }
}
