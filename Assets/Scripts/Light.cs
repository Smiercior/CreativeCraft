using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Light : MonoBehaviour
{
    Tilemap tilemap;

    void Start()
    {
        tilemap = GameObject.FindGameObjectWithTag("Tile").GetComponent<Tilemap>();
    }

    void Update()
    {
        // Check if torch tile exists ( player could destory that tile )
        Vector2 finalPos = new Vector2((int)transform.position.x, (int)transform.position.y);
        Tile actualTile = tilemap.GetTile<Tile>(tilemap.WorldToCell(finalPos));

        foreach (var pos in LightManager.placedTorchLightPositions) Debug.Log(pos);

        if (actualTile != null) Debug.Log(actualTile.name);

        // If there isn't any tile or this tile isn't torch tile, remove placed light object
        if (actualTile == null)
        {
            LightManager.placedTorchLightPositions.Remove(finalPos);
            Destroy(this.gameObject);
        }
        else if (actualTile != null)
        {
            if (actualTile.name != "Torch")
            {
                LightManager.placedTorchLightPositions.Remove(finalPos);
                Destroy(this.gameObject);
            }
        }
    }
}
