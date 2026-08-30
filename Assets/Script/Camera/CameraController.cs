using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerController player;
    public float smoothTime = 0.2f;
    public Vector3 offset = new Vector3(0, 1.0f, -10.0f);
    private Vector3 velocity = Vector3.zero;
    private bool isCameraLocked = false;
    private Camera cam;
    private float originalSize;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        cam = GetComponent<Camera>();

        originalSize = cam.orthographicSize;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(isCameraLocked == false)
        {
            if(player != null)
            {
                Vector3 playerPosition = player.transform.position + offset;

                transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref velocity, smoothTime);
            }
        }
    }

    public void LockCamera(bool isLocked)
    {
        isCameraLocked = isLocked;
    }

    public void UpdateCameraSize(float size = 0)
    {
        cam.orthographicSize = size;
    }

    public void UpdateCameraSizeToOriginal()
    {
        cam.orthographicSize = originalSize;
    }
}
