using UnityEngine;

public class CorruptController : EnemyControllerBase
{
    private Transform playerTransform;

    [Header("Chasing Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float layerSwitchDelay = 1.5f;
    private float lastLayerSwitchTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
