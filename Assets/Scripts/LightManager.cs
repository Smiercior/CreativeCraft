using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightManager : MonoBehaviour
{
    public Transform camera;
    public Transform player;
    public Transform sunLight;
    public Transform torchLight;
    public GameObject placedTorchLight;
    public Tilemap tilemap;
    public static List<Vector2> placedTorchLightPositions;

    // Start is called before the first frame update
    void Start()
    {
        placedTorchLightPositions = new List<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        // Sun follow camera
        sunLight.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, 0);

        // If player is in cave, shut down light
        if (player.transform.position.y <= WorldManager.stoneHeight) sunLight.gameObject.SetActive(false);
        else if (player.transform.position.y > WorldManager.stoneHeight) sunLight.gameObject.SetActive(true);

        // If player select troch from eq, set torch light there
        if (EqManager.GetSelectedItemId() == 116)
        {
            torchLight.gameObject.SetActive(true);
            torchLight.position = player.position;
        }
        else torchLight.gameObject.SetActive(false);

        // Place placed torch light object on every torch tile near the player
        PlaceTorches();
    }

    // Place placed torch light object on every torch tile near the player
    void PlaceTorches()
    {
        // Check area near the player
        for(int x = -20; x <= 20; x++)
        {
            for(int y = -20; y <= 20; y++)
            {
                int finalX = ((int)player.transform.position.x) + x;
                int finalY = ((int)player.transform.position.y) + y;
                Vector2 finalPos = new Vector2(finalX, finalY);
                Tile actualTile = tilemap.GetTile<Tile>(tilemap.WorldToCell(finalPos));

                // If this is torch tile, create placed torch light object there
                if (actualTile != null)
                {
                    // Only create that object once per torch position
                    if (actualTile.name == "Torch" && !placedTorchLightPositions.Contains(finalPos))
                    {
                        placedTorchLightPositions.Add(finalPos);
                        GameObject.Instantiate(placedTorchLight, finalPos, Quaternion.identity);
                    }
                }
            }
        }
    }
}
