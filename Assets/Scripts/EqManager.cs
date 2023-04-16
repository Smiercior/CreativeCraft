using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class EqManager : MonoBehaviour
{
    // Eq variables
    public static int slotNumbers = 36;
    public static List<EqSlot> eqSlots;
    public GameObject eqSlotBtnsObject;
    public static int selectedSlot;
    bool secondSlotFieldBool = false;
    public GameObject itemInfoObject;
    public GameObject pickaxeInfoObject;
    public GameObject axeInfoObject;
    public GameObject swordInfoObject;

    // WorkBench variables
    public static int craftingSlotNumbers = 10; // 10th slot is slot for crafted item
    public static List<CraftingSlot> craftingSlots;
    public GameObject craftingSlotBtnsObject;

    // Furnace variables
    public static int furnaceSlotNumbers = 3; // 3th slot is slot for gained item
    public static List<FurnaceSlot> furnaceSlots;
    public GameObject furnaceSlotBtnsObject;

    // For slot color
    public GameObject firstSlotsField;
    public GameObject secondSlotField;
    List<Image> firstSlotsFieldImages = new List<Image>();
    Image lastSelectedSlot;

    // For change items place in eq
    static Button replaceSlot;

    // Variables
    WorldManager worldManager = new WorldManager();

    // Start is called before the first frame update
    void Start()
    {
        // Set default selected slot
        selectedSlot = 1;
        UpdateSelectedSlot(1);     

        // Get actual world file path from registry and set eq from save
        SetEq(PlayerPrefs.GetString("worldFilePath"));

        // Connect button slots with eq slots
        List<Button> eqSlotBtns = new List<Button>(eqSlotBtnsObject.GetComponentsInChildren<Button>());
        foreach (EqSlot eqSlot in eqSlots)
        {
            eqSlot.slotButton = eqSlotBtns.Find((eqSBtn) => int.Parse(eqSBtn.name.Substring(eqSBtn.name.IndexOf('t') + 1)) == eqSlot.slotNumber);
        }

        // Get empty crafting slots
        craftingSlots = new List<CraftingSlot>();
        craftingSlots = GetEmptyCraftingSlots();

        // Connect crafting button slots with crafting slots
        List<Button> craftingSlotBtns = new List<Button>(craftingSlotBtnsObject.GetComponentsInChildren<Button>());
        foreach (CraftingSlot craftingSlot in craftingSlots)
        {
            craftingSlot.craftingSlotButton = craftingSlotBtns.Find((craftingSBtn) => int.Parse(craftingSBtn.name.Substring(craftingSBtn.name.IndexOf('t') + 1)) == craftingSlot.slotNumber);

        }

        // Get empty furnace slots
        furnaceSlots = new List<FurnaceSlot>();
        furnaceSlots = GetEmptyFurnaceSlots();

        // Connect furnace button slots with furnace slots
        List<Button> furnaceSlotBtns = new List<Button>(furnaceSlotBtnsObject.GetComponentsInChildren<Button>());
        foreach(FurnaceSlot furnaceSlot in furnaceSlots)
        {
            furnaceSlot.furnaceSlotButton = furnaceSlotBtns.Find((furnaceSBtn) => int.Parse(furnaceSBtn.name.Substring(furnaceSBtn.name.IndexOf('t') + 1)) == furnaceSlot.slotNumber);
        }

        // Update eq
        //UpdateEqItems();
        UpdateItems<EqSlot>(eqSlots);
    }

    // Update is called once per frame
    void Update()
    {
        // Select slot manager
        if (Input.GetButtonDown("1"))
        {
            selectedSlot = 1;
            UpdateSelectedSlot(selectedSlot);
        }
        else if (Input.GetButtonDown("2"))
        {
            selectedSlot = 2;
            UpdateSelectedSlot(selectedSlot);
        }
        else if (Input.GetButtonDown("3"))
        {
            selectedSlot = 3;
            UpdateSelectedSlot(selectedSlot);
        }
        else if (Input.GetButtonDown("4"))
        {
            selectedSlot = 4;
            UpdateSelectedSlot(selectedSlot);
        }
        else if (Input.GetButtonDown("5"))
        {
            selectedSlot = 5;
            UpdateSelectedSlot(selectedSlot);
        }
        else if (Input.GetButtonDown("6"))
        {
            selectedSlot = 6;
            UpdateSelectedSlot(selectedSlot);
        }
        else if (Input.GetButtonDown("7"))
        {
            selectedSlot = 7;
            UpdateSelectedSlot(selectedSlot);
        }
        else if (Input.GetButtonDown("8"))
        {
            selectedSlot = 8;
            UpdateSelectedSlot(selectedSlot);
        }
        else if (Input.GetButtonDown("9"))
        {
            selectedSlot = 9;
            UpdateSelectedSlot(selectedSlot);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0) // Mouse wheel up
        {
            selectedSlot++;
            if (selectedSlot > 9) selectedSlot = 1;
            UpdateSelectedSlot(selectedSlot);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) // Mouse wheel down
        {
            selectedSlot--;
            if (selectedSlot < 1) selectedSlot = 9;
            UpdateSelectedSlot(selectedSlot);
        }

        // Hide/Show eq ( only slots 10-36, slots 1-9 are always visible )
        if (Input.GetButtonDown("e"))
        {
            foreach (EqSlot eqSlot in eqSlots)
            {
                if (eqSlot.slotNumber > 9)
                {
                    eqSlot.slotButton.gameObject.SetActive(secondSlotFieldBool);
                }
            }
            secondSlotField.SetActive(secondSlotFieldBool);

            // Hide item info if secondSlotFieldBool is false
            if (secondSlotFieldBool == false)
            {
                itemInfoObject.SetActive(false);
                pickaxeInfoObject.SetActive(false);
                axeInfoObject.SetActive(false);
                swordInfoObject.SetActive(false);
            }

            secondSlotFieldBool = !secondSlotFieldBool;
        }

        // Cancel item item transport, RPM
        if(Input.GetMouseButtonDown(1))
        {
            ClearCursorAndSlot();
        }
    }

    // Get item from eq and add it to furnace slot, gain furnace item
    public void OnFurnaceSlotClicked(Button clickedFurnaceSlot)
    {
        int furnaceSlotNumber = int.Parse(clickedFurnaceSlot.name.Substring(clickedFurnaceSlot.name.IndexOf("t") + 1));

        //// Getting item from eq or furnace slot ////
        MoveItem(clickedFurnaceSlot);

        //// Check if you can gain something ////
        string actualFurnacePattern = "";

        foreach (FurnaceSlot furnaceSlot in furnaceSlots)
        {
            // 3th slot is for gained item
            if (furnaceSlot.slotNumber == 3) continue;
            actualFurnacePattern += furnaceSlot.itemId + ",";
        }

        // Remove ',' from end of string
        actualFurnacePattern = actualFurnacePattern.Substring(0, actualFurnacePattern.Length - 1);

        // Gain item
        if (Item.furnaceRecipe.ContainsKey(actualFurnacePattern))
        {
            int gainedItemId = Item.furnaceRecipe[actualFurnacePattern][0];
            int gainedItemFuelAmount = Item.furnaceRecipe[actualFurnacePattern][1];
            int fuelItemId = Item.furnaceRecipe[actualFurnacePattern][2];
            FurnaceSlot fuelSlot = furnaceSlots.Find((furnaceS) => furnaceS.slotNumber == 2);

            if(fuelSlot.itemAmount >= gainedItemFuelAmount)
            {
                // Get items from furnace slots except gained slot ( 3th slot )
                foreach (FurnaceSlot furnaceSlot in furnaceSlots)
                {
                    // Item slot
                    if (furnaceSlot.slotNumber == 1) furnaceSlot.itemId = 0;

                    // Fuel slot
                    else if(furnaceSlot.slotNumber == 2)
                    {
                        furnaceSlot.itemAmount -= gainedItemFuelAmount;
                        if(furnaceSlot.itemAmount == 0)
                        {
                            furnaceSlot.itemId = 0;
                        }
                    }
                }

                // Gain item
                FurnaceSlot gainedSlot = furnaceSlots.Find((gainedS) => gainedS.slotNumber == 3);

                // If gain slot already have item, add that item to eq
                if (gainedSlot.itemId != 0)
                {
                    AddToEq(gainedSlot.itemId);
                    gainedSlot.itemId = 0;
                }

                // Gain item
                gainedSlot.itemId = gainedItemId;

                //// Update eq and furnace ////
                UpdateItems<EqSlot>(eqSlots);
                UpdateItems<FurnaceSlot>(furnaceSlots);
            }
        }   
    }

    // Get item from furnace gained slot and place in eq
    public void OnGainedSlotClicked(Button clickedGainedSlot)
    {
        MoveItem(clickedGainedSlot);
    }

    // Get item from eq and add it to crafting slot, craft item
    public void OnCraftingSlotClicked(Button clickedCraftingSlot)
    {
        int craftingSlotNumber = int.Parse(clickedCraftingSlot.name.Substring(clickedCraftingSlot.name.IndexOf("t") + 1));

        //// Getting item from eq or crafting slot ////
        MoveItem(clickedCraftingSlot);
  
        //// Check if you can craft something ////
        string actualCraftingPattern = "";
        foreach(CraftingSlot craftingSlot in craftingSlots)
        {
            // 10th slot is for crafted item
            if (craftingSlot.slotNumber == 10) continue;
            actualCraftingPattern += craftingSlot.itemId + ",";
        }

        // Remove ',' from end of string
        actualCraftingPattern = actualCraftingPattern.Substring(0, actualCraftingPattern.Length - 1);

        // Craft item
        if (Item.itemRecipe.ContainsKey(actualCraftingPattern))
        {
            // Clear crafting slots except crafted slot ( 10th slot )
            foreach (CraftingSlot craftingSlot in craftingSlots)
            {
                if(craftingSlot.slotNumber != 10) craftingSlot.itemId = 0;
            }

            CraftingSlot craftedSlot = craftingSlots.Find((craftedS) => craftedS.slotNumber == 10);
            int actualCraftedItemId = Item.itemRecipe[actualCraftingPattern][0];
            int actualCraftedItemAmount = Item.itemRecipe[actualCraftingPattern][1];

            // If crafted slot already have item
            if (craftedSlot.itemId != 0)
            {
                

                // If this item is diffrent than actual crafting item, add that item to eq and crafted item to crafted slot
                if (craftedSlot.itemId != actualCraftedItemId)
                {
                    // Add that item to eq
                    AddToEq(craftedSlot.itemId,craftedSlot.itemAmount);

                    // Add new item to crafted slot
                    craftedSlot.itemId = actualCraftedItemId;
                    craftedSlot.itemAmount = actualCraftedItemAmount;
                }
                // If this item is same as actual crafting item add them
                else if(craftedSlot.itemId == actualCraftedItemId)
                {
                    // Add items
                    craftedSlot.itemAmount += actualCraftedItemAmount;

                    // If added items amount is greater than item max stack
                    if(craftedSlot.itemAmount > Item.itemsMaxStack[craftedSlot.itemId])
                    {
                        int leftItemAmount = craftedSlot.itemAmount - Item.itemsMaxStack[craftedSlot.itemId];
                        AddToEq(craftedSlot.itemId,Item.itemsMaxStack[craftedSlot.itemId]);
                        craftedSlot.itemAmount = leftItemAmount;
                    }
                }
            }
            else
            {
                craftedSlot.itemId = actualCraftedItemId;
                craftedSlot.itemAmount = actualCraftedItemAmount;
            }
        }

        //// Update eq and crafting ////
        //UpdateEqItems();
        //UpdateCraftingItems();
        UpdateItems<EqSlot>(eqSlots);
        UpdateItems<CraftingSlot>(craftingSlots);
    }

    // Get item from crafted slot and place in eq
    public void OnCraftedSlotClicked(Button clickedCraftedSlot)
    {
        MoveItem(clickedCraftedSlot);   
    }

    // Get item from one eq slot and put it in another
    public void OnSlotClicked(Button clickedSlot)
    {
        int slotNumber = int.Parse(clickedSlot.name.Substring(clickedSlot.name.IndexOf("t") + 1));

        MoveItem(clickedSlot);
    }

    // Move item from one slot to another ( for eqSlot,craftingSlot,furnaceSlot )
    public void MoveItem(Button clickedSlot)
    {
        // Replace items ( player press replaceSlot and second slot )
        if (replaceSlot != null)
        {
            // Get first clicked slot class
            string firstSlotClass = replaceSlot.name.Substring(0, replaceSlot.name.IndexOf("S"));
           
            // Get second clicked slot class
            string secondSlotClass = clickedSlot.name.Substring(0, replaceSlot.name.IndexOf("S"));

            if (firstSlotClass == "E")
            {
                // Get item from one eq slot and move to another eq slot
                if (secondSlotClass == "E")
                {
                    EqSlot firstSlot = eqSlots.Find((eqSlot) => eqSlot.slotButton == replaceSlot);
                    EqSlot secondSlot = eqSlots.Find((eqSlot) => eqSlot.slotButton == clickedSlot);
                    int tempItemId = firstSlot.itemId;
                    int tempItemAmount = firstSlot.itemAmount;

                    // If it isn't the same slots
                    if (firstSlot.slotNumber != secondSlot.slotNumber)
                    {
                        // If it's the same items add them
                        if (firstSlot.itemId == secondSlot.itemId)
                        {
                            // Get only one item from first slot and add it to second slot items
                            if (Input.GetKey("left shift")) // GetKey will return true continuously while the user holds down the specific key
                            {
                                // Add one item to second slot
                                secondSlot.itemAmount += 1;

                                if (secondSlot.itemAmount <= Item.itemsMaxStack[secondSlot.itemId])
                                {
                                    // Get one item from first slot
                                    firstSlot.itemAmount -= 1;
                                    if (firstSlot.itemAmount == 0)
                                    { 
                                        firstSlot.itemId = 0;
                                        ClearCursorAndSlot();
                                    }

                                }
                                else
                                {
                                    secondSlot.itemAmount -= 1;
                                }
                            }
                            // Get certain amount of items from first slot and add them to second slot items
                            else
                            {
                                secondSlot.itemAmount += firstSlot.itemAmount;

                                if (secondSlot.itemAmount > Item.itemsMaxStack[secondSlot.itemId])
                                {
                                    int leftItemsAmount = secondSlot.itemAmount - Item.itemsMaxStack[secondSlot.itemId];
                                    secondSlot.itemAmount -= leftItemsAmount;
                                    firstSlot.itemAmount = leftItemsAmount;
                                }
                                // Get all items from first slot and add them to second slot items
                                else
                                {
                                    firstSlot.itemAmount = 0;
                                    firstSlot.itemId = 0;
                                }

                                ClearCursorAndSlot();
                            }


                        }
                        // It's different items
                        else
                        {
                            // Get only one item from first slot
                            if (Input.GetKey("left shift"))
                            {
                                // If second slot is empty
                                if (secondSlot.itemId == 0)
                                {
                                    firstSlot.itemAmount -= 1;
                                    if (firstSlot.itemAmount == 0)
                                    { 
                                        firstSlot.itemId = 0;
                                        ClearCursorAndSlot();
                                    }


                                    secondSlot.itemAmount += 1;
                                    secondSlot.itemId = firstSlot.itemId;
                                }
                            }
                            else
                            {
                                firstSlot.itemId = secondSlot.itemId;
                                firstSlot.itemAmount = secondSlot.itemAmount;

                                secondSlot.itemId = tempItemId;
                                secondSlot.itemAmount = tempItemAmount;

                                ClearCursorAndSlot();
                            }
                        }
                    }

                }

                // Get item from eq and place in work bench crafting slot ( get only one item )
                else if (secondSlotClass == "W")
                {
                    
                    EqSlot eqSlot = eqSlots.Find((eqS) => eqS.slotButton == replaceSlot);
                    CraftingSlot craftingSlot = craftingSlots.Find((craftingS) => craftingS.craftingSlotButton == clickedSlot);

                    // Check if item isn't none item and if it isn't crafted slot ( 10th slot )
                    if (eqSlot.itemId != 0 && craftingSlot.slotNumber != 10)
                    {
                        // Add item to crafting slot if it's empty
                        if(craftingSlot.itemId == 0)
                        {
                            craftingSlot.itemId = eqSlot.itemId;

                            // Lower eq item amount
                            eqSlot.itemAmount--;

                            // If player have zero amount of selected item
                            if (eqSlot.itemAmount == 0)
                            {
                                eqSlot.itemId = 0;
                                ClearCursorAndSlot();
                            }
                        }          
                    }    
                }

                // Get item form eq and place in furnace slot ( get only one item )
                else if(secondSlotClass == "F")
                {
                    EqSlot eqSlot = eqSlots.Find((eqS) => eqS.slotButton == replaceSlot);
                    FurnaceSlot furnaceSlot = furnaceSlots.Find((furnaceS) => furnaceS.furnaceSlotButton == clickedSlot);

                    // Check if item isn't none item and if it isn't furnace gained slot ( 3th slot )
                    if (eqSlot.itemId != 0)
                    {
                        // If furnace slot isn't empty and it's fuel slot ( 2th slot ) item can stack
                        if(furnaceSlot.itemId != 0 && furnaceSlot.slotNumber == 2)
                        {
                            // If it's same item add them ( if that amount is less than max stack ) otherwise don't do anything
                            if(furnaceSlot.itemId == eqSlot.itemId)
                            {
                                if(furnaceSlot.itemAmount + 1 <= Item.itemsMaxStack[eqSlot.itemId])
                                {
                                    // Add item to fuel slot
                                    furnaceSlot.itemAmount++;

                                    // Lower eq item amount
                                    eqSlot.itemAmount--;

                                    // If player have zero amount of selected item
                                    if (eqSlot.itemAmount == 0)
                                    {
                                        eqSlot.itemId = 0;
                                        ClearCursorAndSlot();
                                    }
                                }
                            }                 
                        }

                        // If furnace slot is empty
                        if (furnaceSlot.itemId == 0)
                        {
                            // Add item to furnace slot
                            furnaceSlot.itemId = eqSlot.itemId;

                            // If it's fuel slot ( 2th slot )
                            if (furnaceSlot.slotNumber == 2)
                            {
                                furnaceSlot.itemAmount++;
                            }

                            // Lower eq item amount
                            eqSlot.itemAmount--;

                            // If player have zero amount of selected item
                            if (eqSlot.itemAmount == 0)
                            {
                                eqSlot.itemId = 0;
                                ClearCursorAndSlot();
                            }
                        }           
                    }
                }   
            }        
            // Update eq,crafting,furnace
            UpdateItems<EqSlot>(eqSlots);
            UpdateItems<CraftingSlot>(craftingSlots);
            UpdateItems<FurnaceSlot>(furnaceSlots);
        }

        // If clickedSlot was eq slot set cursor with item, Player must click second slot to replace items
        // else get item from crafting slot or furnace slot and put in eq
        else
        {
            // Get replaceSlot parent object, first clicked slot
            string firstSlotClass = clickedSlot.name.Substring(0, clickedSlot.name.IndexOf("S"));

            // Get item from crafting slot and place in eq
            if (firstSlotClass == "W")
            {
                CraftingSlot firstSlot = craftingSlots.Find((craftingS) => craftingS.craftingSlotButton == clickedSlot);  

                // Crafted slot ( 10th slot )
                if(firstSlot.slotNumber == 10)
                {
                    AddToEq(firstSlot.itemId, firstSlot.itemAmount);
                    firstSlot.itemId = 0;
                    firstSlot.itemAmount = 0;
                }
                // Crafting slot
                else
                {
                    AddToEq(firstSlot.itemId);
                    firstSlot.itemId = 0;
                }
                
            }
            // Get item from furnace slot and place in eq
            else if (firstSlotClass == "F")
            {
                FurnaceSlot furnaceSlot = furnaceSlots.Find((furnaceS) => furnaceS.furnaceSlotButton == clickedSlot); 
                
                // Fuel slot
                if(furnaceSlot.slotNumber == 2)
                {
                    AddToEq(furnaceSlot.itemId,furnaceSlot.itemAmount);
                    furnaceSlot.itemId = 0;
                    furnaceSlot.itemAmount = 0;
                }
                else
                {
                    AddToEq(furnaceSlot.itemId);
                    furnaceSlot.itemId = 0;
                }
                
            }
            // Get item from eq and qait for second slot click
            else if(firstSlotClass == "E")
            {
                replaceSlot = clickedSlot;
                CursorManager.SetWithItemCursor();
            }

            // Update eq,crafting,furnace
            UpdateItems<EqSlot>(eqSlots);
            UpdateItems<CraftingSlot>(craftingSlots);
            UpdateItems<FurnaceSlot>(furnaceSlots);
        }
    }

    public void ClearCursorAndSlot()
    {
        // Clear replaceSlot object
        replaceSlot = null;

        // Set cursor without item
        CursorManager.SetNoItemCursor();
    }

    public void UpdateSelectedSlot(int slotNumber)
    {
        // Reset last selected slot color
        if (lastSelectedSlot != null) lastSelectedSlot.sprite = Resources.Load<Sprite>("SlotBackground");

        // Find selected slot and change it's color
        Image[] images = firstSlotsField.GetComponentsInChildren<Image>();
        foreach (Image img in images) firstSlotsFieldImages.Add(img);
        Image selectedSlot = firstSlotsFieldImages.Find((imgSlot) => imgSlot.name.Substring(imgSlot.name.IndexOf('e') + 1) == slotNumber.ToString());
        if (selectedSlot != null) selectedSlot.sprite = Resources.Load<Sprite>("SelectedSlotBackground");

        // Update last selected slor
        lastSelectedSlot = selectedSlot;
    }

    // Get eq from file save
    public void SetEq(string worldFilePath)
    {
        // Get eq data
        worldManager = new WorldManager();
        eqSlots = worldManager.LoadWorld(worldFilePath).EqSlotsData;     
    }

    // Add item to eq
    public static bool AddToEq(int itemId, int itemAmount = 1)
    {
        // Find slot with same item and non max item amount
        EqSlot sameEqSlot = eqSlots.Find((eqSlot) => eqSlot.itemId == itemId && eqSlot.itemAmount < Item.itemsMaxStack[itemId]);
        if(sameEqSlot != null)
        {
            // Add item to slot, check if this slot max stack is enough for itemAmount
            if(sameEqSlot.itemAmount + itemAmount <= Item.itemsMaxStack[itemId])
            {
                sameEqSlot.itemAmount += itemAmount;
            }
            else
            {
                int leftItemAmount = sameEqSlot.itemAmount + itemAmount - Item.itemsMaxStack[itemId]; // 12 + 10 - 16 = 6
                sameEqSlot.itemAmount += itemAmount - leftItemAmount;

                // Find another slot for rest amount of that item
                AddToEq(itemId, leftItemAmount);
            }
        }

        // There isn't any same item or same item with non max item amount
        else
        {
            // Find free eq slot and put there item
            EqSlot freeEqSlot = eqSlots.Find((eqSlot) => eqSlot.itemId == 0);
            if (freeEqSlot != null)
            {
                freeEqSlot.itemId = itemId;
                freeEqSlot.itemAmount = itemAmount;
            }
            // There isn't any free slot
            else return false;    
        }

        // Update eq
        UpdateItems<EqSlot>(eqSlots);

        return true;
    }

    // Take item from eq and place it in the world ( if we can place that item ) ( 0 - mean that we can't place it )
    public static int GetSelectedItem()
    {
        // Get selected item
        EqSlot selectedSlotItem = eqSlots.Find((i) => i.slotNumber == selectedSlot);
        if(selectedSlotItem != null)
        {
            int itemId = selectedSlotItem.itemId;
            // Check if we can place that item
            if (Item.canPlaceItem[itemId])
            {
                // Get item from eq
                selectedSlotItem.itemAmount--;

                // Clear item from eq if it's amount equal 0
                if (selectedSlotItem.itemAmount == 0) selectedSlotItem.itemId = 0;

                // Update eq
                //UpdateEqItems();
                UpdateItems<EqSlot>(eqSlots);
                return itemId;
            }
            else return 0;
            
        }
        return 0;
    }

    // Get item id from eq
    public static int GetSelectedItemId()
    {
        int itemId = 0;
        // Get selected item
        EqSlot selectedSlotItem = eqSlots.Find((i) => i.slotNumber == selectedSlot);
        if (selectedSlotItem != null)
        {
            itemId = selectedSlotItem.itemId;
        }
        return itemId;
    }

    // Update eq/crafting/furnace slots image according to items id
    public static void UpdateItems<T>(List<T> Slots)
    {
        // Update item image and amount for every button slot

        foreach (T slot in Slots)
        {
            if(slot != null)
            {
                if (slot is EqSlot eqS)
                {
                    // Item image
                    try
                    {
                        if (eqS.itemId != 0) eqS.slotButton.image.sprite = Resources.Load<Sprite>(Item.itemsSpriteName[eqS.itemId]);
                        else eqS.slotButton.image.sprite = null;
                    }
                    catch (KeyNotFoundException)
                    {
                        eqS.slotButton.image.sprite = null;
                    }
                    // Item amount
                    eqS.slotButton.GetComponentInChildren<Text>().text = eqS.itemAmount.ToString();
                }
                else if (slot is CraftingSlot cS)
                {
                    try
                    {
                        if (cS.itemId != 0)
                        {
                            cS.craftingSlotButton.image.sprite = Resources.Load<Sprite>(Item.itemsSpriteName[cS.itemId]);    
                        }
                        else cS.craftingSlotButton.image.sprite = null;
                    }
                    catch (KeyNotFoundException)
                    {
                        cS.craftingSlotButton.image.sprite = null;
                    }

                    // Item amount only for crated slot ( 10th slot )
                    if (cS.slotNumber == 10) cS.craftingSlotButton.GetComponentInChildren<Text>().text = cS.itemAmount.ToString();
                }
                else if (slot is FurnaceSlot fS)
                {
                    try
                    {
                        if (fS.itemId != 0) fS.furnaceSlotButton.image.sprite = Resources.Load<Sprite>(Item.itemsSpriteName[fS.itemId]);
                        else fS.furnaceSlotButton.image.sprite = null;
                    }
                    catch (KeyNotFoundException)
                    {
                        fS.furnaceSlotButton.image.sprite = null;
                    }

                    // Item amount only for fuel slot ( 2th slot )
                    if (fS.slotNumber == 2) fS.furnaceSlotButton.GetComponentInChildren<Text>().text = fS.itemAmount.ToString();
                }
            }
        }
    }

    // Get new eq, with starting items
    public static List<EqSlot> GetEmptySlots()
    {
        List<EqSlot> emptyEqSlots = new List<EqSlot>();

        for (int x = 0; x < slotNumbers; x++)
        {
            // WoodPickaxe
            if(x + 1 == 1) emptyEqSlots.Add(new EqSlot(101, 1, x + 1));
            // WorkBench
            else if(x + 1 == 2) emptyEqSlots.Add(new EqSlot(1001, 1, x + 1));
            // Empty slot
            else emptyEqSlots.Add(new EqSlot(0, 0, x + 1));
        }

        return emptyEqSlots;
    }

    // Get empty crafting slots
    public static List<CraftingSlot> GetEmptyCraftingSlots()
    {
        List<CraftingSlot> emptyCraftingSlots = new List<CraftingSlot>();

        for (int x = 0; x < craftingSlotNumbers; x++)
        {
            emptyCraftingSlots.Add(new CraftingSlot(0, x + 1));
        }

        return emptyCraftingSlots;
    }

    // Get empty firnave slots
    public static List<FurnaceSlot> GetEmptyFurnaceSlots()
    {
        List<FurnaceSlot> emptyFurnaceSlots = new List<FurnaceSlot>();

        for (int x = 0; x < furnaceSlotNumbers; x++)
        {
            emptyFurnaceSlots.Add(new FurnaceSlot(0, x + 1));
        }

        return emptyFurnaceSlots;
    }
}

//// EqSlot data class ////
[Serializable]
public class EqSlot
{
    public int itemId;
    public int itemAmount;
    public int slotNumber;

    [NonSerialized]
    public Button slotButton;

    public EqSlot(int iId, int iA, int sN)
    {
        itemId = iId;
        itemAmount = iA;
        slotNumber = sN;
    }

    public static string GetClassName()
    {
        return "EqSlot";
    }
}

public class CraftingSlot
{
    public int itemId;
    public int slotNumber;
    public int itemAmount; // Only for crafted slot ( 10th slot )

    public Button craftingSlotButton;
    public CraftingSlot(int iId, int sN)
    {
        itemId = iId;
        slotNumber = sN;
    }

    public static string GetClassName()
    {
        return "CraftingSlot";
    }
}

public class FurnaceSlot
{
    public int itemId;
    public int slotNumber;
    public int itemAmount; // Only for fuel slot ( 2th slot )

    public Button furnaceSlotButton;
    public FurnaceSlot(int iId, int sN)
    {
        itemId = iId;
        slotNumber = sN;
    }

    public static string GetClassName()
    {
        return "FurnaceSlot";
    }
}