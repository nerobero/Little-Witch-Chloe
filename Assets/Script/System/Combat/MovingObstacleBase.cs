using UnityEngine;

/// <summary>
/// Base class for any moving obstacles that can damage the characters,
/// including the player and the NPCs.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MovingObstacleBase : MonoBehaviour
{
    private Rigidbody2D _objRB;

    private void Awake()
    {
        _objRB = GetComponent<Rigidbody2D>();
    }
}
