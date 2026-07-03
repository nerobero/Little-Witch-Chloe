using UnityEngine;

// Changes the x offset of the player's blinking ability
// at runtime when collided. Should return back to normal 
[RequireComponent(typeof(Collider2D))]
public class TPXoffsetChanger : MonoBehaviour
{
    [SerializeField] private float degAngle = 0f;

    private bool _hasInteracted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var movementComp = collision.gameObject.GetComponent<PlayerMovement>();

        if (movementComp != null && !_hasInteracted)
        {
            // calculate the cosine 
        }
    }
}
