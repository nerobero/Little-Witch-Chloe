using UnityEngine;

public class CorruptMovement : EnemyMovement
{
    [Header("Movement values")]
    [SerializeField] private LayerMask bgLayer;
    [SerializeField] private LayerMask fgLayer;


    private bool _isBackground = false; 
    public bool IsBackground => _isBackground;
    private int _characterLayer => gameObject.layer; 
    private int _bgLayerIndex => (int)Mathf.Log(bgLayer.value, 2);
    private int _fgLayerIndex => (int)Mathf.Log(fgLayer.value, 2);

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

    public override void MoveToTarget()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        speed *= 1.5f;
    }

    public void BlinkToOtherPlatform()
    {
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */

        //1. finding if there is any teleportable platform within the given radius 
        LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
        if (Physics2D.OverlapCircle(transform.position, 15.0f, layerParam) == null)
            return;

        //2. find the surface to get teleport to:
        float camHalfHeight = Camera.main.orthographicSize;
        float xOffset = _isBackground? 2.5f : -2.5f;
        Vector2 origin = new Vector2(rb.position.x + xOffset, rb.position.y); 
        RaycastHit2D hitresult = Physics2D.Raycast(origin + Vector2.up * camHalfHeight,
                Vector2.down, camHalfHeight * 2f, layerParam);
        if (hitresult.collider == null) return;

        //3. ignoring the colliders of the teleported ground.
        Physics2D.IgnoreLayerCollision(_characterLayer, _bgLayerIndex, !_isBackground);
        Physics2D.IgnoreLayerCollision(_characterLayer, _fgLayerIndex, _isBackground);

        //4. reposition the player character:
        rb.position = new Vector2(rb.position.x + xOffset, hitresult.point.y + 0.1f);

        //5. flip the _isBackground value:
        _isBackground = !_isBackground;
    }
}
