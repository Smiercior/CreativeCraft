using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    // Enemy stats
    public float hp;
    public int damage;
    public int knockback;
    public int speed = 2;
    public int jumpHeight = 5;

    // Variables for enemy AI
    public Rigidbody2D enemyRigidbody2D;
    public Rigidbody2D player;
    Tilemap tilemap;
    float maxPlayerEnemyDistance = 50;
    int direction = 0;  
    bool isGrounded = false;
    bool wasHitted = false;

    // Variables form damage enemy
    public GameObject hpObject;
    float maxHp;
    float untouchableCounter = 0;
    float untouchableTime = 0.5f;

    // Enemy direction
    public int actualDirection { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        tilemap = GameObject.FindObjectOfType<Tilemap>();
        maxHp = hp;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfDestroy();
        SlimyAI();

        // Update enemy direction
        if (direction != 0) actualDirection = direction;

        // Count enemy untouchable frames, if enemy was hitted ( prevent multiple hits on one player weapon swing )
        if(wasHitted)
        {
            untouchableCounter += Time.deltaTime;
            if(untouchableCounter >= untouchableTime)
            {
                wasHitted = false;
                untouchableCounter = 0;
            }
        }

        // Update enemy hpBar according to hp
        hpObject.transform.localScale = new Vector3(hp / maxHp, hpObject.transform.localScale.y,0);
    }

    // Check if enemy is on the ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Tilemap") isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Tilemap") isGrounded = false;
    }

    // Enemy get damage
    public void GetDamage(int dmg, float knockback)
    {
        // If enemy was hitted by player, give enemy some time of untouchable
        if(!wasHitted)
        {
            // Lower enemy hp
            hp -= dmg;

            // Applay knockback according to player direction
            enemyRigidbody2D.AddForce(new Vector2(knockback * PlayerManager.playerDirection * 4, 2), ForceMode2D.Impulse);

            // If enemy has 0 hp, destroy enemy object
            if (hp <= 0)
            {
                SoundManager.EnemyDied(this.gameObject.name);
                Destroy(this.gameObject);
            }

            // Enemy hitted sound
            SoundManager.EnemyGetDamage(this.gameObject.name);

            // Enemy was hitted
            wasHitted = true;
        }
    }

    // Check if player is far away, if it's destroy that object
    public void CheckIfDestroy()
    {
        float XF = Mathf.Abs(this.gameObject.transform.position.x - player.position.x);
        float YF = Mathf.Abs(this.gameObject.transform.position.y - player.position.y);
        float playerEnemyDistance = Mathf.Sqrt(XF * XF + YF * YF);
        if (playerEnemyDistance > maxPlayerEnemyDistance) Destroy(this.gameObject);
    }

    // AI of Slimy enemy
    public void SlimyAI()
    {
        // If player is on the right 
        if (player.position.x > this.transform.position.x) direction = 1;
        // If player is on the left
        else if (player.position.x < transform.position.x) direction = -1;
        else direction = 0;

        // Get tile in front of enemy
        Tile nextToEnemyTile;
        nextToEnemyTile = tilemap.GetTile<Tile>(tilemap.WorldToCell(new Vector3Int((int)this.transform.position.x + actualDirection, (int)this.transform.position.y, 0)));

        // Follow player only if enemy have free space
        if(nextToEnemyTile == null)
        {
            transform.Translate(new Vector3(speed * Time.deltaTime * direction, 0, 0), Space.World);
        }

        // If there is tile next to enemy, try jump ( only can jump if is on the ground )
        if (nextToEnemyTile != null && isGrounded) enemyRigidbody2D.velocity = new Vector2(0, jumpHeight); //enemyRigidbody2D.AddForce(Vector2.up * Time.deltaTime * 100, ForceMode2D.Impulse);
    }
}
