using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using QuickPurchaseCompany.Managers;

namespace QuickPurchaseCompany;

[BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
[BepInProcess("Lethal Company.exe")]
public class QuickPurchaseCompany : BaseUnityPlugin
{
    public const string MOD_GUID = "com.aoirint.quickpurchasecompany";
    public const string MOD_NAME = "Quick Purchase Company";
    public const string MOD_VERSION = "0.1.0";

    internal static new ManualLogSource Logger { get; private set; }

    internal static Harmony harmony = new(MOD_GUID);

    internal static InstantPurchaseManager instantPurchaseManager = new();

    internal static LandingHistoryManager landingHistoryManager = new();

    private void Awake()
    {
        Logger = base.Logger;

        harmony.PatchAll();

        Logger.LogInfo($"Plugin {MOD_NAME} v{MOD_VERSION} is loaded!");
    }
}
