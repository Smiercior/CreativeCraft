using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tree : MonoBehaviour
{
    public BoxCollider2D tree;
    public CapsuleCollider2D treeDestroy;
    TilemapCollider2D tilemapTiles;
    public Rigidbody2D rb;
    Vector2 lastMousePos2D;
    Vector2 treePosition;
    public int hardness = 2048;


    // Start is called before the first frame update
    void Start()
    {
        tilemapTiles = GameObject.FindObjectOfType<Grid>().GetComponentInChildren<Tilemap>().GetComponent<TilemapCollider2D>();
        treePosition = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) // LPM destroy tree
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            mousePos = mousePos2D;

            if (tree.OverlapPoint(mousePos2D) == true) // If mouse is on tree ColidBox
            {
                // Check if player have range to choop the tree
                if(CursorManager.playerHasRange)
                {
                    // Check if we have item that can chop tree ( axe )
                    int itemId = EqManager.GetSelectedItemId();
                    if (Item.canChoopTree.Contains(itemId))
                    {
                        // Hit same block, lower hardness
                        if (mousePos2D.x >= lastMousePos2D.x - 0.25f & mousePos2D.x <= lastMousePos2D.x + 0.25f & mousePos2D.y >= lastMousePos2D.y - 0.25f & mousePos2D.y <= lastMousePos2D.y + 0.25f)
                        {
                            hardness -= Item.axeSpeed[itemId];
                        }

                        lastMousePos2D = mousePos2D; // We hit that block, so we set that as last mouse position

                        if (hardness == 0)
                        {
                            // Tree is falling down
                            rb.AddForceAtPosition(new Vector3(700, 0, 10), this.gameObject.transform.position);
                            hardness = 256;
                        }
                    }
                }   
            }

        }

        // Check if tree touched any tile in tilemaps
        if (treeDestroy.IsTouching(tilemapTiles)) 
        {
            // Drop ten wood
            for(int i = 0; i < 10; i++ )
            {
                Instantiate(Resources.Load<GameObject>("BlockDrop/WoodDrop"), treePosition + new Vector2(i/5,0), Quaternion.identity);
            }

            // Remove that tree from world save
            TreeManager.RemoveTree(treePosition.x,treePosition.y);
            Destroy(gameObject);
        }
    }
}
