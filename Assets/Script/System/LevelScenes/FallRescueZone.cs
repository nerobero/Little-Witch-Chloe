using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FallRescueZone : MonoBehaviour
{
    [Header("Search")]
    [SerializeField] private Collider2D targetGround;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float rayStartHeight = 6f;
    [SerializeField] private float rayDistance = 15f;
    [SerializeField] private float minGroundNormalY = 0.65f;
    [SerializeField] private float playerYOffset = 0.8f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.tag.EndsWith("Player"))
            return;

        if (!TryFindRescuePosition(other.transform.position, out Vector2 rescuePosition))
            return;

        Rigidbody2D rb = other.attachedRigidbody;

        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = rescuePosition;
        }
        else
        {
            other.transform.position = rescuePosition;
        }
    }

    private bool TryFindRescuePosition(Vector2 playerPosition, out Vector2 rescuePosition)
    {
        rescuePosition = playerPosition;

        if (!targetGround)
            return false;

        // Do raycast from the falling point
        RaycastHit2D hit = Physics2D.Raycast(playerPosition, Vector2.up, rayDistance, groundLayer);

        if (hit.collider != null)
        {
            // If the normal vector points downward = if it has properly collided with the underside of the terrain.
            if (Vector2.Dot(hit.normal, Vector2.down) > 0.7f)
            {
                // Directly obtain the collider's topmost Y-axis coordinate.
                float topY = hit.collider.bounds.max.y;
                
                // Keep the X-coordinate where the ray hit, but instantly teleport the Y-coordinate to the very top
                // (Add a slight offset equal to the height beneath the feet so the character doesn't sink into the ground.)
                float characterHalfHeight = 0.5f; 
                rescuePosition = new Vector3(hit.point.x, topY + characterHalfHeight, 0f);
                
                return true;
            }
        }

        return false;
    }
}
