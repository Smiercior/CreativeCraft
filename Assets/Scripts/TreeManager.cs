using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TreeManager : MonoBehaviour
{

    public Tilemap tilemap;
    public GameObject tree;
    public static List<Vector2Int> treePositions;
    int treeDistance = 5;
    static int treeYOffset = 7;
    static int minTreeNumber = 5;

    // Start is called before the first frame update
    void Start()
    {
        WorldManager worldManager = new WorldManager();
        treePositions = worldManager.LoadWorld(PlayerPrefs.GetString("worldFilePath")).treePosiotionsVector;

        // If there isn't any tree, generate them
        if (treePositions.Count == 0)
        {
            GenerateTreePositions();
        }

        // If there is only 5 trees left, generate new trees
        if(treePositions.Count == minTreeNumber)
        {
            treePositions.Clear();
            GenerateTreePositions();
        }

        // Place trees at treePositions
        PlaceTrees();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Generate tree positions
    public void GenerateTreePositions()
    {
        int treeDistanceCounter = 0;

        // Go through world positions
        for (int x = 0; x <= WorldManager.maxWidth; x++)
        {
            treeDistanceCounter++;

            // Place tree
            if (treeDistanceCounter == treeDistance)
            {
                // Reset treeDistanceCounter
                treeDistanceCounter = 0;
                for (int y = WorldManager.stoneHeight; y <= WorldManager.maxHeight; y++)
                {
                    // Check if tile is good for placing tree
                    if (CheckTile(x, y))
                    { 
                        treePositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }

    // Place trees from treePositions, if tree can't be placed, delete that tree position from treePositions
    public void PlaceTrees()
    {
        List<Vector2Int> treesToRemove = new List<Vector2Int>();

        // Try place trees
        foreach(Vector2Int treePosiotion in treePositions)
        {
            if (CheckTile(treePosiotion.x, treePosiotion.y))
            {
                Instantiate(tree, new Vector2(treePosiotion.x, treePosiotion.y + treeYOffset), Quaternion.identity);
            }
            else treesToRemove.Add(treePosiotion);
        }

        // Remove trees that can't be placed
        foreach(Vector2Int treeToRemove in treesToRemove)
        {
            Vector2Int removeTree = treePositions.Find((tPosition) => tPosition == treeToRemove);
            if (removeTree != null) treePositions.Remove(removeTree);
        }
    }

    // Check if tile is good for placing tree
    public bool CheckTile(int x, int y)
    {
        // Check if actual tile is grass and left,right neighbors of this tile are grass too
        Tile actualTile = tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector2(x, y)));
        Tile leftNeighborTile = tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector2(x + 1, y)));
        Tile rightNeighborTile = tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector2(x - 1, y)));
        bool canPlaceTree = false;

        if (actualTile != null && leftNeighborTile != null && rightNeighborTile != null)
        {
            if (actualTile.name == "Grass" && leftNeighborTile.name == "Grass" && rightNeighborTile.name == "Grass")
            {
                canPlaceTree = true;
                // Check if there is free space for tree, tree size 5 x 10 tiles
                for (int yy = y + 1; yy <= y + 10; yy++)
                {
                    if (tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector2(x, yy))) != null ||
                        tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector2(x + 1, yy))) != null ||
                        tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector2(x + 2, yy))) != null ||
                        tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector2(x - 1, yy))) != null ||
                        tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector2(x - 2, yy))) != null)
                    {  
                        // If even one tile isn't null, can't place tree
                        canPlaceTree = false;
                        break;
                    }
                }
            }
        }

        return canPlaceTree;
    }

    // If player chop tree, it's remove from tree positions
    public static void RemoveTree(float x, float y)
    {
        int finalX = Mathf.CeilToInt(x);
        int finalY = Mathf.CeilToInt(y) - treeYOffset;
        Vector2Int removeTree = treePositions.Find((tPosition) => tPosition.x == finalX && tPosition.y == finalY);
        if (removeTree != null) treePositions.Remove(removeTree);
    }
}
