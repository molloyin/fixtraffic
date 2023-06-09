using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
//controls overview camera

public class CameraOverviewController : MonoBehaviour
{
    
    public float panSpeed = 20.00f;
    public float panBorderThickness = 10;
    public float scrollSpeed = 20f;
    public bool musicOn;
    [SerializeField] public KeyCode muteKey;

    public AudioSource track1, track2;

    // Start is called before the first frame update
    void Start()
    {
        musicOn = true;
        muteKey = KeyCode.M;

    }

    // Update is called once per frame
    void Update()
    {
        //mappings here to avoid conflict with car controls
        Vector3 loc = transform.position;
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness) 
        {
            loc.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
        {
            loc.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness)
        {
            loc.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            loc.x += panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        loc.y += -scroll * scrollSpeed * 200 * Time.deltaTime;
        transform.position = loc;

        if (isActiveAndEnabled)
        {
                MuteToggle(); 
        }
    }


    public void MuteToggle()
    {
        if (Input.GetKeyDown(muteKey))
        {
            track1.mute = !track1.mute;
        }
    }

}
