using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Necesario para FirstOrDefault, etc.

public enum RoomType { Start, Normal, Boss, Item, Empty } 

public class MapGenerator : MonoBehaviour
{
    [Header("Map Dimensions")]
    public float roomWidth = 15;
    public float roomHeight = 9;
    public int targetNumberOfRooms = 15; 


    [Header("Room Prefabs")]
    public GameObject startRoomPrefab;
    public GameObject normalRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject itemRoomPrefab;
    public GameObject bossEnemyPrefab;

    [Header("Generation Settings")]
    [Range(0, 1)]
    public float itemRoomChance = 0.1f; 

    // --- Datos Internos ---
    private Dictionary<Vector2Int, RoomType> roomGrid; 
    private List<Vector2Int> roomCoordinates;         
    private Dictionary<Vector2Int, GameObject> instantiatedRooms; 

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // Limpiar mapa anterior si existe
        if (instantiatedRooms != null)
        {
            foreach (var roomInstance in instantiatedRooms.Values)
            {
                Destroy(roomInstance);
            }
        }

        roomGrid = new Dictionary<Vector2Int, RoomType>();
        roomCoordinates = new List<Vector2Int>();
        instantiatedRooms = new Dictionary<Vector2Int, GameObject>();

        // 1. Algoritmo de Generación (Random Walk)
        CreateRoomLayout();

        // 2. Asignar Tipos Especiales (Boss, Item)
        AssignSpecialRooms();

        // 3. Instanciar las Salas
        InstantiateRooms();

        // 4. Configurar Puertas
        SetupDoors();

        
        PlacePlayerInStartRoom();
    }

    void CreateRoomLayout()
    {
        Vector2Int currentPos = Vector2Int.zero; 
        roomGrid[currentPos] = RoomType.Start;   
        roomCoordinates.Add(currentPos);

        int roomsCreated = 1;
        int maxAttempts = targetNumberOfRooms * 5; 
        int attempts = 0;

        while (roomsCreated < targetNumberOfRooms && attempts < maxAttempts)
        {
            attempts++;
            Vector2Int potentialStartPos = roomCoordinates[Random.Range(0, roomCoordinates.Count)];
            Vector2Int direction = GetRandomDirection();
            Vector2Int nextPos = potentialStartPos + direction;

            // Validar nueva posición
            if (IsInGridBounds(nextPos) && !roomGrid.ContainsKey(nextPos))
            {
               
                int neighborCount = CountNeighbors(nextPos);
                if (neighborCount <= 1)
                {
                    roomGrid[nextPos] = RoomType.Normal; 
                    roomCoordinates.Add(nextPos);
                    roomsCreated++;
                }
            }
        }

        if (roomsCreated < targetNumberOfRooms)
        {
            Debug.LogWarning($"MapGenerator: Only created {roomsCreated} rooms out of the target {targetNumberOfRooms}. Consider adjusting parameters.");
        }
    }

    Vector2Int GetRandomDirection()
    {
        int choice = Random.Range(0, 4);
        switch (choice)
        {
            case 0: return Vector2Int.up;    
            case 1: return Vector2Int.down;  
            case 2: return Vector2Int.left;  
            case 3: return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }

    bool IsInGridBounds(Vector2Int pos)
    {
        return true; 
    }

    int CountNeighbors(Vector2Int pos)
    {
        int count = 0;
        if (roomGrid.ContainsKey(pos + Vector2Int.up)) count++;
        if (roomGrid.ContainsKey(pos + Vector2Int.down)) count++;
        if (roomGrid.ContainsKey(pos + Vector2Int.left)) count++;
        if (roomGrid.ContainsKey(pos + Vector2Int.right)) count++;
        return count;
    }


    void AssignSpecialRooms()
    {
        if (roomCoordinates.Count <= 1) return; 

        
        Vector2Int startPos = Vector2Int.zero;
        Vector2Int bossRoomPos = roomCoordinates
            .Where(pos => roomGrid[pos] == RoomType.Normal) // Solo entre las normales
            .OrderByDescending(pos => Vector2Int.Distance(startPos, pos))
            .FirstOrDefault(); 

        if (bossRoomPos != default || roomCoordinates.Count > 1)
        {
            if (bossRoomPos == default && roomCoordinates.Count > 1) 
            {
                bossRoomPos = roomCoordinates.First(p => p != startPos);
            }
            if (bossRoomPos != startPos) 
            {
                roomGrid[bossRoomPos] = RoomType.Boss;
            }
            else if (roomCoordinates.Count > 1) 
            {
                bossRoomPos = roomCoordinates.First(p => p != startPos);
                roomGrid[bossRoomPos] = RoomType.Boss;
            }
        }


        // 2. Salas de Ítem (Opcional)
        int itemRoomsToCreate = Mathf.FloorToInt(roomCoordinates.Count * itemRoomChance);
        List<Vector2Int> potentialItemRooms = roomCoordinates
            .Where(pos => roomGrid[pos] == RoomType.Normal) 
            .ToList();

        int itemRoomsCreated = 0;
        while (itemRoomsCreated < itemRoomsToCreate && potentialItemRooms.Count > 0)
        {
            int randomIndex = Random.Range(0, potentialItemRooms.Count);
            Vector2Int itemRoomPos = potentialItemRooms[randomIndex];
            roomGrid[itemRoomPos] = RoomType.Item;
            potentialItemRooms.RemoveAt(randomIndex); 
            itemRoomsCreated++;
        }
    }

    void InstantiateRooms()
    {
        foreach (var kvp in roomGrid)
        {
            Vector2Int gridPos = kvp.Key;
            RoomType type = kvp.Value;
            GameObject prefabToInstantiate = GetPrefabForType(type);

            if (prefabToInstantiate != null)
            {
                
                Vector3 worldPos = new Vector3(gridPos.x * roomWidth, gridPos.y * roomHeight, 0);
                GameObject roomInstance = Instantiate(prefabToInstantiate, worldPos, Quaternion.identity, this.transform);
                roomInstance.name = $"Room_{gridPos.x}_{gridPos.y} ({type})";

                instantiatedRooms[gridPos] = roomInstance;

                RoomController roomController = roomInstance.GetComponent<RoomController>();
                
            }
        }
    }

    GameObject GetPrefabForType(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start: return startRoomPrefab ? startRoomPrefab : normalRoomPrefab; 
            case RoomType.Normal: return normalRoomPrefab;
            case RoomType.Boss: return bossRoomPrefab ? bossRoomPrefab : normalRoomPrefab; 
            case RoomType.Item: return itemRoomPrefab ? itemRoomPrefab : normalRoomPrefab; 
            default:
                Debug.LogWarning($"Tipo de sala desconocido o no asignado: {type}");
                return null;
        }
    }

    void SetupDoors()
    {
        foreach (var kvp in instantiatedRooms)
        {
            Vector2Int gridPos = kvp.Key;
            GameObject roomInstance = kvp.Value;
            RoomController roomController = roomInstance.GetComponent<RoomController>();

            roomController.SetDoorState(DoorDirection.North, roomGrid.ContainsKey(gridPos + Vector2Int.up));
            roomController.SetDoorState(DoorDirection.South, roomGrid.ContainsKey(gridPos + Vector2Int.down));
            roomController.SetDoorState(DoorDirection.East, roomGrid.ContainsKey(gridPos + Vector2Int.right));
            roomController.SetDoorState(DoorDirection.West, roomGrid.ContainsKey(gridPos + Vector2Int.left));
        }
    }

    void PlacePlayerInStartRoom()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2Int startGridPos = Vector2Int.zero;
            if (instantiatedRooms.ContainsKey(startGridPos))
            {
                
                Vector3 startWorldPos = new Vector3(startGridPos.x * roomWidth, startGridPos.y * roomHeight, 0);
                player.transform.position = new Vector3(startWorldPos.x, startWorldPos.y, player.transform.position.z);

                
                CameraController camController = FindObjectOfType<CameraController>();
                if (camController != null)
                {
                    camController.SnapToPosition(startWorldPos);
                }
            }
        }
    }
    
    // Para que RoomController pueda obtener info de salas vecinas si es necesario
    public GameObject GetRoomInstanceAt(Vector2Int gridPos)
    {
        instantiatedRooms.TryGetValue(gridPos, out GameObject roomInstance);
        return roomInstance;
    }
}

// Enum auxiliar para direcciones de puertas
public enum DoorDirection { North, South, East, West }