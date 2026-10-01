using Unity.VisualScripting;
using UnityEngine;

public class BlobShadowController : MonoBehaviour
{
    public GameObject shadow;
    public float offset;
    private BaseCharacterMovement movementComp;
    private SpriteRenderer shadowSR;

    [Header("size diff on the height")]
    public float maxHeight = 4f;                
    [Range(0.1f, 1f)]
    public float minScaleMultiplier = 0.4f;     
    private Vector3 _baseScale;

    private void Start()
    {
        movementComp = GetComponent<BaseCharacterMovement>();
        shadowSR = shadow.GetComponent<SpriteRenderer>();
        _baseScale = shadow.transform.localScale;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - offset);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 20f, movementComp.GetGroundMask());

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

            shadow.transform.localScale = _baseScale * multiplier;

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
