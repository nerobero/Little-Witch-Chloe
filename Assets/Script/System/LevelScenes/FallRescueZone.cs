using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FallRescueZone : MonoBehaviour
{
    #region deprecated
    //[Header("Search")]
    //[SerializeField] private Collider2D targetGround;
    //[SerializeField] private LayerMask groundLayer;

    // [SerializeField] private float rayStartHeight = 6f;
    // [SerializeField] private float rayDistance = 15f;
    // [SerializeField] private float minGroundNormalY = 0.65f;
    // [SerializeField] private float playerYOffset = 0.8f;
    #endregion

    [Header("Setting")]
    [SerializeField] private List<TileCollider> tilemaps;
    [SerializeField] private float damageAmount = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        other.TryGetComponent<IDamageable>(out var damageable);
        foreach(TileCollider tilemap in tilemaps)
        {
            // only if the other collider's layer is same with the tilemap's layer(i.e. only if it is a player)
            if(other.gameObject.layer == tilemap.TargetPlayerLayer)
            {
                if(rb)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.position = tilemap.LastPosition;
                }
                else
                {
                    other.transform.position = tilemap.LastPosition;
                }

                damageable.TakeDamageHelper(null, damageAmount, Types.EElementType.None);

                return;
            }
        }

        damageable.TakeDamageHelper(null, damageAmount, Types.EElementType.None);
        other.TryGetComponent<EnemyControllerBase>(out var monster);
        if(rb)
        {
            rb.linearVelocity = Vector2.zero;
            monster.Fallen();
        }
        else
        {
            monster.Fallen();
        }


    #region deprecated
        // if (!LayerMask.LayerToName(other.gameObject.layer).EndsWith("Player"))
        //     return;

        // if (!TryFindRescuePosition(other.transform.position, out Vector2 rescuePosition))
        //     return;

        // Rigidbody2D rb = other.attachedRigidbody;

        // if (rb)
        // {
        //     rb.linearVelocity = Vector2.zero;
        //     rb.position = rescuePosition;
        // }
        // else
        // {
        //     other.transform.position = rescuePosition;
        // }
    #endregion
    }

    #region deprecated
    // private bool TryFindRescuePosition(Vector2 playerPosition, out Vector2 rescuePosition)
    // {
    //     rescuePosition = playerPosition;

    //     if (!targetGround)
    //         return false;

    //     // Do raycast from the falling point
    //     RaycastHit2D hit = Physics2D.Raycast(playerPosition, Vector2.up, rayDistance, groundLayer);

    //     if (hit.collider != null)
    //     {
    //         // If the normal vector points downward = if it has properly collided with the underside of the terrain.
    //         if (Vector2.Dot(hit.normal, Vector2.down) > 0.7f)
    //         {
    //             // Directly obtain the collider's topmost Y-axis coordinate.
    //             float topY = hit.collider.bounds.max.y;
                
    //             // Keep the X-coordinate where the ray hit, but instantly teleport the Y-coordinate to the very top
    //             // (Add a slight offset equal to the height beneath the feet so the character doesn't sink into the ground.)
    //             float characterHalfHeight = 0.5f; 
    //             rescuePosition = new Vector3(hit.point.x, topY + characterHalfHeight, 0f);
                
    //             return true;
    //         }
    //     }

    //     return false;
    // }
    #endregion
}
