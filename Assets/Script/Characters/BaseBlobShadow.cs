using Unity.VisualScripting;
using UnityEngine;

public class BaseBlobShadow : MonoBehaviour
{
    public GameObject shadow;
    protected BaseCharacterMovement movementComp;
    public float offset;
    public SpriteRenderer shadowSR { get; private set; }     
    [SerializeField] protected Vector3 _baseScale;
    protected Vector3 _baseLocalScale;

    protected float originalWidth;
    protected float originalHeight;

    [Header("size diff on the height")]
    public float maxHeight = 4f;                
    [Range(0.1f, 1f)]
    public float minScaleMultiplier = 0.4f;


    protected virtual void Awake()
    {
        Collider2D collider = GetComponent<Collider2D>();
        SpriteRenderer mainSR = GetComponent<SpriteRenderer>();

        originalWidth = Mathf.Min(collider.bounds.size.x, mainSR.sprite.rect.width / mainSR.sprite.pixelsPerUnit);
        originalHeight = Mathf.Min(collider.bounds.size.y, mainSR.sprite.rect.height / mainSR.sprite.pixelsPerUnit);
    }

    protected void OnBecameVisible()
    {
        enabled = true;
    }

    protected void OnBecameInvisible()
    {
        enabled = false;
    }

    protected virtual void OnEnable()
    {
        shadow = PoolObjectManager.Instance.GetShadowObject();

        movementComp = GetComponent<BaseCharacterMovement>();

        if(shadow != null)
        {
            Vector3 targetScale = shadow.transform.localScale;
            if(transform.localScale.x != 0)
            {
                targetScale.x = originalWidth;
            }
            targetScale.y = 0.2f;
            //shadow.transform.localScale = _baseScale;
            shadow.transform.localScale = targetScale;
            shadow.transform.parent = gameObject.transform;
            shadow.transform.localPosition = new Vector3(0f, -(originalHeight / 2) / Mathf.Abs(transform.localScale.y), 0f);

            shadow.SetActive(true);
            _baseLocalScale = shadow.transform.localScale;
            shadowSR = shadow.GetComponent<SpriteRenderer>();

            CalculateShadowPosition();
        }

        if(movementComp != null)
        {
            movementComp.onPlatformChanged += ChangeOrderInLayer;
        }
    }

    protected void OnDisable()
    {
        if(movementComp != null)
        {
            movementComp.onPlatformChanged -= ChangeOrderInLayer;
        }

        if(shadow != null)
        {
            PoolObjectManager.Instance.ReturnShadowObject(shadow);
        }
    }

    protected void ChangeOrderInLayer(int order)
    {
        shadowSR.sortingOrder = order;
    }

    protected virtual void CalculateShadowPosition()
    {
        LayerMask groundLayer = LayerMask.GetMask("Foreground_Platform") | LayerMask.GetMask("Background_Platform");

        if(movementComp != null)
        {
            groundLayer = movementComp.GetGroundMask();
        }

        //Vector2 origin = new Vector2(transform.position.x, transform.position.y - offset);
        Debug.Log($"{gameObject}: groundLayer({groundLayer})");
        Vector2 origin = new Vector2(transform.position.x, transform.position.y + originalHeight / 2f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 20f, groundLayer);

        if (hit.collider)
        {
            shadow.SetActive(true);
            shadow.transform.position = new Vector3(origin.x, hit.point.y, shadow.transform.position.z);

            Debug.DrawLine(origin, hit.point, Color.red);
            Debug.Log($"BlobShadowController: hit={hit.collider}");

            // Change the scale of the shadow.
            // height from the ground
            float distance = hit.distance;
            // 0: max height, 1: on the ground
            float scaleRatio = Mathf.Clamp01(1 - (distance / maxHeight));
            float multiplier = Mathf.Lerp(minScaleMultiplier, 1f, scaleRatio);

            shadow.transform.localScale = _baseLocalScale * multiplier;

            if(shadowSR != null)
            {
                Color newColor = shadowSR.color;
                newColor.a = scaleRatio * 0.6f;
                shadowSR.color = newColor;
            }
        }
        else
        {
            shadow.SetActive(false);
        }
    }
}
