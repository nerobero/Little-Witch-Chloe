using UnityEngine;

public class NormalBlinkStrategy : IBlinkStrategy
{
    private LayerMask _fgLayer;
    private LayerMask _bgLayer;

    private float _angle;

    private float camHalfHeight = Camera.main.orthographicSize;

    public NormalBlinkStrategy()
    {
        _fgLayer = 1 << LayerMask.NameToLayer("Foreground_Platform");
        _bgLayer = 1 << LayerMask.NameToLayer("Background_Platform");
        _angle = 30f;
    }

    public NormalBlinkStrategy(float angle)
    {
        _fgLayer = 1 << LayerMask.NameToLayer("Foreground_Platform");
        _bgLayer = 1 << LayerMask.NameToLayer("Background_Platform");
        _angle = angle;
    }

    public void SetUpStrategyParams(LayerMask fg, LayerMask bg, float angle)
    {
        _fgLayer = fg;
        _bgLayer = bg;
        _angle = angle;
    }

    public (bool, Vector2) ProcessTeleport(float alpha, bool isBackground, bool isFacingRight, Transform characOrigin, Transform platformOrigin = null)
    {
        Vector3 pos = characOrigin.position;
        //1. finding if there is any teleportable platform within the given radius 
        LayerMask layerParam = isBackground ? _fgLayer : _bgLayer;
        Collider2D collided = Physics2D.OverlapCircle(pos, alpha, layerParam);

        #if UNITY_EDITOR
        // debug draw
        Color drawColor = collided ? Color.green : Color.red;
        
        // Draw the circle line using drawLine.
        int segments = 30;
        float angle = 0f;

        for (int i = 0; i < segments; i++)
        {
            float angleRad1 = (angle * Mathf.Deg2Rad);
            float angleRad2 = ((angle + 360f / segments) * Mathf.Deg2Rad);

            Vector3 dir1 = new Vector3(Mathf.Cos(angleRad1), Mathf.Sin(angleRad1), 0);
            Vector3 dir2 = new Vector3(Mathf.Cos(angleRad2), Mathf.Sin(angleRad2), 0);

            Debug.DrawLine(pos + dir1 * alpha, pos + dir2 * alpha, drawColor, 10f);
            angle += 360f / segments;
        }
        #endif

        int currLayer = GetGroundLayer(pos, characOrigin.gameObject, isBackground);
        if (collided == null || collided.gameObject.layer == currLayer)
        {
            Debug.LogWarning("cannot teleport.");
            return (false, Vector2.zero);
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
        Vector2 nearestOnPlatform = collided.ClosestPoint(pos);
        Vector2 origin = new Vector2(pos.x + xOffset, nearestOnPlatform.y);
        RaycastHit2D hitresult = Physics2D.Raycast(origin + Vector2.up * camHalfHeight,
                Vector2.down, camHalfHeight * 2f, layerParam);


        #if UNITY_EDITOR
        Vector2 start = origin + Vector2.up * camHalfHeight;
        Vector2 direction = Vector2.down;
        float rayDistance = camHalfHeight * 2f;
        Vector3 endPoint = start + direction * rayDistance;

        Debug.DrawLine(start, endPoint, Color.gold, 10f);
            
        // Draw the cross line to hit position
        Debug.DrawLine(hitresult.point - Vector2.up * 0.2f, hitresult.point + Vector2.up * 0.2f, Color.yellow, 10f);
        Debug.DrawLine(hitresult.point - Vector2.right * 0.2f, hitresult.point + Vector2.right * 0.2f, Color.yellow, 10f);
        #endif

        if (hitresult.collider == null)
        {
            Debug.Log("Null");
            return (false, pos);
        }

        return (true, new Vector2(hitresult.point.x, hitresult.point.y + 1.0f));

    }

    /// <summary>
    /// Gets the index of the layermask that the player is currently standing on.
    /// </summary>
    /// <returns>the index of the current layermask the player is standing on</returns>
    private int GetGroundLayer(Vector3 origin, GameObject obj, bool isBackground)
    {
        // LayerMask layerParam = isBackground ? _bgLayer : _fgLayer;
        // Debug.Log("isBackground? "+ isBackground + "so current layer is " + LayerMask.LayerToName(layerParam));
        // RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1.0f, layerParam);

        // Vector2 direction = Vector2.down;
        // float distance = 1.0f;
        // Vector3 endPoint = origin + (Vector3)(direction * distance);

        // Debug.DrawLine(origin, endPoint, Color.purple, 10f);
        // Debug.Log("hit is null? "+ (hit.collider == null));
        // Debug.Log(hit.collider != null? hit.collider.gameObject : obj);

        // return hit.collider != null ? hit.collider.gameObject.layer : obj.layer;

        return isBackground ? _bgLayer : _fgLayer;
    }
}
