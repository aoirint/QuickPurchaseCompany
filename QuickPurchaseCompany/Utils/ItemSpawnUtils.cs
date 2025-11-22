using System.Linq;
using BepInEx.Logging;
using Unity.Netcode;
using UnityEngine;

namespace QuickPurchaseCompany.Utils;

internal static class ItemSpawnUtils
{
    internal static ManualLogSource Logger => QuickPurchaseCompany.Logger;

    private static GameObject cachedShipGameObject;

    public static GameObject GetShipGameObject()
    {
        if (cachedShipGameObject != null) {
            return cachedShipGameObject;
        }

        var shipGameObject = GameObject.Find("/Environment/HangarShip");
        if (shipGameObject == null) {
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
        if (startOfRound == null) {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return null;
        }

        var playerSpawnPositions = startOfRound.playerSpawnPositions;
        if (playerSpawnPositions == null) {
            // Invalid state
            Logger.LogError("StartOfRound.Instance.playerSpawnPositions is null.");
            return null;
        }

        var playerSpawnPositionTransform = playerSpawnPositions.ElementAtOrDefault(1);
        if (playerSpawnPositionTransform == null) {
            // Invalid state
            Logger.LogError("Player spawn position is null for ID 1.");
            return null;
        }

        return playerSpawnPositionTransform.position;
    }

    public static bool SpawnItemInShip(Item item)
    {
        var startOfRound = StartOfRound.Instance;
        if (startOfRound == null) {
            // Invalid state
            Logger.LogError("StartOfRound.Instance is null.");
            return false;
        }

        var spawnPrefab = item.spawnPrefab;
        if (spawnPrefab == null) {
            // Invalid state
            Logger.LogError("Item.spawnPrefab is null.");
            return false;
        }

        var shipGameObject = GetShipGameObject();
        if (shipGameObject == null) {
            // Invalid state
            Logger.LogError("Failed to get Ship game object.");
            return false;
        }

        var shipTransform = shipGameObject.transform;
        if (shipTransform == null) {
            // Invalid state
            Logger.LogError("Failed to get Ship transform.");
            return false;
        }

        var baseSpawnPositionNullable = GetBaseSpawnPosition();
        if (baseSpawnPositionNullable == null) {
            // Invalid state
            Logger.LogError("Failed to get spawn position.");
            return false;
        }
        var baseSpawnPosition = baseSpawnPositionNullable.Value;

        // Default position for out of bounds items in the base game
        const float offsetXRange = 0.7f;

        float offsetX = Random.Range(-offsetXRange, offsetXRange);
        const float offsetZ = 2.0f;
        const float offsetY = 0.5f;

        var spawnPosition = baseSpawnPosition + new Vector3(offsetX, offsetY, offsetZ);

        var gameObject = Object.Instantiate(
            spawnPrefab,
            spawnPosition,
            Quaternion.identity,
            startOfRound.propsContainer
        );

        var grabbableObject = gameObject.GetComponent<GrabbableObject>();
        if (grabbableObject == null) {
            // Invalid state
            Logger.LogError("Failed to get GrabbableObject component from spawned item.");
            Object.Destroy(gameObject);
            return false;
        }

        grabbableObject.fallTime = 0f;
        grabbableObject.hasHitGround = true; // Disable drop sound effect
        grabbableObject.isInShipRoom = true;
        grabbableObject.transform.parent = shipTransform;

        var networkObject = gameObject.GetComponent<NetworkObject>();
        if (networkObject == null) {
            // Invalid state
            Logger.LogError("Failed to get NetworkObject component from spawned item.");
            Object.Destroy(gameObject);
            return false;
        }

        networkObject.Spawn(false);

        return true;
    }
}
