using Unity.VisualScripting;
using UnityEngine;

public class BlobShadowController : MonoBehaviour
{
    public GameObject shadow;
    //public RaycastHit hit;
    public float offset;
    private BaseCharacterMovement movementComp;

    [Header("size diff on the height")]
    public float maxHeight = 4f;                
    [Range(0.1f, 1f)]
    public float minScaleMultiplier = 0.4f;     
    private Vector3 _baseScale;

    private void Start()
    {
        movementComp = GetComponent<BaseCharacterMovement>();
        _baseScale = shadow.transform.localScale;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - offset);
        //Ray downRay = new Ray(new Vector3(this.transform.position.x, this.transform.position.y - offset, this.transform.position.z), -Vector3.up);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 20f, movementComp.GetGroundMask());
        //Vector3 hitPosition = hit.point;
        //shadow.transform.position = hitPosition;

        if (hit.collider)
        {
            shadow.SetActive(true);
            shadow.transform.position = new Vector3(origin.x, hit.point.y, shadow.transform.position.z);

            float height = origin.y - hit.point.y;            
            float t = Mathf.Clamp01(height / maxHeight);      
            float multiplier = Mathf.Lerp(1f, minScaleMultiplier, t);

            shadow.transform.localScale = _baseScale * multiplier;
        }
        else
        {
            shadow.SetActive(false);
        }
    }
}
