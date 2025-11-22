using BepInEx.Logging;
using Unity.Netcode;

namespace QuickPurchaseCompany.Utils;

internal static class NetworkUtils
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    public static bool IsServer()
    {
        var networkManager = NetworkManager.Singleton;
        if (networkManager == null)
        {
            Logger.LogError("NetworkManager.Singleton is null.");
            return false;
        }

        return networkManager.IsServer;
    }
}
