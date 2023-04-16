using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

//// Tiles manager ////
public class TileManager : MonoBehaviour
{
    // Tiles variables
    public Collider2D boxcollider2d;
    public Tilemap tilemap;
    public const float playerMinSetDistance = 1f; // Minimum player-tile set distance

    // UI variables
    public GameObject workBenchUI;
    public GameObject furnaceUI;

    // Variables 
    Vector3 mousePos;
    Vector2 mousePos2D;
    Vector2 last_mousePos2D;
    Vector2 position;
    WorldManager worldManager = new WorldManager();
    public Transform playerBodyPosition;
    int hardness;
    public static List<TileData> tilesData;
    string worldFilePath;
    bool sameBlock = false;

    // Furnitures variables
    Vector2 workBenchPosition;
    Vector2 furnacePosition;
    static int maxPlayerWorkbenchRange = 5;
    static int maxPlayerFurnaceRange = 5;

    void Start()
    {
        // Get actual world file path from registry and set world from save
        SetWorld(PlayerPrefs.GetString("worldFilePath"));
    }

    // Generate tiles according to tile id from tiles list //
    public void SetWorld(string worldFilePath)
    {
        // Get world data
        worldManager = new WorldManager();
        tilesData = worldManager.LoadWorld(worldFilePath).TilesData;

        // Setup tiles
        foreach (TileData t in tilesData)
        {
            position.x = t.x;
            position.y = t.y;

            // Get tile from tiles pallet and put them into tilemap by it tileMaterialId
            if(t.tileMaterialId != 0) tilemap.SetTile(tilemap.WorldToCell(position), TileData.GetTileByTileMaterialId(t.tileMaterialId));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check player workbench distance
        float XF = Mathf.Abs(workBenchPosition.x - playerBodyPosition.position.x);
        float YF = Mathf.Abs(workBenchPosition.y - playerBodyPosition.position.y);
        float RF = Mathf.Sqrt(XF * XF + YF * YF);
        if (RF > maxPlayerWorkbenchRange) workBenchUI.SetActive(false);

        // Check player furnace distance
        XF = Mathf.Abs(furnacePosition.x - playerBodyPosition.position.x);
        YF = Mathf.Abs(furnacePosition.y - playerBodyPosition.position.y);
        RF = Mathf.Sqrt(XF * XF + YF * YF);
        if (RF > maxPlayerFurnaceRange) furnaceUI.SetActive(false);

        //// Check if plater have range ////
        if(CursorManager.playerHasRange)
        {
            CursorManager.SetNormalCursor();

            //// LPM - Destroy tile ////
            if (Input.GetMouseButton(0)) // LPM destroy block
            {
                // GetMouseButtonDown(0) Register only click moment //
                // GetMouseButton(0) Register holding button every frame //

                // Get mouse position
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos2D = new Vector2(mousePos.x, mousePos.y);

                if (boxcollider2d.OverlapPoint(mousePos2D) == true) // If mouse coordinates is on coordinates of ColidBox2D
                {
                    // Check if we have item that can destroy tiles ( pickaxe )
                    int itemId = EqManager.GetSelectedItemId();
                    if (itemId != 0 && Item.canDestroyTile.Contains(itemId))
                    {
                        // Check if item tier is high enough to destroy actual tile ( pickaxe tier ) ( 10000 is for indestructable tiles )
                        Tile actualTile = tilemap.GetTile<Tile>(tilemap.WorldToCell(mousePos2D));
                        int itemTier = Item.pickaxeTier[itemId];

                        if (actualTile != null)
                        {
                            if (itemTier >= TileData.tilesDestroyPickaxeTier[actualTile.name] && TileData.tilesDestroyPickaxeTier[actualTile.name] != 10000)
                            {
                                // Hit new block, set hardness
                                if (sameBlock == false)
                                {
                                    // Hitting tile make sound
                                    SoundManager.PlayTileHittingSound(true,actualTile.name);

                                    // Get new tile hardness
                                    hardness = TileData.GetTileHardness(actualTile);          
                                }

                                // Hit same block, lower hardness
                                if (mousePos2D.x >= last_mousePos2D.x - 0.49f & mousePos2D.x <= last_mousePos2D.x + 0.49f & mousePos2D.y >= last_mousePos2D.y - 0.49f & mousePos2D.y <= last_mousePos2D.y + 0.49f)
                                {
                                    sameBlock = true;
                                    hardness -= Item.pickaxeSpeed[itemId];
                                }
                                else sameBlock = false;

                                last_mousePos2D = mousePos2D; // We hit that block, so we set that as last mouse position

                                // Destroy block
                                if (hardness <= 0)
                                {
                                    // Stop hitting tile sound
                                    SoundManager.PlayTileHittingSound(false,actualTile.name);

                                    // Drop item from tile
                                    BlockDrop(tilemap.GetTile<Tile>(tilemap.WorldToCell(mousePos2D)), mousePos2D); // Create collectable item

                                    // Destroy tile
                                    tilemap.SetTile(tilemap.WorldToCell(mousePos2D), null); // Destroy tile 
                                    TileData destroyedTile = tilesData.Find((t) => t.x == (int)mousePos2D.x && t.y == (int)mousePos2D.y);
                                    destroyedTile.tileMaterialId = 0; // Save destoyed tile
                                    sameBlock = false;
                                }
                            }
                        }
                    }
                }
            }

            //// RPM - Set tile ////
            else if (Input.GetMouseButtonDown(1))
            {

                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos2D = new Vector2(mousePos.x, mousePos.y);

                // Can' place tile outside world borders
                if (!(mousePos.x < 0 || mousePos.x >= WorldManager.maxWidth + 1))
                {
                    // Only can place tile on empty space ( on null tile )
                    Tile actualTile = tilemap.GetTile<Tile>(tilemap.WorldToCell(mousePos2D));

                    // If actualTile is furniture that player can interact with
                    if( actualTile != null)
                    {
                        string finalActualTileName = actualTile.name;

                        // Remove folder name if tiles contain it
                        if (finalActualTileName.Contains("/")) finalActualTileName = finalActualTileName.Substring(finalActualTileName.IndexOf('/') + 1);

                        // If this tile is WorkBench, open WorkBench UI
                        if (finalActualTileName == "WorkBench")
                        { 
                            workBenchUI.SetActive(true);
                            workBenchPosition = mousePos;
                        }
                        // If this tile is Furnace, open Furnace UI
                        if (finalActualTileName == "Furnace")
                        {
                            furnaceUI.SetActive(true);
                            furnacePosition = mousePos;
                        }
                    }
                    // Empty space, place tile
                    else if (actualTile == null)
                    {
                        // Only can place tile next to another tile
                        if(CheckTileNeighbors(mousePos2D))
                        {
                            // Player can't place tile on itself
                            // Count player-mouse distance in flout
                            int xF = (int)Mathf.Abs(mousePos2D.x - playerBodyPosition.position.x);
                            int yF = (int)Mathf.Abs(mousePos2D.y - playerBodyPosition.position.y);
                            float rF = Mathf.Sqrt(xF * xF + yF * yF);
                            if (rF >= playerMinSetDistance) PlaceItem(mousePos2D);
                        }    
                    }
                }
            }
        }
    }

    // Check if tile has at least one neighbor tiles next to 
    bool CheckTileNeighbors(Vector2 mousePos2D)
    {
        List<Vector2> neighborTiles = new List<Vector2>();
        neighborTiles.Add(new Vector2(mousePos2D.x - 1f, mousePos2D.y)); // Left neighbor
        neighborTiles.Add(new Vector2(mousePos2D.x + 1f, mousePos2D.y)); // Right neighbor
        neighborTiles.Add(new Vector2(mousePos2D.x, mousePos2D.y + 1f)); // Upper neighbor
        neighborTiles.Add(new Vector2(mousePos2D.x, mousePos2D.y - 1f)); // Lower neighbor
        
        foreach(Vector2 neighborTile in neighborTiles)
        {
            // There is at least one neighbor tiles
            if (tilemap.GetTile<Tile>(tilemap.WorldToCell(neighborTile)) != null) return true;
        }

        // There aren't any neighbor tiles
        return false;
    }

    void PlaceItem(Vector2 mousePos2D)
    {
        int itemId = EqManager.GetSelectedItem();
        if (itemId != 0)
        {
            // Set tile by item id
            Tile newTile = new Tile();
            newTile.name = Item.itemId.First((item) => item.Value == itemId).Key;
            newTile.sprite = Resources.Load<Sprite>(Item.itemsSpriteName[itemId]);
            tilemap.SetTile(tilemap.WorldToCell(mousePos2D), newTile);

            // Update tile list
            TileData setTile = tilesData.Find((t) => t.x == (int)mousePos2D.x && t.y == (int)mousePos2D.y);
            setTile.tileMaterialId = itemId;
        }
    }

    //// Drop block as item ////
    void BlockDrop( Tile actualTile, Vector2 mouse_poz )
    {
        // Skip air tile ( null tile )
        if(actualTile != null)
        {
            string finalTileName = actualTile.name;

            // Remove folder name if tiles contain it
            if (finalTileName.Contains("/")) finalTileName = finalTileName.Substring(finalTileName.IndexOf('/') + 1);

            GameObject actualBlockDrop = Resources.Load<GameObject>($"BlockDrop/{finalTileName}Drop");
            if (actualBlockDrop != null) Instantiate(actualBlockDrop, mouse_poz, Quaternion.identity);
        }    
    }
}
