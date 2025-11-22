using BepInEx;
using HarmonyLib;
using QuickPurchaseCompany.Patches;

namespace QuickPurchaseCompany;

[BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
[BepInProcess("Lethal Company.exe")]
public class QuickPurchaseCompany : BaseUnityPlugin
{
    public const string MOD_GUID = "com.aoirint.quickpurchasecompany";
    public const string MOD_NAME = "Quick Purchase Company";
    public const string MOD_VERSION = "0.1.0";

    private readonly Harmony harmony = new(MOD_GUID);

    private void Awake()
    {
        harmony.PatchAll();

        Logger.LogInfo($"Plugin {MOD_NAME} {MOD_VERSION} is loaded!");
    }
}
