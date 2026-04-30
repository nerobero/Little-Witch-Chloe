using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerController player;
    public float smoothTime = 0.2f;
    public Vector3 offset = new Vector3(0, 1.0f, -10.0f);
    private Vector3 velocity = Vector3.zero;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(player != null)
        {
            Vector3 playerPosition = player.transform.position + offset;

            transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref velocity, smoothTime);
        }
    }
}
