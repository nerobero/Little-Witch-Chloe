using UnityEngine;

public class CorruptMovement : EnemyMovement
{
    [SerializeField] private float speedIncFactor = 1f;
    private float _originalSpeed;
    
    protected override void Awake()
    {
        base.Awake();
        
        _originalSpeed = speed;
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
            speed *= speedIncFactor;
        
        base.MoveToTarget(target);
        // Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        // //SetMoveDirection(direction);
    }

    public override void StopChasing()
    {
        if(isChasing)
            speed = _originalSpeed;

        base.StopChasing();
    }

    public override void OnBlinkCallback()
    {
        BlinkToOtherPlatform();
    }

    public override void BlinkToOtherPlatform()
    {
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */

        //1. finding if there is any teleportable platform within the given radius 
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

        layerParam = _isBackground ? bgLayer : fgLayer;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, layerParam);
        if (hitresult.collider == null)
        {
            Debug.Log("Null");
            return;
        }            

        //3. flip the _isBackground value:
        _isBackground = !_isBackground;

        //4. ignoring the colliders of the teleported ground.
        if(_isBackground)
        {
            myCollider.includeLayers |= (1 << _bgLayerIndex);
            myCollider.includeLayers &= ~(1 << _fgLayerIndex);
            myCollider.excludeLayers |= (1 << _fgLayerIndex);
            myCollider.excludeLayers &= ~(1 << _bgLayerIndex);
        }
        else
        {
            myCollider.includeLayers |= (1 << _fgLayerIndex);
            myCollider.includeLayers &= ~(1 << _bgLayerIndex);
            myCollider.excludeLayers |= (1 << _bgLayerIndex);
            myCollider.excludeLayers &= ~(1 << _fgLayerIndex);
        }

        //5. reposition the player character:
        rb.position = new Vector2(hitresult.point.x, hitresult.point.y + 1.0f);

        // 6. Change the order layer
        ChangeOrderInLayer();

        base.BlinkToOtherPlatform();
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Blink Enemy");
    }

    /// <summary>
    /// Set the gameobject's orderInLayer -1 or 0 based on whether
    /// the character is in the background or not.
    /// </summary>
}
