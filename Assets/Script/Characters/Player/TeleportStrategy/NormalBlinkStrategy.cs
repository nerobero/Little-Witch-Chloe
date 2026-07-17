using UnityEngine;

public class NormalBlinkStrategy : IBlinkStrategy
{
    private LayerMask _fgLayer;
    private LayerMask _bgLayer;

    private float _angle;

    private float camHalfHeight = Camera.main.orthographicSize;

    public NormalBlinkStrategy()
    {
        _fgLayer = LayerMask.NameToLayer("Foreground Platform");
        _bgLayer = LayerMask.NameToLayer("Background Platform");
        _angle = 30f;
    }

    public NormalBlinkStrategy(float angle)
    {
        _fgLayer = LayerMask.NameToLayer("Foreground Platform");
        _bgLayer = LayerMask.NameToLayer("Background Platform");
        _angle = angle;
    }

    public void SetUpStrategyParams(LayerMask fg, LayerMask bg, float angle)
    {
        _fgLayer = fg;
        _bgLayer = bg;
        _angle = angle;
    }

    public Vector2 ProcessTeleport(float alpha, bool isBackground, bool isFacingRight, Transform characOrigin, Transform platformOrigin = null)
    {
        //1. finding if there is any teleportable platform within the given radius 
        LayerMask layerParam = isBackground ? _fgLayer : _bgLayer;
        Collider2D collided = Physics2D.OverlapCircle(characOrigin.position, 15.0f, layerParam);
        int currLayer = GetGroundLayer(characOrigin, isBackground);
        if (collided == null || collided.gameObject.layer == currLayer)
        {
            Debug.LogWarning("cannot teleport.");
            return Vector2.zero;
        }

        float xOffset = Mathf.Cos(Mathf.Deg2Rad*_angle);
        //2. find the surface to get teleport to:
        // 2a. flipping the xOffset based on the character's move direction
        // and whether the character is in the background or not:
        if (isFacingRight)
            xOffset = isBackground ? xOffset : -xOffset;
        else
            xOffset = isBackground ? -xOffset : xOffset;

        // 2b. using raycast to determine where on the surface the character can 'blink' to:
        // the vertical search band is centered on the nearest point on the target platform
        // (rather than the character's current position) so that blinking while airborne
        // (jumping/falling, where the character's y drifts) still lands near that platform
        // instead of missing it or, for composite tilemap colliders, landing anywhere along
        // the whole merged platform layer.
        Vector2 nearestOnPlatform = collided.ClosestPoint(characOrigin.position);
        Vector2 origin = new Vector2(characOrigin.position.x + xOffset, nearestOnPlatform.y);
        RaycastHit2D hitresult = Physics2D.Raycast(origin + Vector2.up * camHalfHeight,
                Vector2.down, camHalfHeight * 2f, layerParam);
        if (hitresult.collider == null)
        {
            Debug.Log("Null");
            return Vector2.zero;
        }

        return new Vector2(hitresult.point.x, hitresult.point.y + 1.0f);

    }

    /// <summary>
    /// Gets the index of the layermask that the player is currently standing on.
    /// </summary>
    /// <returns>the index of the current layermask the player is standing on</returns>
    private int GetGroundLayer(Transform origin, bool isBackground)
    {
        LayerMask layerParam = isBackground ? _bgLayer : _fgLayer;
        RaycastHit2D hit = Physics2D.Raycast(origin.position, Vector2.down, 1.0f, layerParam);

        return hit.collider != null ? hit.collider.gameObject.layer : origin.gameObject.layer;
    }
}
