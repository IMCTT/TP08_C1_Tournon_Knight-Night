using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// One kind of ground (grass, cave dirt, bricks, wood platform...).
// [System.Serializable] lets Unity show it in the Inspector inside a list.
[System.Serializable]
public class GroundType
{
    public string name = "Grass";
    [Tooltip("Character used for this ground in the level file, for example #")]
    public string symbol = "#";
    [Tooltip("Thin platform, 1 tile thick: only uses the 3 platform tiles")]
    public bool isPlatform = false;

    [Header("Block (9 pieces)")]
    public TileBase topLeft;
    public TileBase top;
    public TileBase topRight;
    public TileBase left;
    public TileBase center;
    public TileBase right;
    public TileBase bottomLeft;
    public TileBase bottom;
    public TileBase bottomRight;

    [Header("Platform (only if Is Platform)")]
    public TileBase platformLeft;
    public TileBase platformMiddle;
    public TileBase platformRight;
}

// Builds the level from a text file, in the Editor, with one click.
// Ground symbols come from the "Ground Types" list. Objects:
//   P player    E enemy    F enemy type 2    C coin    H heal
//   D damage    I invulnerability    T triple jump    K key    G goal    S spikes
//   . empty (sky)    , empty with cave background
// Row 1 of the file is the TOP of the level.
//
// How to use: right click on this component's title in the Inspector > "Build Level".
public class LevelBuilder : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private TextAsset levelFile;
    [SerializeField] private Tilemap groundTilemap;
    [Tooltip("Empty object where enemies and pickups are created (so they can be cleared).")]
    [SerializeField] private Transform spawnedParent;

    [Header("Cave Background")]
    [Tooltip("Tilemap without collider, behind the ground (your 'Background' tilemap).")]
    [SerializeField] private Tilemap backgroundTilemap;
    [Tooltip("Tile painted behind the cave. Tip: darken it with the Color of the Tilemap component.")]
    [SerializeField] private TileBase caveBackgroundTile;

    [Header("Ground")]
    [SerializeField] private GroundType[] groundTypes;

    [Header("Objects (leave empty to skip)")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemy2Prefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject healPrefab;
    [SerializeField] private GameObject damagePrefab;
    [SerializeField] private GameObject invulnerabilityPrefab;
    [SerializeField] private GameObject tripleJumpPrefab;
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject spikesPrefab;

    private string[] rows;
    private Dictionary<char, GroundType> groundBySymbol;

    [ContextMenu("Build Level")]
    public void BuildLevel()
    {
        ClearLevel();

        // A dictionary lets us find the ground type of a character quickly: groundBySymbol['#']
        groundBySymbol = new Dictionary<char, GroundType>();
        foreach (GroundType type in groundTypes)
        {
            if (!string.IsNullOrEmpty(type.symbol))
            {
                groundBySymbol[type.symbol[0]] = type;
            }
        }

        // Split the file in lines and drop empty ones
        rows = levelFile.text.Replace("\r", "").Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
        int height = rows.Length;

        for (int row = 0; row < height; row++)
        {
            for (int x = 0; x < rows[row].Length; x++)
            {
                char symbol = rows[row][x];
                if (symbol == '.') continue;

                // Row 0 is the top of the level, so we flip it for Unity (Y goes up)
                Vector3Int cell = new Vector3Int(x, height - 1 - row, 0);

                if (symbol == ',')
                {
                    PaintCaveBackground(cell);
                }
                else if (groundBySymbol.TryGetValue(symbol, out GroundType ground))
                {
                    groundTilemap.SetTile(cell, ChooseGroundTile(ground, x, row));

                    // Thin platforms don't cover the whole cell: paint the cave behind them
                    if (ground.isPlatform && NextToCaveAir(x, row))
                    {
                        PaintCaveBackground(cell);
                    }
                }
                else
                {
                    PlaceObject(symbol, groundTilemap.GetCellCenterWorld(cell));

                    // Objects inside the cave also need the background behind them
                    if (NextToCaveAir(x, row))
                    {
                        PaintCaveBackground(cell);
                    }
                }
            }
        }

        groundTilemap.CompressBounds();
        MarkSceneChanged();
        Debug.Log("Level built: " + rows[0].Length + " x " + height + " tiles");
    }

    [ContextMenu("Clear Level")]
    public void ClearLevel()
    {
        groundTilemap.ClearAllTiles();
        if (backgroundTilemap != null) backgroundTilemap.ClearAllTiles();

        // Destroy everything that was created before (backwards, because the list shrinks)
        if (spawnedParent != null)
        {
            for (int i = spawnedParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(spawnedParent.GetChild(i).gameObject);
            }
        }

        MarkSceneChanged();
    }

    // Picks the right piece (edge, corner, center) by looking at the 4 neighbours
    private TileBase ChooseGroundTile(GroundType ground, int x, int row)
    {
        bool leftSide = Connects(ground, x - 1, row);
        bool rightSide = Connects(ground, x + 1, row);

        if (ground.isPlatform)
        {
            if (!leftSide) return Fallback(ground.platformLeft, ground.platformMiddle);
            if (!rightSide) return Fallback(ground.platformRight, ground.platformMiddle);
            return ground.platformMiddle;
        }

        bool up = Connects(ground, x, row - 1);
        bool down = Connects(ground, x, row + 1);

        TileBase tile;
        if (!up)        // top row (grass)
        {
            tile = !leftSide ? ground.topLeft : !rightSide ? ground.topRight : ground.top;
        }
        else if (!down) // bottom row
        {
            tile = !leftSide ? ground.bottomLeft : !rightSide ? ground.bottomRight : ground.bottom;
        }
        else            // middle
        {
            tile = !leftSide ? ground.left : !rightSide ? ground.right : ground.center;
        }

        return Fallback(tile, ground.center);
    }

    // Is there ground at (x, row) that joins with this ground (no edge between them)?
    // Blocks join with other blocks (grass touching cave dirt has no edge).
    // Platforms only join with the same platform.
    private bool Connects(GroundType ground, int x, int row)
    {
        // Outside the map: below and to the sides count as ground, above doesn't
        if (row < 0) return false;
        if (row >= rows.Length || x < 0 || x >= rows[row].Length) return !ground.isPlatform;

        if (!groundBySymbol.TryGetValue(rows[row][x], out GroundType other)) return false;

        if (ground.isPlatform) return other == ground;
        return !other.isPlatform;
    }

    private void PaintCaveBackground(Vector3Int cell)
    {
        if (backgroundTilemap != null && caveBackgroundTile != null)
        {
            backgroundTilemap.SetTile(cell, caveBackgroundTile);
        }
    }

    private bool IsCaveAir(int x, int row)
    {
        return row >= 0 && row < rows.Length && x >= 0 && x < rows[row].Length && rows[row][x] == ',';
    }

    private bool NextToCaveAir(int x, int row)
    {
        return IsCaveAir(x - 1, row) || IsCaveAir(x + 1, row) || IsCaveAir(x, row - 1) || IsCaveAir(x, row + 1);
    }

    // Use the first tile, or the backup if the first one wasn't assigned
    private TileBase Fallback(TileBase tile, TileBase backup)
    {
        return tile != null ? tile : backup;
    }

    private void PlaceObject(char symbol, Vector3 position)
    {
        if (symbol == 'P')
        {
            if (player != null) player.position = position;
            return;
        }

        GameObject prefab = null;
        switch (symbol)
        {
            case 'E': prefab = enemyPrefab; break;
            case 'F': prefab = enemy2Prefab != null ? enemy2Prefab : enemyPrefab; break;
            case 'C': prefab = coinPrefab; break;
            case 'H': prefab = healPrefab; break;
            case 'D': prefab = damagePrefab; break;
            case 'I': prefab = invulnerabilityPrefab; break;
            case 'T': prefab = tripleJumpPrefab; break;
            case 'K': prefab = keyPrefab; break;
            case 'G': prefab = goalPrefab; break;
            case 'S': prefab = spikesPrefab; break;
            default:
                Debug.LogWarning("Unknown symbol '" + symbol + "' in the level file");
                break;
        }

        if (prefab == null) return; // no prefab assigned for this symbol: skip it

        GameObject obj = null;

#if UNITY_EDITOR
        // If it's a prefab from the Project, keep the link to it (blue in the Hierarchy)
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(prefab))
        {
            obj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, spawnedParent);
        }
#endif

        // Otherwise (an object from the scene), just copy it
        if (obj == null)
        {
            obj = Instantiate(prefab, spawnedParent);
            obj.name = prefab.name;
        }

        obj.transform.position = position;
    }

    private void MarkSceneChanged()
    {
#if UNITY_EDITOR
        // Without this Unity doesn't notice the change and won't ask to save the scene
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }
}
