using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // If player collect item
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            // Get item id
            string itemName = this.gameObject.name.Substring(0, this.gameObject.name.IndexOf("Drop"));

            int itemId = Item.itemId[itemName];

            // If item was added to eq ( if eq isn't full )
            if (EqManager.AddToEq(itemId))
            {
                Destroy(gameObject);

                // Play get sound
                SoundManager.PlayGetItemSound();
            }
        }
    }
}
