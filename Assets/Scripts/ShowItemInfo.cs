using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Show item info if player hover over eq slot
public class ShowItemInfo : MonoBehaviour
{
    public GameObject itemInfoObject;
    public GameObject pickaxeInfoObject;
    public GameObject axeInfoObject;
    public GameObject swordInfoObject;
    Vector3 infoObjectPosition;
    public void OnMouseEnter(Button btnEqSlot)
    {
        // Get item info
        EqSlot eqSlot = EqManager.eqSlots.Find((eqS) => eqS.slotButton == btnEqSlot);

        if(eqSlot.itemId != 0)
        {
            string itemName;

            // If it's tool item
            if (Item.isTool.Contains(eqSlot.itemId))
            {
                int itemId = eqSlot.itemId;
                itemName = Item.itemId.First((item) => item.Value == eqSlot.itemId).Key;

                if (itemName.Contains("Pickaxe"))
                {
                    // Get item data
                    int itemSpeed = Item.pickaxeSpeed[itemId];
                    int itemTier = Item.pickaxeTier[itemId];

                    // Show item info object with item data;
                    List<Text> itemData = new List<Text>(pickaxeInfoObject.GetComponentsInChildren<Text>());
                    itemData.Find((itemD) => itemD.name == "ItemName").text = itemName;
                    itemData.Find((itemD) => itemD.name == "ItemSpeed").text = itemSpeed.ToString();
                    itemData.Find((itemD) => itemD.name == "ItemTier").text = itemTier.ToString();

                    pickaxeInfoObject.SetActive(true);
                    infoObjectPosition = new Vector3(btnEqSlot.transform.position.x, btnEqSlot.transform.position.y - 8, 0);
                    pickaxeInfoObject.transform.position = infoObjectPosition;
                }
                else if(itemName.Contains("Axe"))
                {
                    // Get item data
                    int itemSpeed = Item.axeSpeed[itemId];

                    // Show item info object with item data;
                    List<Text> itemData = new List<Text>(axeInfoObject.GetComponentsInChildren<Text>());
                    itemData.Find((itemD) => itemD.name == "ItemName").text = itemName;
                    itemData.Find((itemD) => itemD.name == "ItemSpeed").text = itemSpeed.ToString();

                    axeInfoObject.SetActive(true);
                    infoObjectPosition = new Vector3(btnEqSlot.transform.position.x, btnEqSlot.transform.position.y - 8, 0);
                    axeInfoObject.transform.position = infoObjectPosition;
                }
                else if (itemName.Contains("Sword"))
                {
                    // Get item data
                    int itemDamage = Item.swordDamage[itemId];
                    float itemKnockback = Item.swordKnockback[itemId];

                    // Show item info object with item data;
                    List<Text> itemData = new List<Text>(swordInfoObject.GetComponentsInChildren<Text>());
                    itemData.Find((itemD) => itemD.name == "ItemName").text = itemName;
                    itemData.Find((itemD) => itemD.name == "ItemDamage").text = itemDamage.ToString();
                    itemData.Find((itemD) => itemD.name == "ItemKnockback").text = itemKnockback.ToString();

                    swordInfoObject.SetActive(true);
                    infoObjectPosition = new Vector3(btnEqSlot.transform.position.x, btnEqSlot.transform.position.y - 8, 0);
                    swordInfoObject.transform.position = infoObjectPosition;
                }
            }
            // It isn't tool item
            else
            {
                // Get item data
                itemName = Item.itemId.First((itemN) => itemN.Value == eqSlot.itemId).Key;
                int itemMaxStack = Item.itemsMaxStack[eqSlot.itemId];
                bool itemCanDestroyTile;

                if (Item.canDestroyTile.Contains(eqSlot.itemId)) itemCanDestroyTile = true;
                else itemCanDestroyTile = false;

                // Show item info object with item data;
                List<Text> itemData = new List<Text>(itemInfoObject.GetComponentsInChildren<Text>());
                itemData.Find((itemD) => itemD.name == "ItemName").text = itemName;
                itemData.Find((itemD) => itemD.name == "ItemMaxStack").text = itemMaxStack.ToString();
                itemData.Find((itemD) => itemD.name == "ItemCanDestroyTile").text = itemCanDestroyTile.ToString();

                itemInfoObject.SetActive(true);
                infoObjectPosition = new Vector3(btnEqSlot.transform.position.x, btnEqSlot.transform.position.y - 8, 0);
                itemInfoObject.transform.position = infoObjectPosition;
            }     
        }
    }

    public void OnMouseExit(Button btnEqSlot)
    {
        itemInfoObject.SetActive(false);
        pickaxeInfoObject.SetActive(false);
        axeInfoObject.SetActive(false);
        swordInfoObject.SetActive(false);
    }
}
