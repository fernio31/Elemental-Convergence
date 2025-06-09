using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;

public class RoomPrefabGenerator : EditorWindow
{
    public List<Tile> floorTiles = new List<Tile>();
    public List<Tile> wallTiles = new List<Tile>();
    public List<Tile> decorTiles = new List<Tile>();

    public int roomCount = 5;
    public Vector2Int roomSize = new Vector2Int(10, 8);
    public string savePath = "Assets/Rooms/Prefabs";

    [MenuItem("Tools/Room Prefab Generator")]
    static void Init()
    {
        GetWindow<RoomPrefabGenerator>("Room Prefab Generator").Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Room Prefab Generator", EditorStyles.boldLabel);
        roomCount = EditorGUILayout.IntField("Room Count", roomCount);
        roomSize = EditorGUILayout.Vector2IntField("Room Size (tiles)", roomSize);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        EditorGUILayout.Space();
        DrawTileList("Floor Tiles", floorTiles);
        DrawTileList("Wall Tiles", wallTiles);
        DrawTileList("Decor Tiles", decorTiles);

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Room Prefabs"))
        {
            GenerateRooms();
        }
    }

    void DrawTileList(string label, List<Tile> tiles)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        int removeIndex = -1;

        for (int i = 0; i < tiles.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            tiles[i] = (Tile)EditorGUILayout.ObjectField(tiles[i], typeof(Tile), false);
            if (GUILayout.Button("X", GUILayout.Width(20)))
                removeIndex = i;
            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex >= 0)
            tiles.RemoveAt(removeIndex);

        if (GUILayout.Button($"Add {label.Replace(" Tiles", "")} Tile"))
        {
            tiles.Add(null);
        }
    }

    void GenerateRooms()
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        for (int i = 0; i < roomCount; i++)
        {
            GameObject roomGO = new GameObject($"Room_{i}");
            Transform grid = new GameObject("Grid", typeof(Grid)).transform;
            grid.parent = roomGO.transform;

            var floorGO = CreateTilemap("Floor", grid, false);
            var wallGO = CreateTilemap("Walls", grid, true);
            var decorGO = CreateTilemap("Decor", grid, false);

            var floorMap = floorGO.GetComponent<Tilemap>();
            var wallMap = wallGO.GetComponent<Tilemap>();
            var decorMap = decorGO.GetComponent<Tilemap>();

            if (floorMap != null) FillFloor(floorMap);
            if (wallMap != null) BuildWalls(wallMap);
            if (decorMap != null) AddDecor(decorMap);

            string prefabPath = $"{savePath}/Room_{i}.prefab";
            PrefabUtility.SaveAsPrefabAsset(roomGO, prefabPath);
            DestroyImmediate(roomGO);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Se generaron {roomCount} habitaciones en {savePath}");
    }

    private GameObject CreateTilemap(string name, Transform parent, bool addCollider = false)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = parent;

        var tilemap = go.AddComponent<Tilemap>();
        go.AddComponent<TilemapRenderer>();

        if (addCollider)
        {
            if (!go.GetComponent<TilemapCollider2D>())
            {
                go.AddComponent<TilemapCollider2D>();
            }

            // Agrega Rigidbody2D si no existe ya
            if (!go.GetComponent<Rigidbody2D>())
            {
                var rb = go.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
            }
        }

        return go;
    }

    void FillFloor(Tilemap map)
    {
        if (floorTiles.Count == 0) return;

        for (int x = 0; x < roomSize.x; x++)
        {
            for (int y = 0; y < roomSize.y; y++)
            {
                map.SetTile(new Vector3Int(x, y, 0), floorTiles[Random.Range(0, floorTiles.Count)]);
            }
        }
    }

    void BuildWalls(Tilemap map)
    {
        if (wallTiles.Count == 0) return;

        for (int x = 0; x < roomSize.x; x++)
        {
            map.SetTile(new Vector3Int(x, 0, 0), wallTiles[Random.Range(0, wallTiles.Count)]);
            map.SetTile(new Vector3Int(x, roomSize.y - 1, 0), wallTiles[Random.Range(0, wallTiles.Count)]);
        }

        for (int y = 1; y < roomSize.y - 1; y++)
        {
            map.SetTile(new Vector3Int(0, y, 0), wallTiles[Random.Range(0, wallTiles.Count)]);
            map.SetTile(new Vector3Int(roomSize.x - 1, y, 0), wallTiles[Random.Range(0, wallTiles.Count)]);
        }
    }

    void AddDecor(Tilemap map)
    {
        if (decorTiles.Count == 0) return;

        int count = Random.Range(1, 4);
        for (int i = 0; i < count; i++)
        {
            int x = Random.Range(1, roomSize.x - 1);
            int y = Random.Range(1, roomSize.y - 1);
            map.SetTile(new Vector3Int(x, y, 0), decorTiles[Random.Range(0, decorTiles.Count)]);
        }
    }
}
