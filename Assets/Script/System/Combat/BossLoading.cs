using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossLoading : MonoBehaviour
{
    [Header("World Settings")]
    public GameObject InvisibleWall;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // only work if the collider has player layer
        if(LayerMask.LayerToName(other.gameObject.layer).Contains("Player"))
        {
            Debug.Log("Enter");
            UIManager.Instance.Get<TransLoadingHUD>().Show();
            other.GetComponent<PlayerMovement>().ForceToBeOnForeground();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // only work if the collider has player layer
        if(LayerMask.LayerToName(other.gameObject.layer).Contains("Player"))
        {
            Debug.Log("Exit");
            UIManager.Instance.Get<TransLoadingHUD>().Hide();

            InvisibleWall.transform.position = 
                new Vector3(
                    gameObject.transform.position.x,
                    other.transform.position.y + 2.0f, 
                    0
                );
        }
    }

}
