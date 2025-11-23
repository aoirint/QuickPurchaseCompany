using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace QuickPurchaseCompany.Utils;

internal static class ItemSpawnUtils
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    private static GameObject cachedShipGameObject;

    private static Dictionary<int, float> cachedSpawnOffsetXByItemId = new();

    public static GameObject GetShipGameObject()
    {
        if (cachedShipGameObject != null)
        {
            return cachedShipGameObject;
        }

        var shipGameObject = GameObject.Find("/Environment/HangarShip");
        if (shipGameObject == null)
        {
            // Invalid state
            Logger.LogError("Failed to find Ship game object.");
            return null;
        }

        cachedShipGameObject = shipGameObject;

        return shipGameObject;
    }

    public static Vector3? GetBaseSpawnPosition()
    {
        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null)
        {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return null;
        }

        var playerSpawnPositions = startOfRound.playerSpawnPositions;
        if (playerSpawnPositions == null)
        {
            // Invalid state
            Logger.LogError("StartOfRound.Instance.playerSpawnPositions is null.");
            return null;
        }

        var playerSpawnPositionTransform = playerSpawnPositions.ElementAtOrDefault(1);
        if (playerSpawnPositionTransform == null)
        {
            // Invalid state
            Logger.LogError("Player spawn position is null for ID 1.");
            return null;
        }

        return playerSpawnPositionTransform.position;
    }

    public static float GetSpawnOffsetXByItemId(int itemId)
    {
        if (cachedSpawnOffsetXByItemId.TryGetValue(itemId, out var cachedOffsetX))
        {
            return cachedOffsetX;
        }

        // Range for out of bounds items in the base game
        const float offsetXRange = 0.7f;

        var itemIdUint = (uint)itemId;

        // Randomize the seed to improve distribution
        var randomSeed = math.hash(new uint4(itemIdUint, 0xDEADBEEFu, 0x12345678u, 0x87654321u));
        if (randomSeed == 0u)
        {
            // Unity.Mathematics.Random does not allow seed 0. Use 1 instead.
            randomSeed = 1u;
        }

        var random = new Unity.Mathematics.Random(randomSeed);
        float offsetX = random.NextFloat(-offsetXRange, offsetXRange);

        cachedSpawnOffsetXByItemId[itemId] = offsetX;

        Logger.LogDebug($"Generated offset X for an item ID. itemId={itemId}, offsetX={offsetX}");

        return offsetX;
    }

    public static bool SpawnItemInShip(Item item)
    {
        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null)
        {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return false;
        }

        var spawnPrefab = item.spawnPrefab;
        if (spawnPrefab == null)
        {
            // Invalid state
            Logger.LogError("Item.spawnPrefab is null.");
            return false;
        }

        var shipGameObject = GetShipGameObject();
        if (shipGameObject == null)
        {
            // Invalid state
            Logger.LogError("Failed to get Ship game object.");
            return false;
        }

        var elevatorTransform = startOfRound.elevatorTransform;
        if (elevatorTransform == null)
        {
            // Invalid state
            Logger.LogError("StartOfRound.Instance.elevatorTransform is null.");
            return false;
        }

        var baseSpawnPositionNullable = GetBaseSpawnPosition();
        if (baseSpawnPositionNullable == null)
        {
            // Invalid state
            Logger.LogError("Failed to get spawn position.");
            return false;
        }
        var baseSpawnPosition = baseSpawnPositionNullable.Value;

        float offsetX = GetSpawnOffsetXByItemId(item.itemId);

        // Default position for out of bounds items in the base game: (random(-0.7f, 0.7f), 2.0f, 0.5f)
        const float offsetZ = 1.0f; // Slightly forward to the center of the ship
        const float offsetY = 0.5f;

        var spawnPosition = baseSpawnPosition + new Vector3(offsetX, offsetY, offsetZ);

        var gameObject = Object.Instantiate(
            spawnPrefab,
            spawnPosition,
            Quaternion.identity,
            elevatorTransform
        );

        var grabbableObject = gameObject.GetComponent<GrabbableObject>();
        if (grabbableObject == null)
        {
            // Invalid state
            Logger.LogError("Failed to get GrabbableObject component from spawned item.");
            Object.Destroy(gameObject);
            return false;
        }

        grabbableObject.fallTime = 0f;
        grabbableObject.isInElevator = true;
        grabbableObject.isInShipRoom = true;
        grabbableObject.hasHitGround = true; // Disable drop sound effect

        var networkObject = gameObject.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            // Invalid state
            Logger.LogError("Failed to get NetworkObject component from spawned item.");
            Object.Destroy(gameObject);
            return false;
        }

        networkObject.Spawn(false);

        return true;
    }
}
