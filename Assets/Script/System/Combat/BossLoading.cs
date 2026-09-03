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
            UIManager.Instance.Get<TransLoadingHUD>()?.Hide();

            StartCoroutine(CreateInvisibleWall(1.0f, other.transform.position));
        }
    }

    IEnumerator CreateInvisibleWall(float time, Vector3 position)
    {
        float elapsed = 0.0f;

        while(elapsed <= time)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        InvisibleWall.transform.position = 
                new Vector3(
                    transform.position.x + 100f,
                    position.y, 
                    0
                );
    }
}
