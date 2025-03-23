using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct TerrainLayer
    {
        public Tilemap tilemap; // The Tilemap for this terrain type
        public TileBase[] tiles; // Array of tiles to use for this terrain type (e.g., multiple grass tiles)
        [Range(0f, 1f)] public float density; // Probability of this terrain type appearing
    }

    [Header("Tilemap Settings")]
    public TerrainLayer[] groundLayers; // Array of ground Tilemaps (e.g., grass, water, sand)
    public TerrainLayer obstacleLayer; // Layer for obstacles (trees, mountains)

    [Header("Map Dimensions")]
    public int mapWidth = 50; // Width of the map in tiles
    public int mapHeight = 50; // Height of the map in tiles

    [Header("Terrain Settings")]
    [Range(0f, 1f)] public float obstacleDensity = 0.1f; // Percentage of tiles that will have obstacles (trees, mountains)

    [Header("Edge Settings")]
    public int edgeBuffer = 5; // Number of tiles from the edge to force a specific terrain (e.g., water)

    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs; // Array of enemy prefabs to spawn
    public int minEnemies = 5; // Minimum number of enemies to spawn
    public int maxEnemies = 10; // Maximum number of enemies to spawn

    [Header("Item Settings")]
    public GameObject[] itemPrefabs; // Array of item prefabs to spawn (e.g., Torch_Yellow)
    public int minItems = 5; // Minimum number of items to spawn
    public int maxItems = 10; // Maximum number of items to spawn

    [Header("Player Settings")]
    public Transform player; // Reference to the player to avoid spawning objects on top of them

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // Clear all existing Tilemaps
        foreach (var layer in groundLayers)
        {
            layer.tilemap.ClearAllTiles();
        }
        obstacleLayer.tilemap.ClearAllTiles();

        // Step 1: Generate the ground with multiple terrain types
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // Check if this position is near the edge
                bool isEdge = x < edgeBuffer || x >= mapWidth - edgeBuffer || y < edgeBuffer || y >= mapHeight - edgeBuffer;

                // Use Perlin noise for natural terrain distribution
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);

                // Default to the first layer (e.g., grass) if no other conditions are met
                TerrainLayer selectedLayer = groundLayers[0];

                // If near the edge, force a specific terrain (e.g., water)
                if (isEdge && groundLayers.Length > 1)
                {
                    selectedLayer = groundLayers[1]; // Use the second layer (e.g., water) for edges
                }
                else
                {
                    // Use noise to decide terrain type
                    float totalDensity = 0f;
                    foreach (var layer in groundLayers)
                    {
                        totalDensity += layer.density;
                    }

                    float randomValue = Random.value * totalDensity;
                    float cumulativeDensity = 0f;

                    for (int i = 0; i < groundLayers.Length; i++)
                    {
                        cumulativeDensity += groundLayers[i].density;
                        if (randomValue <= cumulativeDensity)
                        {
                            selectedLayer = groundLayers[i];
                            break;
                        }
                    }
                }

                // Randomly select a tile from the selected layer's tiles array
                if (selectedLayer.tiles.Length > 0)
                {
                    TileBase selectedTile = selectedLayer.tiles[Random.Range(0, selectedLayer.tiles.Length)];
                    selectedLayer.tilemap.SetTile(new Vector3Int(x, y, 0), selectedTile);
                }
            }
        }

        // Step 2: Place obstacles (trees, mountains, etc.)
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // Skip the player's starting position
                Vector3 worldPos = groundLayers[0].tilemap.CellToWorld(new Vector3Int(x, y, 0));
                if (Vector2.Distance(worldPos, player.position) < 2f) continue;

                // Randomly place obstacles
                if (Random.value < obstacleDensity && obstacleLayer.tiles.Length > 0)
                {
                    TileBase selectedObstacle = obstacleLayer.tiles[Random.Range(0, obstacleLayer.tiles.Length)];
                    obstacleLayer.tilemap.SetTile(new Vector3Int(x, y, 0), selectedObstacle);
                }
            }
        }

        // Step 3: Spawn enemies
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 spawnPos = GetRandomSpawnPosition();
            if (spawnPos != Vector2.zero) // Ensure a valid position was found
            {
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }
        }

        // Step 4: Spawn items
        int itemCount = Random.Range(minItems, maxItems + 1);
        for (int i = 0; i < itemCount; i++)
        {
            Vector2 spawnPos = GetRandomSpawnPosition();
            if (spawnPos != Vector2.zero) // Ensure a valid position was found
            {
                GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
                Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            // Generate a random position within the map bounds
            int x = Random.Range(0, mapWidth);
            int y = Random.Range(0, mapHeight);
            Vector3Int tilePos = new Vector3Int(x, y, 0);
            Vector3 worldPos = groundLayers[0].tilemap.CellToWorld(tilePos);

            // Check if the position is free (no obstacles and not too close to the player)
            if (obstacleLayer.tilemap.GetTile(tilePos) == null && Vector2.Distance(worldPos, player.position) > 2f)
            {
                // Ensure the position is not on water (or other non-walkable terrain)
                bool isWalkable = true;
                foreach (var layer in groundLayers)
                {
                    if (layer.tilemap.GetTile(tilePos) != null && layer.tilemap.gameObject.CompareTag("NonWalkable"))
                    {
                        isWalkable = false;
                        break;
                    }
                }

                if (isWalkable)
                {
                    return worldPos;
                }
            }
        }

        Debug.LogWarning("Could not find a valid spawn position after " + maxAttempts + " attempts.");
        return Vector2.zero; // Return an invalid position if no valid spot is found
    }
}