using System.Collections.Generic;
using UnityEngine;
using Types;

public class BackgroundController : MonoBehaviour 
{
    private Vector2 startPos;
    public Camera cam;
    public GameObject parallaxPoint;
    public bool enableXParallax = false;
    public bool enableYParallax = false;
    public float parallexEffectX; // 0.15 is good for the tree backgrounds.
    public float parallexEffectY;

    void Start()
    {
        cam = Camera.main;
        startPos = transform.position;
    }

    void FixedUpdate()
    {   
        //fallback to cam pos in worldspace if parallaxPoint is not set
        Vector2 camPos;
        if (parallaxPoint != null)
        {
            camPos = cam.transform.position - parallaxPoint.transform.position;
        } else 
        {
            camPos = cam.transform.position;
        }
        
        Vector2 distance = Vector2.Scale(camPos, new Vector2(parallexEffectX, parallexEffectY));
        
        transform.position = new Vector3(enableXParallax ? startPos.x + distance.x : startPos.x , enableYParallax ? startPos.y + distance.y : startPos.y , transform.position.z);

    }
}
