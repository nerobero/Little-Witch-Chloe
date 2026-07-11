using UnityEngine;

public class TileCollider : MonoBehaviour
{
    private Vector3 lastPosition;

    public Vector3 LastPosition => lastPosition;

    private int targetPlayerLayer;
    public int TargetPlayerLayer => targetPlayerLayer;

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.layer == targetPlayerLayer)
        {
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
