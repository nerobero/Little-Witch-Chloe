using UnityEngine;

/// <summary>
/// Base class for any moving obstacles that can damage the characters,
/// including the player and the NPCs.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MovingObstacleBase : MonoBehaviour
{
    protected Rigidbody2D _objRB;
    //protected bool isBackground = false;
    protected SpriteRenderer _spriteRender;
    protected PolygonCollider2D myCollider;
    
    [Header("Movement")]
    protected float moveDir;
    [SerializeField] protected float speed;
    [SerializeField] protected bool isBackground;

    [Header("MoveingObstacle")]
    [SerializeField] protected float damageAmount;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask enemyLayer;
    protected int playerLayerIndex => (int)Mathf.Log(playerLayer.value , 2);
    protected int enmeyLayerIndex => (int)Mathf.Log(enemyLayer.value , 2);

    [SerializeField] protected LayerMask bgLayer;
    [SerializeField] protected LayerMask fgLayer;
    protected int _bgLayerIndex => (int)Mathf.Log(bgLayer.value, 2);
    protected int _fgLayerIndex => (int)Mathf.Log(fgLayer.value, 2);

    private void Awake()
    {
        _objRB = GetComponent<Rigidbody2D>();
        _spriteRender = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<PolygonCollider2D>();
    }

    protected void Start()
    {
        int orderInLayer = isBackground ? 0 : 2;
        _spriteRender.sortingOrder = orderInLayer;
        SetMoveDirection(1);

         if(isBackground)
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
    }

    protected void FixedUpdate()
    {
        _objRB.linearVelocity = new Vector2(moveDir * speed, _objRB.linearVelocity.y);
    }

    protected virtual void SetOrderLayer()
    {
        int orderInLayer = PlayerController.Instance.PlayerMove.OrderInLayer + 1;
        _spriteRender.sortingOrder = orderInLayer;
    }

    protected void SetMoveDirection(float dir)
    {
        moveDir = dir;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        int otherLayer = other.gameObject.layer;


        if(otherLayer != playerLayerIndex && otherLayer != enmeyLayerIndex)
        {
            return;
        }

        var move = other.GetComponent<PlayerMovement>();
        bool isOtherBackground = false;
        if(move == null)
        {
            var otherMove = other.GetComponent<EnemyMovement>();
            if(otherMove == null)
            {
                return;
            }

            isOtherBackground = otherMove.IsBackground;
        }
        else
        {
            isOtherBackground = move.IsBackground;
        }

        if(isOtherBackground != isBackground)
        {
            return;
        }

        var Stat = other.GetComponent<StatManager>();

        Stat.TakeDamage(gameObject, damageAmount, Types.EElementType.None);
    }
}
