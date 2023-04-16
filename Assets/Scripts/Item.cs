using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    //// Item ID ////

    /// Blocks
    // Null - 0
    // Stone - 1
    // Dirt - 2
    // Grass - 3
    // Coal ore - 4
    // Copper ore - 5
    // Iron ore - 6
    // Silver ore - 7
    // Gold ore - 8
    // Magnesium ore - 9
    // Diamond ore - 10
    // Uran ore - 11
    // Enderitum - 12

    /// Tools
    // Wood pickaxe - 101
    // Copper pickaxe - 102

    /// Furnitures
    // WorkBench - 1001

    // For button slots item image
    static public Dictionary<int, string> itemsSpriteName = new Dictionary<int, string>()
    {
        { 0, "null" },
        { 1, "Stone1" },
        { 2, "Dirt1" },
        { 3, "Grass1" },
        { 4, "CoalOre" },
        { 5, "CopperOre"},
        { 6, "IronOre" },
        { 7, "SilverOre" },
        { 8, "GoldOre" },
        { 9, "MagnesiumOre" },
        { 10, "DiamondOre" },
        { 11, "UranOre" },
        { 12, "Enderitum" },
        { 13, "Wood"} ,
        { 14, "Plank"} ,

        { 100, "Tools/Stick"},
        { 101, "Tools/WoodPickaxe" },
        { 102, "Tools/CopperPickaxe" },
        { 103, "Tools/WoodAxe" },
        { 104, "Tools/CopperAxe" },
        { 105, "Tools/WoodSword" },
        { 106, "Tools/CopperSword" },
        { 107, "Tools/IronPickaxe" },
        { 108, "Tools/IronAxe" },
        { 109, "Tools/IronSword" },
        { 110, "Tools/SilverPickaxe" },
        { 111, "Tools/SilverAxe" },
        { 112, "Tools/SilverSword" },
        { 113, "Tools/DiamondPickaxe" },
        { 114, "Tools/DiamondAxe" },
        { 115, "Tools/DiamondSword" },
        { 116, "Tools/Torch"},

        { 1001, "Furnitures/WorkBench"},
        { 1002, "Furnitures/Furnace"},
        { 2001, "OreIngots/CopperIngot"},
        { 2002, "OreIngots/IronIngot"},
        { 2003, "OreIngots/SilverIngot"},
        { 2004, "OreIngots/GoldIngot"},
        { 2005, "OreIngots/MagnesiumIngot"},
        { 2006, "OreIngots/Diamond"},
        { 2007, "OreIngots/UranIngot"}
    };

    // For item drop and tile manager ( place item from eq )
    static public Dictionary<string, int> itemId = new Dictionary<string, int>()
    {
        { "null", 0 },
        { "Stone", 1 },
        { "Dirt", 2 },
        { "Grass", 3 },
        { "CoalOre", 4 },
        { "CopperOre", 5},
        { "IronOre", 6 },
        { "SilverOre", 7 },
        { "GoldOre", 8 },
        { "MagnesiumOre", 9 },
        { "DiamondOre", 10 },
        { "UranOre", 11 },
        { "Enderitum", 12},
        { "Wood", 13} ,
        { "Plank", 14} ,

        { "Stick", 100},
        { "WoodPickaxe", 101 },
        { "CopperPickaxe", 102 },
        { "WoodAxe", 103 },
        { "CopperAxe", 104 },
        { "WoodSword", 105 },
        { "CopperSword", 106 },
        { "IronPickaxe", 107 },
        { "IronAxe", 108 },
        { "IronSword", 109 },
        { "SilverPickaxe", 110 },
        { "SilverAxe", 111 },
        { "SilverSword", 112 },
        { "DiamondPickaxe", 113 },
        { "DiamondAxe", 114 },
        { "DiamondSword", 115 },
        { "Torch", 116 },

        { "WorkBench", 1001 },
        { "Furnace", 1002 },

        { "CopperIngot", 2001 },
        { "IronIngot", 2002 },
        { "SilverIngot", 2003 },
        { "GoldIngot", 2004 },
        { "MagnesiumIngot", 2005 },
        { "Diamond", 2006 },
        { "UranIngot", 2007 }
    };

    // For eq manager, ItemRecipe ( every number is item id ) : [CraftedItemId,CraftetItemAmount]
    static public Dictionary<string, int[]> itemRecipe = new Dictionary<string, int[]>()
    {
        { "0,0,0,13,13,0,13,13,0", new int[]{14,8}}, //  8 x Plank
        { "0,0,0,14,0,0,14,0,0", new int[]{100,4}}, // 4 x Stick

        { "14,14,14,0,100,0,0,100,0", new int[]{101,1}}, // 1 x WoodPickaxe
        { "0,14,14,0,100,14,0,100,0", new int[]{103,1}}, // 1 x WoodAxe
        { "0,14,0,14,14,14,0,100,0", new int[]{105,1}}, // 1 x WoodSword

        { "2001,2001,2001,0,100,0,0,100,0", new int[]{102,1}}, // 1 x CopperPickaxe
        { "0,2001,2001,0,100,2001,0,100,0", new int[]{104,1}}, // 1 x CopperAxe
        { "0,2001,0,2001,2001,2001,0,100,0", new int[]{106,1}}, // 1 x CopperSword

        { "2002,2002,2002,0,100,0,0,100,0", new int[]{107,1}}, // 1 x IronPickaxe
        { "0,2002,2002,0,100,2002,0,100,0", new int[]{108,1}}, // 1 x IronAxe
        { "0,2002,0,2002,2002,2002,0,100,0", new int[]{109,1}}, // 1 x IronSword

        { "2003,2003,2003,0,100,0,0,100,0", new int[]{110,1}}, // 1 x SilverPickaxe
        { "0,2003,2003,0,100,2003,0,100,0", new int[]{111,1}}, // 1 x SilverAxe
        { "0,2003,0,2003,2003,2003,0,100,0", new int[]{112,1}}, // 1 x SilverSword

        { "2006,2006,2006,0,100,0,0,100,0", new int[]{113,1}}, // 1 x DiamondPickaxe
        { "0,2006,2006,0,100,2006,0,100,0", new int[]{114,1}}, // 1 x DiamondAxe
        { "0,2006,0,2006,2004,2006,0,100,0", new int[]{115,1}}, // 1 x DiamondSword

        { "0,4,0,0,4,0,0,100,0", new int[]{116,16}}, // 16 x Torch

        { "0,0,0,14,14,0,14,14,0", new int[]{1001,1 }}, // 1 x WorkBench
        { "1,1,1,1,4,1,1,1,1", new int[]{1002,1 }}, // 1 x Furnace
    };

    // For eq manager, FurnaceRecipe ( every number is item id ) : [GainedItemId,FuelAmount,FuelItemId]
    static public Dictionary<string, int[]> furnaceRecipe = new Dictionary<string, int[]>()
    {
        {"5,4", new int[]{2001, 1, 4}}, // Copper ingot use 1 CoalOre
        {"6,4", new int[]{2002, 2, 4}}, // Iron ingot use 2 CoalOre
        {"7,4", new int[]{2003, 2, 4}}, // Silver ingot use 2 CoalOre
        {"8,4", new int[]{2004, 3, 4}}, // Gold ingot use 3 CoalOre
        {"9,4", new int[]{2005, 3, 4}}, // Magnesium ingot use 3 CoalOre
        {"10,4", new int[]{2006, 4, 4}}, // Diamond use 4 CoalOre
        {"11,4", new int[]{2007, 5, 4}}, // Uran ingot use 5 CoalOre

        {"5,13", new int[]{2001, 4, 13}}, // Copper ingot use 4 Wood
        {"6,13", new int[]{2002, 8, 13}}, // Iron ingot use 8 Wood
        {"7,13", new int[]{2003, 8, 13}}, // Silver ingot use 8 Wood 
        {"8,13", new int[]{2004, 12, 13}}, // Gold ingot use 12 Wood 
        {"9,13", new int[]{2005, 12, 13}}, // Magnesium ingot use 12 Wood
        {"10,13", new int[]{2006, 16, 13}}, // Diamond use 16 Wood 
        {"11,13", new int[]{2007, 20, 13}}, // Uran ingot use 20 Wood

        {"5,14", new int[]{2001, 8, 14}}, // Copper ingot use 8 Plank
        {"6,14", new int[]{2002, 16, 14}}, // Iron ingot use 16 Plank
        {"7,14", new int[]{2003, 16, 14}}, // Silver ingot use 16 Plank 
        {"8,14", new int[]{2004, 24, 14}}, // Gold ingot use 24 Plank
        {"9,14", new int[]{2005, 24, 14}}, // Magnesium ingot use 24 Plank
        {"10,14", new int[]{2006, 32, 14}}, // Diamond use 32 Plank 
        {"11,14", new int[]{2007, 40, 14}}, // Uran ingot use 40 Plank
    };

    // For eq manager, ItemId : MaxStack
    static public Dictionary<int, int> itemsMaxStack = new Dictionary<int, int>()
    {
        { 0, 0 },
        { 1, 512 },
        { 2, 512 },
        { 3, 5 },
        { 4, 128 },
        { 5, 128 },
        { 6, 128 },
        { 7, 128 },
        { 8, 128 },
        { 9, 128 },
        { 10, 128 },
        { 11, 128 },
        { 12, 128 },
        { 13, 128 },
        { 14, 128 },
        { 100, 16},
        { 101, 1 },
        { 102, 1 },
        { 103, 1 },
        { 104, 1 },
        { 105, 1 },
        { 106, 1 },
        { 107, 1 },
        { 108, 1 },
        { 109, 1 },
        { 110, 1 },
        { 111, 1 },
        { 112, 1 },
        { 113, 1 },
        { 114, 1 },
        { 115, 1 },
        { 116, 64},
        { 1001, 1},
        { 1002, 1},
        { 2001, 64},
        { 2002, 64},
        { 2003, 64},
        { 2004, 64},
        { 2005, 64},
        { 2006, 64},
        { 2007, 64}
    };

    static public Dictionary<int, int> pickaxeSpeed = new Dictionary<int, int>()
    {
        { 101, 1 },
        { 102, 2 },
        { 107, 3 },
        { 110, 4},
        { 113, 5},
    };

    static public Dictionary<int, int> pickaxeTier = new Dictionary<int, int>()
    {
        { 101, 1 },
        { 102, 2 },
        { 107, 3 },
        { 110, 4},
        { 113, 5},
    };

    static public Dictionary<int, int> axeSpeed = new Dictionary<int, int>()
    {
        { 103, 1 },
        { 104, 2 },
        { 108, 5},
        { 111, 10},
        { 114, 15},
    };

    static public Dictionary<int, int> swordDamage = new Dictionary<int, int>()
    {
        { 105, 5 },
        { 106, 10 },
        { 109, 15},
        { 112, 20},
        { 115, 35}
    };

    static public Dictionary<int, float> swordKnockback = new Dictionary<int, float>()
    {
        { 105, 2 },
        { 106, 2.5f },
        { 109, 2.8f},
        { 112, 3f},
        { 115, 3.3f}
    };

    static public Dictionary<int, bool> canPlaceItem = new Dictionary<int, bool>()
    {
        { 0, false },
        { 1, true },
        { 2, true },
        { 3, true },
        { 4, true },
        { 5, true },
        { 6, true },
        { 7, true },
        { 8, true },
        { 9, true },
        { 10, true },
        { 11, true },
        { 12, false },
        { 13, true },
        { 14, true },
        { 100, false},
        { 101, false },
        { 102, false },
        { 103, false },
        { 104, false },
        { 105, false },
        { 106, false },
        { 107, false },
        { 108, false },
        { 109, false },
        { 110, false },
        { 111, false },
        { 112, false },
        { 113, false },
        { 114, false },
        { 115, false },
        { 116, true},
        { 1001, true},
        { 1002, true},
        { 2001, false },
        { 2002, false },
        { 2003, false },
        { 2004, false },
        { 2005, false },
        { 2006, false },
        { 2007, false }
    };

    // If you can use item to destroy tile
    static public List<int> canDestroyTile = new List<int>()
    {
        101,
        102,
        107,
        110,
        113,
    };

    // If you can use item to chop tree
    static public List<int> canChoopTree = new List<int>()
    {
        103,
        104,
        108,
        111,
        114,
    };

    // For player manager, to change player hand item
    static public List<int> isTool = new List<int>()
    {
        101,
        102,
        103,
        104,
        105,
        106,
        107,
        108,
        109,
        110,
        111,
        112,
        113,
        114,
        115,
    };
}
