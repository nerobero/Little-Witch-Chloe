using UnityEngine;

// Changes the x offset of the player's blinking ability
// at runtime when collided. Should return back to normal 
[RequireComponent(typeof(Collider2D))]
public class TPXoffsetChanger : MonoBehaviour
{
    [SerializeField] private float degAngle = 0f;

    // Is the teleport limited to one point of the platform?
    // False by default.
    public bool IsLimitTeleport = false;

    private bool _hasInteracted = false;

    private IBlinkStrategy _strategy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var movementComp = collision.gameObject.GetComponent<PlayerMovement>();

        if (movementComp != null && !_hasInteracted)
        {
            // pass in different strategy classes for different cases of teleport calculation:
            if (IsLimitTeleport)
            {
                
            }
            else
            {
                
            }
        }
    }
}
