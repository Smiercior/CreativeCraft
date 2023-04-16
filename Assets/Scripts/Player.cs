using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerManager playerManager;
    public Rigidbody2D playerRigidbody2D;

    // Enemy hit player
    void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if(enemy != null)
        {
            // Damage player
            playerManager.DamagePlayer(enemy.damage);

            // Knockback player
            playerRigidbody2D.AddForce(new Vector2(enemy.knockback * enemy.actualDirection * 10, 5), ForceMode2D.Impulse);
        }   
    }
}
