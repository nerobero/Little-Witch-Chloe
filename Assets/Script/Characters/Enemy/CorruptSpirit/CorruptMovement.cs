using UnityEngine;

public class CorruptMovement : EnemyMovement
{

    void Start()
    {
        
    }

    public override void Think()
    {
        // Check Z axis 

        base.Think();
        // float offsetX = MoveDir * Random.Range(minDistance, maxDistance);
        // float offsetY = MoveDir * Random.Range(minDistance, maxDistance);
        
        // // Make target position in 2~3 grids on the x-y axis from its spawn point.
        // Vector2 targetPosition = spawnPosition + new Vector2(offsetX, offsetY);

        // SetMoveDirection(MoveDir);


    }

    public override void MoveToTarget(Vector2 target)
    {
        if(!isChasing)
            speed *= 1.5f;
        
        base.MoveToTarget(target);
        // Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        // //SetMoveDirection(direction);
    }

    public override void StopChasing()
    {
        if(isChasing)
            speed /= 1.5f;

        base.StopChasing();
    }

    public override void BlinkToOtherPlatform()
    {
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */

        //1. finding if there is any teleportable platform within the given radius 
        base.BlinkToOtherPlatform();
        LayerMask layerParam = _isBackground ? fgLayer : bgLayer;
        Collider2D collided = Physics2D.OverlapCircle(transform.position, 15.0f, layerParam);
        int currLayer = GetGroundLayer();
        if (collided == null || collided.gameObject.layer == currLayer)
        {
            Debug.LogWarning("cannot teleport.");
            return;
        }

        //2. find the surface to get teleport to:
        float camHalfHeight = Camera.main.orthographicSize;
        float xOffset = 0.89f * 2f;

        if (_animController.IsFacingRight)
            xOffset = _isBackground ? -xOffset : xOffset;
        else
            xOffset = _isBackground ? xOffset : -xOffset;

        
        Vector2 origin = new Vector2(rb.position.x + xOffset, rb.position.y);
        Debug.DrawRay(origin + Vector2.up * camHalfHeight, Vector2.down * camHalfHeight * 2f, new Color(0, 0, 1), 2.0f);
        RaycastHit2D hitresult = Physics2D.Raycast(origin + Vector2.up * camHalfHeight,
                Vector2.down, camHalfHeight * 2f, layerParam);
        if (hitresult.collider == null)
        {
            Debug.Log("Null");
            return;
        }            

        //3. flip the _isBackground value:
        _isBackground = !_isBackground;

        //4. ignoring the colliders of the teleported ground.
        Physics2D.IgnoreLayerCollision(platformLayer, _bgLayerIndex, !_isBackground);
        Physics2D.IgnoreLayerCollision(platformLayer, _fgLayerIndex, _isBackground);

        //5. reposition the player character:
        rb.position = new Vector2(hitresult.point.x, hitresult.point.y + 1.0f);

        orderInLayer = _isBackground? -1 : 0;
        _spriteRender.sortingOrder = orderInLayer;
    }
}
