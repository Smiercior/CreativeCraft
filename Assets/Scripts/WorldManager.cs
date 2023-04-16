using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class WorldManager : MonoBehaviour
{
    //// Word size variables ////
    public static int maxWidth = 500; // Max world width
    public static int maxHeight = 80; // Max world height 
    public static int minHeight = -20; // Min world height
    public static int stoneHeight = 50;
    public float scale = 0.045f;
    public static Vector2Int playerSpawnPoint;


    //// Variables for tiles ////
    public List<TileData> tilesData;
    public Tilemap tilemap;
    Vector2 position;
    int[,] coordinates = new int[maxWidth, stoneHeight + Mathf.Abs(minHeight)]; // + minHeight, because array can't have negative index
    // X: 0 <-> 500
    // y: -20 <-> 80
    // stoney : -20 <-> 50

    // Create new random world
    public void CreateWorld(string worldFilePath, int seed)
    {
        TileData tile;
        tilesData = new List<TileData>();

        //// Create empty tiles ////
        for (int x = 0; x <= maxWidth; x++)
        {
            for (int y = minHeight; y <= maxHeight + 1; y++)
            {
                tilesData.Add(new TileData(x, y, 0));
            }
        }

        //// Generate landform ////
        int distance, space;
        float next = 0.0f; // Variable going through perlin noise
        distance = stoneHeight; // Landform starting point

        for (int x = 0; x <= maxWidth; x++)
        {
            // Generate value that we add to hight
            distance = (int)(Mathf.PerlinNoise(next + seed, 0.0f) * 22); // Always start from same point, so if we want diffrent landform we need to add seed
            next += 1f * scale; // How different landform height

            // Put dirt //
            for (int y = stoneHeight; y <= stoneHeight + distance; y++)
            {
                TileData tileDirt = tilesData.Find((t) => t.x == x && t.y == y);
                tileDirt.tileMaterialId = 2;
            }

            // Put last layer, grass //
            TileData tileGrass = tilesData.Find((t) => t.x == x && t.y == stoneHeight + distance + 1);
            tileGrass.tileMaterialId = 3;
        }

        //// Generate caves ////
        for(int i = 0; i <= 2; i++) GenerateCavesT();

        //// Generate underland form ////
        next = 0.0f; // Variable going through perlin noise
        distance = stoneHeight; // Landform starting point

        for (int x = 0; x <= maxWidth; x++)
        {
            // Generate value that we add to hight
            distance = (int)(Mathf.PerlinNoise(next, 0.0f) * 10); // Always start from same point, so if we want diffrent landform we need to add seed
            next += 1f * scale; // How different landform height

            // Put dirt //
            for (int y = stoneHeight; y >= stoneHeight - distance; y--)
            {
                TileData tileDirt = tilesData.Find((t) => t.x == x && t.y == y);
                tileDirt.tileMaterialId = 2;
            }
        }

        //// Generate ores ////
        GenerateOresById(9, 4); // Generate coal ore
        GenerateOresById(8, 5); // Generate copper ore
        GenerateOresById(7, 6); // Generate iron ore
        GenerateOresById(6, 7); // Generate silver ore
        GenerateOresById(5, 8); // Generate gold ore
        GenerateOresById(4, 9); // Generate magnesium ore
        GenerateOresById(2, 10); // Generate diamond ore
        GenerateOresById(1, 11); // Generate uranium ore
        
        //// Generate industractible ore on the last layer and right left border ////
        foreach(var tileData in tilesData)
        {
            if (tileData.y == minHeight) tileData.tileMaterialId = 12;
            else if (tileData.x == 0 && tileData.y <= stoneHeight) tileData.tileMaterialId = 1;
            else if (tileData.x == maxWidth && tileData.y <= stoneHeight) tileData.tileMaterialId = 1;
        }


        //// Save generated world ////
        SaveCreatedWorld(tilesData,worldFilePath);
    }

    // Load world from file and return SaveData object with world data
    public SaveData LoadWorld(string worldFilePath)
    {
        string jsonWorldData = File.ReadAllText(worldFilePath);
        SaveData currentWorldSave = new SaveData();
        currentWorldSave = JsonUtility.FromJson<SaveData>(jsonWorldData);
        playerSpawnPoint = currentWorldSave.playerSpawnPoint;
        return currentWorldSave;
    }

    // Save ford to file after created
    public void SaveCreatedWorld(List<TileData> TilesData, string worldFilePath)
    {
        SaveData currentWorldSave = new SaveData();
        currentWorldSave.HP = 100;
        currentWorldSave.Mana = 100;
        currentWorldSave.TilesData = TilesData;
        currentWorldSave.EqSlotsData = EqManager.GetEmptySlots();
        currentWorldSave.treePosiotionsVector = new List<Vector2Int>();

        // Set player spawn point
        int worldCenterX = (int)maxWidth / 2;
        int worldCenterY = (int)TilesData.Find((tileData) => tileData.x == worldCenterX && tileData.tileMaterialId == 3).y; // Grass tile
        currentWorldSave.playerSpawnPoint = new Vector2Int(worldCenterX, worldCenterY);

        // Transform data to json string
        string jsonWorldData = currentWorldSave.GetJsonString();

        // Save json string to file
        File.Delete(worldFilePath);
        File.WriteAllText(worldFilePath, jsonWorldData);
    }

    // Save world to file, trigger by canvas save button, get data from another classes
    public void SaveWorld()
    {
        // Get actual world save path
        string worldFilePath = PlayerPrefs.GetString("worldFilePath");

        if(worldFilePath != null)
        {
            // Get data from antoher classes
            SaveData currentWorldSave = new SaveData();
            currentWorldSave.HP = PlayerManager.maxPlayerHp;
            currentWorldSave.Mana = 100;
            currentWorldSave.TilesData = TileManager.tilesData;
            currentWorldSave.EqSlotsData = EqManager.eqSlots;
            currentWorldSave.treePosiotionsVector = TreeManager.treePositions;
            currentWorldSave.playerSpawnPoint = playerSpawnPoint;

            // Transform data to json string
            string jsonWorldData = currentWorldSave.GetJsonString();

            // Save json string to file
            File.Delete(worldFilePath);
            File.WriteAllText(worldFilePath, jsonWorldData);
        }
    }

    // Generate ores
    public void GenerateOresById(int layerOresNumber, int tileId)
    {
        // Ore generator variables
        int[] oreSizes = { 2, 3, 4, 9 };
        List<int> layerOreXCoordinates = new List<int>();
        System.Random randomOreSize = new System.Random();
        System.Random randomXCoodinates = new System.Random();

        // Goig through leayers
        for (int y = stoneHeight; y >= -15; y = y - 5)
        {
            // Generate actual layer ore x locations
            for (int i = 0; i < layerOresNumber; i++)
            {
                layerOreXCoordinates.Add(randomXCoodinates.Next(3, maxWidth));
            }

            // Going through x coordinates per layer
            for (int x = 3; x <= maxWidth - 3; x++)
            {
                // If x locations is in list
                if (layerOreXCoordinates.Contains(x))
                {
                    // Put random ore size into x coordinate
                    int oreSize = oreSizes[randomOreSize.Next(0, 3)];
                    if (oreSize == 2)
                    {
                        TileData tileOre;

                        tileOre = tilesData.Find((t) => t.x == x && t.y == y);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x && t.y == y - 1);
                        tileOre.tileMaterialId = tileId; // Ore
                    }

                    if (oreSize == 3)
                    {
                        TileData tileOre;

                        tileOre = tilesData.Find((t) => t.x == x && t.y == y);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x && t.y == y - 1);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x && t.y == y - 2);
                        tileOre.tileMaterialId = tileId; // Ore
                    }

                    if (oreSize == 4)
                    {
                        TileData tileOre;

                        tileOre = tilesData.Find((t) => t.x == x && t.y == y);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x && t.y == y - 1);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x + 1 && t.y == y);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x + 1 && t.y == y - 1);
                        tileOre.tileMaterialId = tileId; // Ore
                    }

                    if (oreSize == 9)
                    {
                        TileData tileOre;

                        tileOre = tilesData.Find((t) => t.x == x && t.y == y);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x + 1 && t.y == y);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x + 2 && t.y == y);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x && t.y == y - 1);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x + 1 && t.y == y - 1);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x + 2 && t.y == y - 1);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x && t.y == y - 2);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x + 1 && t.y == y - 2);
                        tileOre.tileMaterialId = tileId; // Ore

                        tileOre = tilesData.Find((t) => t.x == x + 2 && t.y == y - 2);
                        tileOre.tileMaterialId = tileId; // Ore
                    }
                }
            }

            // Clear ore x locations
            layerOreXCoordinates.Clear();
        }
    }

    // Generate random caves
    public void GenerateCavesT()
    {
        // Create blank map
        for (int x = 0;  x < maxWidth; x++)
        {
            for (int y = 0; y < stoneHeight + Mathf.Abs(minHeight); y++)
            {
                coordinates[x, y] = 0;
            }
        }
        
        // Random fill with stone
        System.Random randomNumber = new System.Random();
        for (int x = 0, y = 0; x < maxWidth; x++)
        {
            for (y = 0; y < stoneHeight + Mathf.Abs(minHeight); y++)
            {
                if(randomNumber.Next(1,101) > 45) coordinates[x, y] = 1;
            }
        }
       
         // Wall logic
         
         for (int x = 0, y = 0; y < stoneHeight + Mathf.Abs(minHeight); y++)
         {
             for(x = 0; x < maxWidth; x++)
             {
                // Get adjacent not empty tiles number
                int wallsNumber = GetAdjacentNotEmptyTiles(x,y,1,1);

                 // Not empty tile
                 if (coordinates[x,y] == 1)
                 {
                     if (wallsNumber >= 4) 
                     {
                         coordinates[x, y] = 1; // Stone tile
                         continue;
                     }
                     if(wallsNumber < 2)
                     {
                         coordinates[x, y] = 0; // Empty tile
                         continue;
                     }

                 }
                 else
                 {
                     if(wallsNumber >= 5)
                     {
                         coordinates[x, y] = 1; // Stone tile
                         continue;
                     }
                 }
                 coordinates[x, y] = 0;      
             }
         }

         // Add coordinates to tilesData 
         foreach(var tileData in tilesData)
         {
             try
             {
                 tileData.tileMaterialId = coordinates[tileData.x, tileData.y + Mathf.Abs(minHeight)];
             }
             catch(Exception ex)
             {

             }         
         }
    }

    public int GetAdjacentNotEmptyTiles(int x, int y, int scopeX, int scopeY)
    {
        int startX = x - scopeX;
        int startY = y - scopeY;
        int endX = x + scopeX;
        int endY = y + scopeY;
        int tileCounter = 0;

        int iX = startX;
        int iY = scopeY;

        for (iX = startX; iX <= endX; iX++)
        {
            for(iY = startY; iY <= endY; iY++)
            {
                // If it isn't considered tile
                if(!(iX == x && iY == y))
                {
                    try
                    {
                        if (coordinates[iX, iY] == 1) tileCounter++;
                    }
                    catch(Exception ex)
                    {

                    }     
                }
            }
        }

        return tileCounter;
    }

    // Generate random caves
    public void GenerateCaves(int layerCavesNumber)
    {
        // Cave generator variables
        int[] caveHeights = { 30, 20, 10, 8 };
        List<int> layerCaveXCoordinates = new List<int>();
        System.Random randomCaveHeight = new System.Random();
        System.Random randomXCoodinates = new System.Random();


        // Goig through leayers
        for (int y = stoneHeight; y >= -15; y = y - 10)
        {

            // Generate actual layer cave x locations
            for (int i = 0; i < layerCavesNumber; i++)
            {
                layerCaveXCoordinates.Add(randomXCoodinates.Next(3, maxWidth));
            }

            // Going through x coordinates per layer
            for (int x = 3; x <= maxWidth - 3; x++)
            {
                // If x locations is in list
                if (layerCaveXCoordinates.Contains(x))
                {
                    // Put random cave with certain height into x coordinate
                    int caveHeight = caveHeights[randomCaveHeight.Next(0, 3)];

                    // Build cave with certain height
                    System.Random randomXWidth = new System.Random();

                    // Going through cave height
                    for(int yy = 0; yy <= caveHeight; yy++ )
                    {
                        int xWidth = randomXWidth.Next(1, 8);

                        // Going trough cave width to right
                        for (int xx = 0; xx <= xWidth; xx++ )
                        {
                            TileData tileCave;
                            try
                            {
                                tileCave = tilesData.Find((t) => t.x == x + xx && t.y == y - yy);
                                tileCave.tileMaterialId = 0; // Air
                            }
                            catch(NullReferenceException)
                            {

                            }
                            
                        }

                        // Going trough cave width to left
                        for (int xx = 0; xx <= xWidth; xx++)
                        {
                            TileData tileCave;
                            try
                            {
                                tileCave = tilesData.Find((t) => t.x == x - xx && t.y == y - yy);
                                tileCave.tileMaterialId = 0; // Air
                            }
                            catch (NullReferenceException)
                            {

                            }

                        }
                    }
                }
            }

            // Clear cave x locations
            layerCaveXCoordinates.Clear();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

//// Save data class ////
[Serializable]
public class SaveData
{
    public int HP;
    public int Mana;
    public List<TileData> TilesData;
    public List<EqSlot> EqSlotsData;
    public List<Vector2Int> treePosiotionsVector;
    public Vector2Int playerSpawnPoint;

    public string GetJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}


