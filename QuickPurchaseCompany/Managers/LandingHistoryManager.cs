using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;

namespace QuickPurchaseCompany.Managers;

internal class LandingHistoryManager
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    const int LANDING_HISTORY_SIZE = 1;

    private List<string> landingEntries = new List<string>();

    public bool AddLandingHistory(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Logger.LogError("Scene name is null or empty. Cannot add to landing history.");
            return false;
        }

        landingEntries.Add(sceneName);

        // Keep only the last `LANDING_HISTORY_SIZE` entries
        landingEntries = landingEntries.TakeLast(LANDING_HISTORY_SIZE).ToList();

        Logger.LogDebug($"Updated landing history. landingEntries={string.Join(", ", landingEntries)}");

        return true;
    }

    public List<string> GetLandingHistory()
    {
        return landingEntries.ToList();
    }

    public bool ClearLandingHistory()
    {
        landingEntries.Clear();
        return true;
    }
}
