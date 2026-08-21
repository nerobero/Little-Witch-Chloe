using UnityEngine;

public class BossLoading : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // only work if the collider has player layer
        if(LayerMask.LayerToName(other.gameObject.layer).Contains("Player"))
        {
            Debug.Log("Enter");
            UIManager.Instance.Get<TransLoadingHUD>().Show();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // only work if the collider has player layer
        if(LayerMask.LayerToName(other.gameObject.layer).Contains("Player"))
        {
            Debug.Log("Exit");
            UIManager.Instance.Get<TransLoadingHUD>().Hide();
        }
    }
}
