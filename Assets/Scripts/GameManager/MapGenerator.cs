using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct GroundTileSet
    {
        public TileBase centerTile; // Tile cho mảnh giữa
        public TileBase topLeftCorner; // Góc trái trên
        public TileBase topRightCorner; // Góc phải trên
        public TileBase bottomLeftCorner; // Góc trái dưới
        public TileBase bottomRightCorner; // Góc phải dưới
        public TileBase topEdge; // Cạnh trên
        public TileBase bottomEdge; // Cạnh dưới
        public TileBase leftEdge; // Cạnh trái
        public TileBase rightEdge; // Cạnh phải
    }

    [System.Serializable]
    public struct TreeTileSet
    {
        public TileBase bottomLeft; // Thân dưới trái
        public TileBase bottomCenter; // Thân dưới giữa
        public TileBase bottomRight; // Thân dưới phải
        public TileBase topLeft; // Thân trên trái
        public TileBase topCenter; // Thân trên giữa
        public TileBase topRight; // Thân trên phải
        public TileBase top; // Ngọn cây
    }

    [Header("Tilemap Settings")]
    public Tilemap groundTilemap; // Tilemap cho ground (grass)
    public GroundTileSet groundTiles; // Các loại tile cho ground
    public Tilemap treeBottomTilemap; // Tilemap cho phần thân dưới của cây
    public Tilemap treeTopTilemap; // Tilemap cho phần thân trên của cây
    public TreeTileSet[] treeTileSets;

    [Header("Map Dimensions")]
    public int mapWidth = 50;
    public int mapHeight = 50;

    [Header("Terrain Settings")]
    public bool useRandomSeed = true; // Sử dụng seed ngẫu nhiên
    public int fixedSeed = 0; // Seed cố định nếu useRandomSeed = false

    private float seedOffsetX; // Offset ngẫu nhiên cho noise
    private float seedOffsetY;

    [Header("Tree Settings")]
    [Range(0.01f, 0.5f)] public float treeNoiseScale = 0.1f; // Nhiễu thô
    [Range(0f, 1f)] public float treeThreshold = 0.3f; // Giảm để tăng số lượng cây
    [Range(0.01f, 0.5f)] public float treeNoiseScaleFine = 0.2f; // Nhiễu mịn
    [Range(0f, 1f)] public float treeNoiseWeightFine = 0.2f; // Trọng số nhiễu mịn

    [Header("Monster Settings")]
    public GameObject[] monsterPrefabs; // Prefab quái vật
    [Range(0.01f, 0.5f)] public float monsterNoiseScale = 0.1f;
    [Range(0f, 1f)] public float monsterThreshold = 0.5f; // Giảm để tăng số lượng quái

    [Header("Item Settings")]
    public GameObject[] itemPrefabs; // Prefab vật phẩm
    [Range(0.01f, 0.5f)] public float itemNoiseScale = 0.1f;
    [Range(0f, 1f)] public float itemThreshold = 0.5f; // Giảm để tăng số lượng vật phẩm
    private bool[,] occupied; // Mảng theo dõi vị trí đã đặt cây, quái, vật phẩm


    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // Clear existing Tilemaps
        groundTilemap.ClearAllTiles();
        treeBottomTilemap.ClearAllTiles();
        treeTopTilemap.ClearAllTiles();

        // Khởi tạo mảng occupied
        occupied = new bool[mapWidth, mapHeight];

        // Generate random seed offset
        if (useRandomSeed)
        {
            seedOffsetX = Random.Range(-10000f, 10000f);
            seedOffsetY = Random.Range(-10000f, 10000f);
        }
        else
        {
            seedOffsetX = fixedSeed;
            seedOffsetY = fixedSeed;
        }

        // Step 1: Generate noise maps for trees, monsters, and items
        float[,] treeNoiseMap = new float[mapWidth, mapHeight];
        float[,] treeNoiseMapFine = new float[mapWidth, mapHeight];
        float[,] monsterNoiseMap = new float[mapWidth, mapHeight];
        float[,] itemNoiseMap = new float[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // Noise for trees (lớp thô)
                treeNoiseMap[x, y] = Mathf.PerlinNoise(x * treeNoiseScale + seedOffsetX + 2000f, y * treeNoiseScale + seedOffsetY + 2000f);
                // Noise for trees (lớp mịn)
                treeNoiseMapFine[x, y] = Mathf.PerlinNoise(x * treeNoiseScaleFine + seedOffsetX + 3000f, y * treeNoiseScaleFine + seedOffsetY + 3000f);
                // Noise for monsters
                monsterNoiseMap[x, y] = Mathf.PerlinNoise(x * monsterNoiseScale + seedOffsetX + 4000f, y * monsterNoiseScale + seedOffsetY + 4000f);
                // Noise for items
                itemNoiseMap[x, y] = Mathf.PerlinNoise(x * itemNoiseScale + seedOffsetX + 5000f, y * itemNoiseScale + seedOffsetY + 5000f);
            }
        }

        // Step 2: Place ground tiles (only grass)
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                TileBase selectedTile = groundTiles.centerTile;
                if (x == 0 && y == mapHeight - 1) selectedTile = groundTiles.topLeftCorner;
                else if (x == mapWidth - 1 && y == mapHeight - 1) selectedTile = groundTiles.topRightCorner;
                else if (x == 0 && y == 0) selectedTile = groundTiles.bottomLeftCorner;
                else if (x == mapWidth - 1 && y == 0) selectedTile = groundTiles.bottomRightCorner;
                else if (y == mapHeight - 1) selectedTile = groundTiles.topEdge;
                else if (y == 0) selectedTile = groundTiles.bottomEdge;
                else if (x == 0) selectedTile = groundTiles.leftEdge;
                else if (x == mapWidth - 1) selectedTile = groundTiles.rightEdge;

                groundTilemap.SetTile(new Vector3Int(x, y, 0), selectedTile);
            }
        }

        // Step 3: Place trees
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float treeNoise = treeNoiseMap[x, y] * (1f - treeNoiseWeightFine) + treeNoiseMapFine[x, y] * treeNoiseWeightFine;
                if (treeNoise < treeThreshold)
                {
                    if (x + 2 >= mapWidth || y + 2 >= mapHeight) continue;

                    bool isOccupied = false;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (occupied[x + i, y + j])
                            {
                                isOccupied = true;
                                break;
                            }
                        }
                        if (isOccupied) break;
                    }
                    if (isOccupied) continue;

                    if (treeTileSets.Length > 0)
                    {
                        TreeTileSet tree = treeTileSets[Random.Range(0, treeTileSets.Length)];
                        treeBottomTilemap.SetTile(new Vector3Int(x, y, 0), tree.bottomLeft);
                        treeBottomTilemap.SetTile(new Vector3Int(x + 1, y, 0), tree.bottomCenter);
                        treeBottomTilemap.SetTile(new Vector3Int(x + 2, y, 0), tree.bottomRight);
                        treeTopTilemap.SetTile(new Vector3Int(x, y + 1, 0), tree.topLeft);
                        treeTopTilemap.SetTile(new Vector3Int(x + 1, y + 1, 0), tree.topCenter);
                        treeTopTilemap.SetTile(new Vector3Int(x + 2, y + 1, 0), tree.topRight);
                        treeTopTilemap.SetTile(new Vector3Int(x + 1, y + 2, 0), tree.top);

                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                occupied[x + i, y + j] = true;
                            }
                        }
                    }
                }
            }
        }

        // Step 3: Place monsters (within map bounds)
        for (int x = 1; x < mapWidth - 1; x++)
        {
            for (int y = 1; y < mapHeight - 1; y++)
            {
                if (monsterNoiseMap[x, y] > monsterThreshold && !occupied[x, y])
                {
                    if (monsterPrefabs.Length > 0)
                    {
                        GameObject monsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
                        Vector3 worldPos = groundTilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, 0);
                        GameObject spawnedMonster = Instantiate(monsterPrefab, worldPos, Quaternion.identity);
                        occupied[x, y] = true;
                    }
                }
            }
        }

        // Step 4: Place items (within map bounds)
        for (int x = 1; x < mapWidth - 1; x++)
        {
            for (int y = 1; y < mapHeight - 1; y++)
            {
                if (itemNoiseMap[x, y] > itemThreshold && !occupied[x, y])
                {
                    if (itemPrefabs.Length > 0)
                    {
                        GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
                        Vector3 worldPos = groundTilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, 0);
                        GameObject spawnedItem = Instantiate(itemPrefab, worldPos, Quaternion.identity);
                        occupied[x, y] = true;

                        // Kiểm tra nếu vị trí vượt ra ngoài bản đồ
                        Vector3 tilemapMin = groundTilemap.CellToWorld(new Vector3Int(0, 0, 0));
                        Vector3 tilemapMax = groundTilemap.CellToWorld(new Vector3Int(mapWidth - 1, mapHeight - 1, 0));
                    }
                }
            }
        }
    }
}