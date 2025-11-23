#nullable enable

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SkipDropshipCompany.Generated;
using SkipDropshipCompany.Managers;

namespace SkipDropshipCompany;

[BepInPlugin(ModInfo.GUID, ModInfo.NAME, ModInfo.VERSION)]
[BepInProcess("Lethal Company.exe")]
public class SkipDropshipCompany : BaseUnityPlugin
{

    internal static new ManualLogSource? Logger { get; private set; }

    internal static Harmony harmony = new(ModInfo.GUID);

    internal static InstantPurchaseManager instantPurchaseManager = new();

    internal static LandingHistoryManager landingHistoryManager = new();

    internal static ConfigEntry<bool>? isFirstDayRerouteRequiredConfig;

    private void Awake()
    {
        Logger = base.Logger;

        isFirstDayRerouteRequiredConfig = Config.Bind(
            "SkipDropshipCompany",
            "Requires rerouting on the first day",
            false,
            "If true, rerouting to the company will be required to skip the dropship on the first day."
        );

        harmony.PatchAll();

        Logger.LogInfo($"Plugin {ModInfo.NAME} v{ModInfo.VERSION} is loaded!");
    }
}
