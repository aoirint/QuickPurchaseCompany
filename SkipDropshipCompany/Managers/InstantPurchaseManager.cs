#nullable enable

using System.Collections.Generic;
using BepInEx.Logging;
using SkipDropshipCompany.Helpers;
using SkipDropshipCompany.Utils;

namespace SkipDropshipCompany.Managers;

internal sealed class PrepareInstantPurchaseResult
{
    public List<int> DropShipBoughtItemIndexes { get; }
    public List<int> InstantBoughtItemIndexes { get; }

    public PrepareInstantPurchaseResult(
        List<int> dropShipBoughtItemIndexes,
        List<int> instantBoughtItemIndexes
    )
    {
        DropShipBoughtItemIndexes = dropShipBoughtItemIndexes;
        InstantBoughtItemIndexes = instantBoughtItemIndexes;
    }
}

internal sealed class SpawnPreparedInstantPurchasedItemsResult
{
    public List<int> DropShipBoughtItemIndexes { get; }
    public List<int> InstantBoughtItemIndexes { get; }

    public SpawnPreparedInstantPurchasedItemsResult(
        List<int> dropShipBoughtItemIndexes,
        List<int> instantBoughtItemIndexes
    )
    {
        DropShipBoughtItemIndexes = dropShipBoughtItemIndexes;
        InstantBoughtItemIndexes = instantBoughtItemIndexes;
    }
}

internal class InstantPurchaseManager
{
    internal static ManualLogSource Logger => SkipDropshipCompany.Logger;

    private PrepareInstantPurchaseResult? preparedInstantPurchaseResult;

    private static bool IsInstantPurchaseAllowed()
    {
        Logger.LogDebug("Checking if instant purchase is allowed.");

        var isFirstDayRerouteRequiredConfig = SkipDropshipCompany.isFirstDayRerouteRequiredConfig;
        var isFirstDayRerouteRequired = isFirstDayRerouteRequiredConfig?.Value ?? false;

        Logger.LogDebug($"Configs: isFirstDayRerouteRequired={isFirstDayRerouteRequired}");

        var isLandedOnCompany = RoundHelpers.IsLandedOnCompany();
        var IsInFirstDayOrbit = RoundHelpers.IsInFirstDayOrbit();
        var IsInFirstDayOrbitAndRoutingToCompany = RoundHelpers.IsInFirstDayOrbitAndRoutingToCompany();
        var isInOrbitAndLastLandedOnCompanyAndRoutingToCompany = RoundHelpers.IsInOrbitAndLastLandedOnCompanyAndRoutingToCompany();

        Logger.LogDebug(
            "Flags:" +
            $" IsLandedOnCompany={isLandedOnCompany}" +
            $" IsInFirstDayOrbit={IsInFirstDayOrbit}" +
            $" IsInFirstDayOrbitAndRoutingToCompany={IsInFirstDayOrbitAndRoutingToCompany}" +
            $" isInOrbitAndLastLandedOnCompanyAndRoutingToCompany={isInOrbitAndLastLandedOnCompanyAndRoutingToCompany}"
        );

        return (
            isLandedOnCompany ||
            (!isFirstDayRerouteRequired && IsInFirstDayOrbit) ||
            IsInFirstDayOrbitAndRoutingToCompany ||
            isInOrbitAndLastLandedOnCompanyAndRoutingToCompany
        );
    }

    public PrepareInstantPurchaseResult? PrepareInstantPurchase(List<int> boughtItemIndexes)
    {
        if (!IsInstantPurchaseAllowed())
        {
            Logger.LogDebug("Instant purchase is not allowed in the current game state.");
            return null;
        }

        return PrepareInstantPurchaseUnchecked(boughtItemIndexes: boughtItemIndexes);
    }

    public PrepareInstantPurchaseResult? PrepareInstantPurchaseUnchecked(List<int> boughtItemIndexes)
    {
        var prepareInstantPurchaseResult = new PrepareInstantPurchaseResult(
            dropShipBoughtItemIndexes: [],
            instantBoughtItemIndexes: boughtItemIndexes
        );

        preparedInstantPurchaseResult = prepareInstantPurchaseResult;

        return prepareInstantPurchaseResult;
    }

    public PrepareInstantPurchaseResult? GetPreparedInstantPurchaseResult()
    {
        return preparedInstantPurchaseResult;
    }

    public SpawnPreparedInstantPurchasedItemsResult? SpawnPreparedInstantPurchasedItems()
    {
        if (preparedInstantPurchaseResult == null)
        {
            Logger.LogDebug("No prepared instant purchase to spawn.");
            return null;
        }

        var instantBoughtItemIndexes = preparedInstantPurchaseResult.InstantBoughtItemIndexes;

        foreach (var buyableItemIndex in instantBoughtItemIndexes)
        {
            var item = TerminalUtils.GetBuyableItemByIndex(buyableItemIndex);
            if (item == null)
            {
                Logger.LogError($"Failed to get bought item. buyableItemIndex={buyableItemIndex}");
                return null;
            }

            var spawnSucceeded = ItemSpawnUtils.SpawnItemInShip(item: item);
            if (!spawnSucceeded)
            {
                Logger.LogError($"Failed to spawn instant purchased item. item.name={item.name}");
                return null;
            }
        }

        var result = new SpawnPreparedInstantPurchasedItemsResult(
            dropShipBoughtItemIndexes: preparedInstantPurchaseResult.DropShipBoughtItemIndexes,
            instantBoughtItemIndexes: preparedInstantPurchaseResult.InstantBoughtItemIndexes
        );

        // Cleanup the prepared result after spawning
        preparedInstantPurchaseResult = null;

        return result;
    }
}
