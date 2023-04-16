using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    public GameObject hand;

    // Damage enemy
    void OnTriggerEnter2D(Collider2D enemy)
    {
        // Get enemy class from enemy object
        Enemy hittedEnemy = enemy.gameObject.GetComponent<Enemy>();

        // Only can damage enemy if sword is in movement
        if (hand.transform.rotation != Quaternion.Euler(0, 0, 93.62f) && hand.transform.rotation != Quaternion.Euler(0, 0, -93.62f))
        {    
            if (hittedEnemy != null)
            {
                // Damage and knockback enemy according to weapon
                int weaponItemId = EqManager.GetSelectedItemId();
                hittedEnemy.GetDamage(Item.swordDamage[weaponItemId], Item.swordKnockback[weaponItemId]);
            }
        }   
    }
}
