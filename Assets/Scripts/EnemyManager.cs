using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyManager : MonoBehaviour
{
    // Enemies objects
    public GameObject GreenSlimy;
    public GameObject BlueSlimy;
    public GameObject OrangeSlimy;
    public GameObject RedSlimy;
    List<GameObject> enemies = new List<GameObject>();

    // Variables for enemies spawn
    public Rigidbody2D player;
    float nextEnemySpawnTime = 20;
    int enemyPlayerSpawnDistance = 10;
    public float nextEnemySpawnCounter;
    public Tilemap tilemap;


    // Start is called before the first frame update
    void Start()
    {
        enemies.Add(GreenSlimy);
        enemies.Add(BlueSlimy);
        enemies.Add(OrangeSlimy);
        enemies.Add(RedSlimy);
        nextEnemySpawnCounter = nextEnemySpawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        //// Spawn enemies ////
        
        // Spawn next random enemy
        if(nextEnemySpawnCounter <= 0)
        {
            foreach(var spawnChance in enemiesSpawnChance)
            {
                // Get random number from enemy spawn change range
                System.Random getNumber = new System.Random();
                int drawnNumber = getNumber.Next(0, spawnChance.Value[0]);

                // If that namber is 1, then spawn enemy
                if(drawnNumber == spawnChance.Value[1])
                {
                    GameObject enemy = enemies.Find((e) => e.name == spawnChance.Key);

                    // Spawn enemy next to player
                    if (enemy != null)
                    {
                        // Get appropriate location to spawn
                        int xPos = (int)player.position.x + PlayerManager.playerDirection * enemyPlayerSpawnDistance;
                        int yPos = (int)player.position.y;

                        // Find first position with none tile and spawn enemy
                        for(int y = yPos; y <= WorldManager.maxHeight; y++)
                        {
                            Tile freeSpaceTile = tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector3(xPos, y)));
                            Tile freeSpaceTile2 = tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector3(xPos, y + 1)));
                            Tile freeSpaceTile3 = tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector3(xPos + 1, y)));
                            Tile freeSpaceTile4 = tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector3(xPos + 1, y + 1)));
                            Tile freeSpaceTile5 = tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector3(xPos - 1, y)));
                            Tile freeSpaceTile6 = tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector3(xPos - 1, y + 1)));
                            List<Tile> freeSpaceTiles = new List<Tile>() { freeSpaceTile, freeSpaceTile2, freeSpaceTile3, freeSpaceTile4, freeSpaceTile5, freeSpaceTile6 };

                            // If doesn't find any not null tile in freeSpaceTiles list, spawn enemy
                            if (freeSpaceTiles.Find((fT) => fT != null ) == null)
                            {
                                // Prevent spawn one enemy on another
                                int additionalX = getNumber.Next(0, 1);
                                Instantiate(enemy, new Vector2(xPos + additionalX, y), Quaternion.identity);
                                break;
                            }
                        }         
                    }  
                }           
            }
            nextEnemySpawnCounter = nextEnemySpawnTime;
        }
        // Wait some time
        else
        {
            nextEnemySpawnCounter -= Time.deltaTime;
        }
    }

    // Chance to spawn certain enemy, Enemy name : [random range, winning number]
    Dictionary<string, int[]> enemiesSpawnChance = new Dictionary<string, int[]>()
    {
        { "GreenSlimy", new int[] {10,1} },
        { "BlueSlimy", new int[] {15,1} },
        { "OrangeSlimy", new int[] {20,1} },
        { "RedSlimy", new int[] {30,1} },
    };
}
