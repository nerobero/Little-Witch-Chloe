using UnityEngine;

public class TileCollider : MonoBehaviour
{
    private Vector3 lastPosition;

    public Vector3 LastPosition => lastPosition;

    private int targetPlayerLayer;
    public int TargetPlayerLayer => targetPlayerLayer;
    private readonly float minDotProduct = Mathf.Cos(45f * Mathf.Deg2Rad);

    public void OnCollisionEnter2D(Collision2D other)
    {
        Vector2 normal = other.GetContact(0).normal;

        float dot = Vector2.Dot(Vector2.down, normal);

        if(other.gameObject.layer == targetPlayerLayer)
        {
            if(dot < minDotProduct)
            {
                return;
            }
            
            lastPosition = other.gameObject.transform.position;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string myLayerName = LayerMask.LayerToName(gameObject.layer);
        string prefix = myLayerName.Replace("_Platform", "");

        string targetLayerName = prefix + "_Player";
        targetPlayerLayer = LayerMask.NameToLayer(targetLayerName);
    }
}
