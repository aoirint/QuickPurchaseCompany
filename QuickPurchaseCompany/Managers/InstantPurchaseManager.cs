using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using QuickPurchaseCompany.Helpers;
using QuickPurchaseCompany.Utils;

namespace QuickPurchaseCompany.Managers;

internal sealed class PrepareInstantPurchaseResult
{
    public bool Succeeded { get; }
    public List<int> DropShipBoughtItemIndexes { get; }
    public List<int> InstantBoughtItemIndexes { get; }

    public PrepareInstantPurchaseResult(
        bool succeeded,
        List<int> dropShipBoughtItemIndexes,
        List<int> instantBoughtItemIndexes
    )
    {
        Succeeded = succeeded;
        DropShipBoughtItemIndexes = dropShipBoughtItemIndexes;
        InstantBoughtItemIndexes = instantBoughtItemIndexes;
    }
}

internal sealed class SpawnPreparedInstantPurchasedItemsResult
{
    public bool Succeeded { get; }
    public List<int> DropShipBoughtItemIndexes { get; }
    public List<int> InstantBoughtItemIndexes { get; }

    public SpawnPreparedInstantPurchasedItemsResult(
        bool succeeded,
        List<int> dropShipBoughtItemIndexes,
        List<int> instantBoughtItemIndexes
    )
    {
        Succeeded = succeeded;
        DropShipBoughtItemIndexes = dropShipBoughtItemIndexes;
        InstantBoughtItemIndexes = instantBoughtItemIndexes;
    }
}

internal class InstantPurchaseManager
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    private PrepareInstantPurchaseResult preparedInstantPurchaseResult;

    private static bool IsInstantPurchaseAllowed()
    {
        Logger.LogDebug("Checking if instant purchase is allowed.");

        var isFirstDayOrbit = RoundHelpers.IsFirstDayOrbit();
        var isLandedOnCompany = RoundHelpers.IsLandedOnCompany();
        var isInOrbitAndLastLandedOnCompanyAndRoutingOnCompany = RoundHelpers.IsInOrbitAndLastLandedOnCompanyAndRoutingOnCompany();

        Logger.LogDebug(
            $"IsFirstDayOrbit={isFirstDayOrbit}" +
            $" IsLandedOnCompany={isLandedOnCompany}" +
            $" isInOrbitAndLastLandedOnCompanyAndRoutingOnCompany={isInOrbitAndLastLandedOnCompanyAndRoutingOnCompany}"
        );

        return isFirstDayOrbit || isLandedOnCompany || isInOrbitAndLastLandedOnCompanyAndRoutingOnCompany;
    }

    public PrepareInstantPurchaseResult PrepareInstantPurchase(List<int> boughtItemIndexes)
    {
        if (!IsInstantPurchaseAllowed())
        {
            Logger.LogDebug("Instant purchase is not allowed in the current game state.");
            return new PrepareInstantPurchaseResult(
                succeeded: false,
                dropShipBoughtItemIndexes: [],
                instantBoughtItemIndexes: []
            );
        }

        return PrepareInstantPurchaseUnchecked(boughtItemIndexes: boughtItemIndexes);
    }

    public PrepareInstantPurchaseResult PrepareInstantPurchaseUnchecked(List<int> boughtItemIndexes)
    {
        var prepareInstantPurchaseResult = new PrepareInstantPurchaseResult(
            succeeded: true,
            dropShipBoughtItemIndexes: [],
            instantBoughtItemIndexes: boughtItemIndexes
        );

        preparedInstantPurchaseResult = prepareInstantPurchaseResult;

        return prepareInstantPurchaseResult;
    }

    public PrepareInstantPurchaseResult GetPreparedInstantPurchaseResult()
    {
        return preparedInstantPurchaseResult;
    }

    public SpawnPreparedInstantPurchasedItemsResult SpawnPreparedInstantPurchasedItems()
    {
        if (preparedInstantPurchaseResult == null || !preparedInstantPurchaseResult.Succeeded)
        {
            Logger.LogDebug("No prepared instant purchase to spawn.");
            return new SpawnPreparedInstantPurchasedItemsResult(
                succeeded: false,
                dropShipBoughtItemIndexes: [],
                instantBoughtItemIndexes: []
            );
        }

        var instantBoughtItemIndexes = preparedInstantPurchaseResult.InstantBoughtItemIndexes;

        foreach (var buyableItemIndex in instantBoughtItemIndexes)
        {
            var item = TerminalUtils.GetBuyableItemByIndex(buyableItemIndex);
            if (item == null)
            {
                Logger.LogError($"Failed to get bought item. buyableItemIndex={buyableItemIndex}");
                return new SpawnPreparedInstantPurchasedItemsResult(
                    succeeded: false,
                    dropShipBoughtItemIndexes: [],
                    instantBoughtItemIndexes: []
                );
            }

            var spawnSucceeded = ItemSpawnUtils.SpawnItemInShip(item: item);
            if (!spawnSucceeded)
            {
                Logger.LogError($"Failed to spawn instant purchased item. item.name={item.name}");
                return new SpawnPreparedInstantPurchasedItemsResult(
                    succeeded: false,
                    dropShipBoughtItemIndexes: [],
                    instantBoughtItemIndexes: []
                );
            }
        }

        var result = new SpawnPreparedInstantPurchasedItemsResult(
            succeeded: true,
            dropShipBoughtItemIndexes: preparedInstantPurchaseResult.DropShipBoughtItemIndexes,
            instantBoughtItemIndexes: preparedInstantPurchaseResult.InstantBoughtItemIndexes
        );

        // Cleanup the prepared result after spawning
        preparedInstantPurchaseResult = null;

        return result;
    }
}
