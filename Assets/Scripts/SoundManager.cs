using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Sound variables
    public AudioClip backgroundMusic;
    public static AudioClip hittingStoneAndOreSound;
    public static AudioClip hittingGrassAndDirtSound;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = true;
            audioSource.clip = backgroundMusic;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
    }

    void Update()
    {
        // Stop sound if player no longer pressing left mouse button
        if (Input.GetMouseButton(0) != true)
        {
            AudioSource audioSource = GameObject.FindObjectOfType<Camera>().GetComponents<AudioSource>()[1];
            if(audioSource != null) audioSource.Stop();
        }
    }

    // Play sound when player collect drop item
    public static void PlayGetItemSound()
    {
        AudioClip getSound = Resources.Load<AudioClip>("Sounds/GetSound");
        AudioSource audioSource = GameObject.FindObjectOfType<Camera>().GetComponents<AudioSource>()[0];
        if (getSound != null && audioSource != null) audioSource.PlayOneShot(getSound);
    }

    // Play sound when player hit tile, sound depends of tile type
    public static void PlayTileHittingSound(bool state, string tileName)
    {
        hittingStoneAndOreSound = Resources.Load<AudioClip>("Sounds/HittingStoneAndOreSound");
        hittingGrassAndDirtSound = Resources.Load<AudioClip>("Sounds/HittingGrassAndDirtSound");

        AudioSource audioSource = GameObject.FindObjectOfType<Camera>().GetComponents<AudioSource>()[1];

        if (hittingStoneAndOreSound != null && hittingGrassAndDirtSound != null && audioSource != null)
        {
            // If we have sound for tile
            if (tileSoundType.ContainsKey(tileName))
            {
                if (tileSoundType[tileName] == 1) audioSource.clip = hittingStoneAndOreSound;
                else if (tileSoundType[tileName] == 2) audioSource.clip = hittingGrassAndDirtSound;
            }
            else state = false;

            // Play sound
            if (state)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
            // Stop sound
            else if (!state)
            {
                audioSource.Stop();
            }
        }
    }

    public static void PlayerGetDamageSound()
    {
        AudioClip getDamageSound = Resources.Load<AudioClip>("Sounds/GetDamageSound");
        AudioSource audioSource = GameObject.FindObjectOfType<Camera>().GetComponents<AudioSource>()[0];
        if (getDamageSound != null && audioSource != null) audioSource.PlayOneShot(getDamageSound);
    }

    public static void EnemyGetDamage(string enemyName)
    {
        AudioSource audioSource = GameObject.FindObjectOfType<Camera>().GetComponents<AudioSource>()[0];
        if (enemyName.Contains("Slimy"))
        {
            AudioClip enemyGetDamageSound = Resources.Load<AudioClip>("Sounds/SlimyGetDamageSound");
            if (enemyGetDamageSound != null && audioSource != null) audioSource.PlayOneShot(enemyGetDamageSound);
        }
    }

    public static void EnemyDied(string enemyName)
    {
        AudioSource audioSource = GameObject.FindObjectOfType<Camera>().GetComponents<AudioSource>()[0];
        if (enemyName.Contains("Slimy"))
        {
            AudioClip enemyDiedSound = Resources.Load<AudioClip>("Sounds/SlimyDiedSound");
            if (enemyDiedSound != null && audioSource != null) audioSource.PlayOneShot(enemyDiedSound);
        }
    }

    public static Dictionary<string, int> tileSoundType = new Dictionary<string, int>()
    {
        { "Stone", 1 },
        { "CoalOre", 1},
        { "CopperOre", 1},
        { "IronOre", 1},
        { "SilverOre", 1},
        { "GoldOre", 1},
        { "MagnesiumOre", 1},
        { "DiamondOre", 1},
        { "UranOre", 1},
        { "Grass", 2},
        { "Dirt", 2}
    };
}
