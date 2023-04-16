using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    // Tool objects
    public GameObject hand;
    public GameObject swordObject;
    public GameObject toolObject;
    public GameObject tileObject;

    // Variables for tool roatation
    bool rotate;
    int rotationAngle;
    public int rotationSpeed = 10;

    int actualPlayerDirection;
    int lastPlayerDirection = 2; // 2 is just starting value

    int actualSelectedItemId;
    int lastSelectedItemId = -1; // -1 is just starting value

    // Variables for fall damage
    int maxHeigh = -100;


    // Variables for HP
    public Slider hpBar;
    public Text dieText;
    public GameObject player;
    public static int maxPlayerHp;
    public static int playerHp;
    bool playerIsDed = false;
    float respawnTime = 0;
    int regenerateHpAmount = 20;
    float regenerateHpTime = 10;
    float regenerateHpTimeCounter = 0;

    // Player direction variable
    public static int playerDirection;

    // Start is called before the first frame update
    void Start()
    {
        // Get player hp from save file
        WorldManager worldManager = new WorldManager();
        maxPlayerHp = worldManager.LoadWorld(PlayerPrefs.GetString("worldFilePath")).HP;
        playerHp = maxPlayerHp;
        hpBar.maxValue = playerHp;
        hpBar.value = playerHp;

        // Move player to spawn point
        MovePlayerToSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        // Update player tool if tool item is selected in eq
        CheckEqSelectedItem();

        // Move tool if player prees LMB ( Left Mouse Button )
        MoveToolOnLMB();

        // Update hp bar
        hpBar.value = playerHp;

        // Respawn player after 4 seconds
        if(playerIsDed)
        {
            respawnTime += Time.deltaTime;
            if (respawnTime >= 4)
            {
                respawnTime = 0;
                dieText.gameObject.SetActive(false);
                player.SetActive(true);
                playerHp = maxPlayerHp;
                hpBar.value = playerHp;
                MovePlayerToSpawn();
                playerIsDed = false;
            }
        }

        // Update player direction ( -1 Left Arrow, 1 Right Arro, 0 None Arrow )
        if ((int)Input.GetAxisRaw("Horizontal") != 0) playerDirection = (int)Input.GetAxisRaw("Horizontal");

        // Regenerate player's hp
        RegenerateHp();
    }

    // Physics operations always must be used in FixedUpdate method
    void FixedUpdate()
    {
        // Rotate tool
        if(rotate)
        {
            hand.transform.Rotate(new Vector3(0,0,-actualPlayerDirection),rotationSpeed);

            // Count actual rotation angle
            rotationAngle += rotationSpeed;
            if (rotationAngle >= 360) rotationAngle = 0;
        }
        // If player doesn't prees LMB, rotate object to default position and stop rotating
        else if(rotate != true && rotationAngle != 0)
        {
            hand.transform.Rotate(new Vector3(0, 0, -actualPlayerDirection), rotationSpeed);
            rotationAngle += rotationSpeed;
            if (rotationAngle >= 360) rotationAngle = 0;
        }

        // Check if damge player from falling damage
        CheckFallDamage();
    }

    // Move player to spawn position
    void MovePlayerToSpawn()
    {
        if (WorldManager.playerSpawnPoint != null)
        {
            Vector3 playerPosition = new Vector3(WorldManager.playerSpawnPoint.x, WorldManager.playerSpawnPoint.y);
            player.gameObject.transform.position = playerPosition;
        }
    }

    // Check what item is selected in eq, if it's tool set that tool in plater's hand
    void CheckEqSelectedItem()
    {
        int selectedToolId = EqManager.GetSelectedItemId();
        string toolName = Item.itemId.First((item) => item.Value == selectedToolId).Key;
        toolObject.SetActive(false);
        swordObject.SetActive(false);
        tileObject.SetActive(false);

        if (Item.isTool.Contains((EqManager.GetSelectedItemId())))
        {
            if (toolName.Contains("Pickaxe") || toolName.Contains("Axe"))
            {
                toolObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Item.itemsSpriteName[selectedToolId]);
                toolObject.SetActive(true);
            }
            else if (toolName.Contains("Sword"))
            {
                swordObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Item.itemsSpriteName[selectedToolId]);
                swordObject.SetActive(true);
            }
        }
        // Update player tile object in hand if tile item is selected in eq
        else if (Item.canPlaceItem.ContainsKey(selectedToolId))
        {
            tileObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Item.itemsSpriteName[selectedToolId]);
            tileObject.SetActive(true);
        }
        else
        {
            toolObject.SetActive(false);
            swordObject.SetActive(false);
            tileObject.SetActive(false);
        }
    }

    // Move tool if player prees LMB ( Left Mouse Button )
    void MoveToolOnLMB()
    {
        // Detect LMB preesed moment, start rotating tool
        if (Input.GetMouseButtonDown(0))
        {
            rotate = true;

            // Can't rotate tile object
            if (tileObject.activeSelf == true) rotate = false;
        }
        // Detect LMB release, stop rotating tool
        if (Input.GetMouseButtonUp(0))
        {
            rotate = false;
        }

        // Detect player direction, if player change direction reset roation, ( -1 Left Arrow, 1 Right Arro, 0 None Arrow )
        if ((int)Input.GetAxisRaw("Horizontal") != 0)
        {
            // Get actual player direction
            actualPlayerDirection = (int)Input.GetAxisRaw("Horizontal");

            // Check if player change direction
            if (lastPlayerDirection != 2)
            {
                // Reset rotation
                if (actualPlayerDirection != lastPlayerDirection)
                {
                    rotationAngle = 0;
                    if (actualPlayerDirection == -1) hand.transform.rotation = Quaternion.Euler(0, 0, 93.62f);
                    else if (actualPlayerDirection == 1) hand.transform.rotation = Quaternion.Euler(0, 0, -93.62f);

                }
            }

            // Actual direction is last player direction
            lastPlayerDirection = actualPlayerDirection;
        }

        // Get actual selected item id
        actualSelectedItemId = EqManager.GetSelectedItemId();

        // Check if player change selected item
        if(lastSelectedItemId != -1)
        {
            // Reset rotation
            if(actualSelectedItemId != lastSelectedItemId)
            {
                rotationAngle = 0;
                if (actualPlayerDirection == -1) hand.transform.rotation = Quaternion.Euler(0, 0, -93.62f);
                else if (actualPlayerDirection == 1) hand.transform.rotation = Quaternion.Euler(0, 0, +93.62f);
            }
        }
        lastSelectedItemId = actualSelectedItemId;
    }

    // If player fell down applay damage or kill player
    void CheckFallDamage()
    {
        // If player is in the air ( falling down ), count in air time
        if(!CharacterController2D.m_Grounded)
        {
            // Get highest player jump position
            if (player.transform.position.y > maxHeigh)
            {
                maxHeigh = (int)player.transform.position.y;
            }
        }
        else if(CharacterController2D.m_Grounded)
        {
            // Count player fell distance
            int fallPosition = Mathf.CeilToInt(player.transform.position.y);
            int fallDistance = maxHeigh - fallPosition;

            // Reset maxHeight variable
            maxHeigh = -100;

            // Do dmg according to player fall distance
            if (fallDistance >= 8 && fallDistance < 12 )
            {
                DamagePlayer(20);
            }
            else if(fallDistance >= 12 && fallDistance < 16)
            {
                DamagePlayer(40);
            }
            else if (fallDistance >= 16 && fallDistance < 20)
            {
                DamagePlayer(100);
            }
            else if (fallDistance >= 20 && fallDistance < 30)
            {
                DamagePlayer(400);
            }
            // Instant death
            else if (fallDistance >= 30)
            {
                DamagePlayer(-1);
            }
        }
    }

    // Damage player, if dmg value equals -1 is insta death
    public void DamagePlayer(int dmg)
    {
        playerHp -= dmg;
        SoundManager.PlayerGetDamageSound();

        // Player died
        if (dmg == -1 || playerHp <= 0 )
        {
            hpBar.value = 0;
            player.SetActive(false);
            dieText.gameObject.SetActive(true);
            playerIsDed = true;
        }
    }

    void RegenerateHp()
    {
        // Increase regenerate hp counter if hp is below maxPlayerHp or regenerate player's hp
        if(playerHp < maxPlayerHp)
        {
            // Regenerate player's hp
            if(regenerateHpTimeCounter >= regenerateHpTime)
            {
                int hpLeft = maxPlayerHp - (playerHp + regenerateHpAmount);
                if (hpLeft >= 0) playerHp += regenerateHpAmount;
                else
                {
                    playerHp += regenerateHpAmount - hpLeft;
                }
                regenerateHpTimeCounter = 0;
            }

            // increase regenerate hp counter
            regenerateHpTimeCounter += Time.deltaTime;
        }
    }
}
