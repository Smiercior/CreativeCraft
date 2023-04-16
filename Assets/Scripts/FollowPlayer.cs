using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Transform sunLight;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Camera follow player
        Vector3 cameraToPlayerPosition = new Vector3();
        cameraToPlayerPosition.x = player.position.x;
        cameraToPlayerPosition.y = player.position.y + 7f;
        cameraToPlayerPosition.z = transform.position.z;
        transform.position = cameraToPlayerPosition;
    }
}
 