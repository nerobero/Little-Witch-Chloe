using UnityEngine;

/// <summary>
/// Placed at a scene position the player should be moved to for a cutscene beat. Wraps
/// PlayerMovement.TeleportTo/MoveTo as zero/one-float-argument methods, since a Signal
/// Receiver's UnityEvent can't serialize a Vector3 argument in the Inspector.
/// </summary>
public class PlayerMoveWaypoint : MonoBehaviour
{
    public void TeleportPlayerHere()
    {
        PlayerController.Instance.PlayerMove.TeleportTo(transform.position);
    }

    public void MovePlayerHere(float duration)
    {
        PlayerController.Instance.PlayerMove.MoveTo(transform.position, duration);
    }
}
