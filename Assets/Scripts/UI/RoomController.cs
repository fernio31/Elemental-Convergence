using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    [Header("Configuración de Puertas")]
    public GameObject doorNorth;
    public GameObject doorSouth;
    public GameObject doorEast;
    public GameObject doorWest;

    [Header("Spawning de Enemigos")]
    public GameObject[] enemyPrefabs;
    public Transform[] enemySpawnPoints;
    public bool spawnEnemiesOnEnter = true;
    public int minEnemies = 1;
    public int maxEnemies = 3;

    [Header("Loot")]
    public GameObject[] lootPrefabs;
    public Transform lootSpawnPoint;
    public bool spawnLootOnClear = true;

    [Header("Configuración Específica de Sala de Jefe")]
    public Transform[] bossTeleportPoints;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool roomCleared = false;
    private bool playerHasEntered = false;

    void Awake()
    {
        if (doorNorth != null) doorNorth.SetActive(false);
        if (doorSouth != null) doorSouth.SetActive(false);
        if (doorEast != null) doorEast.SetActive(false);
        if (doorWest != null) doorWest.SetActive(false);

        if (!spawnEnemiesOnEnter)
        {
            roomCleared = true;
        }
    }

    // --- NUEVO MÉTODO AÑADIDO ---
    void Start()
    {
        // Si la sala se marcó como "limpia" durante Awake (porque no tiene enemigos)...
        if (roomCleared)
        {
            // ...nos aseguramos de que las puertas estén explícitamente desbloqueadas desde el inicio.
            UnlockDoors();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerHasEntered)
        {
            playerHasEntered = true;

            if (spawnEnemiesOnEnter && !roomCleared)
            {
                Debug.Log($"Jugador ha entrado en la sala {gameObject.name}. Spawneando enemigos.");
                SpawnEnemies();
                LockDoors();
            }
        }
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs.Length == 0 || enemySpawnPoints.Length == 0)
        {
            OnRoomCleared();
            return;
        }

        activeEnemies.Clear();
        int numEnemiesToSpawn = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < numEnemiesToSpawn; i++)
        {
            GameObject enemyPrefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Transform spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
            GameObject enemyInstance = Instantiate(enemyPrefabToSpawn, spawnPoint.position, spawnPoint.rotation, transform);
            activeEnemies.Add(enemyInstance);

            BossAI_VoidWeaver bossAI = enemyInstance.GetComponent<BossAI_VoidWeaver>();
            if (bossAI != null)
            {
                if (bossTeleportPoints != null && bossTeleportPoints.Length > 0)
                {
                    bossAI.teleportPoints = this.bossTeleportPoints;
                }
                else
                {
                    Debug.LogError($"¡La sala del jefe ({gameObject.name}) no tiene puntos de teletransporte asignados!", this);
                }
            }

            EnemyHealth enemyHealth = enemyInstance.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.SetRoomController(this);
            }
            else
            {
                BossHealth bossHealth = enemyInstance.GetComponent<BossHealth>();
                if (bossHealth != null)
                {
                    bossHealth.SetRoomController(this);
                }
                else
                {
                    Debug.LogError($"El prefab '{enemyPrefabToSpawn.name}' no tiene EnemyHealth NI BossHealth.", enemyInstance);
                }
            }
        }
    }

    public void EnemyDefeated(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        if (activeEnemies.Count == 0 && !roomCleared)
        {
            OnRoomCleared();
        }
    }

    void OnRoomCleared()
    {
        Debug.Log($"¡Sala {gameObject.name} completada!");
        roomCleared = true;
        UnlockDoors();

        if (spawnLootOnClear)
        {
            SpawnLoot();
        }
    }

    void SpawnLoot()
    {
        if (lootPrefabs.Length > 0 && lootSpawnPoint != null)
        {
            GameObject lootToSpawn = lootPrefabs[Random.Range(0, lootPrefabs.Length)];
            Instantiate(lootToSpawn, lootSpawnPoint.position, Quaternion.identity, transform);
        }
    }

    public void SetDoorState(DoorDirection direction, bool neighborExists)
    {
        GameObject doorObject = GetDoorObject(direction);
        if (doorObject != null)
        {
            doorObject.SetActive(neighborExists);
        }
    }

    public void LockDoors()
    {
        UpdateAllDoors(true);
        Debug.Log("Puertas bloqueadas.");
    }

    public void UnlockDoors()
    {
        UpdateAllDoors(false);
        Debug.Log("Puertas desbloqueadas.");
    }

    private void UpdateAllDoors(bool locked)
    {
        UpdateSingleDoor(doorNorth, locked);
        UpdateSingleDoor(doorSouth, locked);
        UpdateSingleDoor(doorEast, locked);
        UpdateSingleDoor(doorWest, locked);
    }

    private void UpdateSingleDoor(GameObject doorObject, bool locked)
    {
        if (doorObject != null && doorObject.activeSelf)
        {
            Collider2D blockerCollider = null;
            foreach (Collider2D col in doorObject.GetComponents<Collider2D>())
            {
                if (!col.isTrigger)
                {
                    blockerCollider = col;
                    break;
                }
            }

            if (blockerCollider != null)
            {
                blockerCollider.enabled = locked;
            }
        }
    }

    private GameObject GetDoorObject(DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.North: return doorNorth;
            case DoorDirection.South: return doorSouth;
            case DoorDirection.East:  return doorEast;
            case DoorDirection.West:  return doorWest;
            default:                  return null;
        }
    }
}