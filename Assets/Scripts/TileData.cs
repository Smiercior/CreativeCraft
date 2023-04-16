using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

//// Tile data class ////
[Serializable]
public class TileData
{
    public int x;
    public int y;
    public int tileMaterialId;

    [NonSerialized]
    public static Tile Grass, Dirt, Stone, CoalOre, CopperOre, IronOre, SilverOre, GoldOre, MagnesiumOre, DiamondOre, UranOre, Enderium;
    [NonSerialized]
    public static Tile Wood, Plank;
    [NonSerialized]
    public static Tile WorkBench;

    //// Tile ID ( Same as Item id for tiles ) ////
    /// Blocks
    // Empty - 0
    // Stone1 - 1
    // Dirt1 - 2
    // Grass1 - 3
    // Coal ore - 4
    // Copper ore - 5
    // Iron ore - 6
    // Silver ore - 7
    // GoldOre - 8
    // Magnesium ore - 9
    // Diamond ore - 10
    // Uran ore - 11
    // Enderium - 12

    /// Furnitures
    // WorkBench - 1001

    public static Tile GetTileByTileMaterialId(int tileMaterialId)
    {
        // Block tiles
        if (tileMaterialId == 1) return Resources.Load<Tile>("Tiles/Stone");
        else if (tileMaterialId == 2) return Resources.Load<Tile>("Tiles/Dirt");
        else if (tileMaterialId == 3) return Resources.Load<Tile>("Tiles/Grass");
        else if (tileMaterialId == 4) return Resources.Load<Tile>("Tiles/CoalOre");
        else if (tileMaterialId == 5) return Resources.Load<Tile>("Tiles/CopperOre");
        else if (tileMaterialId == 6) return Resources.Load<Tile>("Tiles/IronOre");
        else if (tileMaterialId == 7) return Resources.Load<Tile>("Tiles/SilverOre");
        else if (tileMaterialId == 8) return Resources.Load<Tile>("Tiles/GoldOre");
        else if (tileMaterialId == 9) return Resources.Load<Tile>("Tiles/MagnesiumOre");
        else if (tileMaterialId == 10) return Resources.Load<Tile>("Tiles/DiamondOre");
        else if (tileMaterialId == 11) return Resources.Load<Tile>("Tiles/UranOre");
        else if (tileMaterialId == 12) return Resources.Load<Tile>("Tiles/Enderitum");
        else if (tileMaterialId == 13) return Resources.Load<Tile>("Tiles/Wood");
        else if (tileMaterialId == 14) return Resources.Load<Tile>("Tiles/Plank");

        // Furniture tiles
        else if (tileMaterialId == 1001) return Resources.Load<Tile>("Furnitures/WorkBench");
        else if (tileMaterialId == 1002) return Resources.Load<Tile>("Furnitures/Furnace");
        else return null;
    }

    [NonSerialized]
    static public Dictionary<string, int> tilesHardness = new Dictionary<string, int>()
    {
        { "Stone", 256 },
        { "Dirt", 128 },
        { "Grass", 128 },
        { "CoalOre", 384 },
        { "CopperOre", 512 },
        { "IronOre", 640 },
        { "SilverOre", 768 },
        { "GoldOre", 1024 },
        { "MagnesiumOre", 1024 },
        { "DiamondOre", 1280 },
        { "UranOre", 1536 },
        { "Enderium", 10000 },
        { "WorkBench", 128},
        { "Furnace", 128},
        { "Wood", 128},
        { "Plank", 128},
        { "Torch", 1}
    };

    static public Dictionary<string, int> tilesDestroyPickaxeTier = new Dictionary<string, int>()
    {
        { "Stone", 1 },
        { "Dirt", 1 },
        { "Grass", 1 },
        { "CoalOre", 1 },
        { "CopperOre", 1 },
        { "IronOre", 2 },
        { "SilverOre", 3 },
        { "GoldOre", 3 },
        { "MagnesiumOre", 4 },
        { "DiamondOre", 4 },
        { "UranOre", 5 },
        { "Enderium", 10000 },
        { "WorkBench", 1 },
        { "Furnace", 1 },
        { "Wood", 1},
        { "Plank", 1},
        { "Torch", 1},
    };

    public TileData(int X, int Y, int TileMaterialId)
    {
        x = X;
        y = Y;
        tileMaterialId = TileMaterialId;
    }

    public static int GetTileHardness(Tile tile)
    {
        foreach (KeyValuePair<string, int> kvp in tilesHardness)
        {
            // Skip air tile ( null tile )
            if (tile != null)
            {
                if (tile.name.Contains(kvp.Key)) return kvp.Value;
            }

        }
        return 0;
    }
}
