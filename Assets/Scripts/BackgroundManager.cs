using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public Camera mainCamera;
    public Rigidbody2D playerPosition;
    int caveBackgroundHeight = 50;
    public Sprite background;
    public Sprite caveBackground;
    public Image backgroundImage;

    // Update is called once per frame
    void Update()
    {
        // If player is under certain level, change background to cave background, else change to normal background
        if (playerPosition.position.y < caveBackgroundHeight) backgroundImage.sprite = caveBackground;
        else if (playerPosition.position.y >= caveBackgroundHeight) backgroundImage.sprite = background;
    }
}
