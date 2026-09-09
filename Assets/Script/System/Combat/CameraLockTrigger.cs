using UnityEngine;

public class CameraLockTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // only work if the collider has player layer
        if(LayerMask.LayerToName(other.gameObject.layer).Contains("Player"))
        {
            CameraController camController = Camera.main.GetComponent<CameraController>();

            camController.LockCamera(true);
            camController.UpdateCameraSize(10f);
            camController.gameObject.transform.position = 
            new Vector3(
                gameObject.transform.position.x,
                gameObject.transform.position.y,
                camController.offset.z
            );

            other.gameObject.transform.position = gameObject.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Exit: {other.tag}");
        // if the collider is main camera
        if(LayerMask.LayerToName(other.gameObject.layer).Contains("Player"))
        {
            CameraController camController = Camera.main.GetComponent<CameraController>();

            camController.LockCamera(false);
            camController.UpdateCameraSizeToOriginal();
        }
    }
}
