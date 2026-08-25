using UnityEngine;
using Types;
/// <summary>
/// Base class for any interactable items.
/// Utilizes the pool object manager to handle potential respawning
/// of the items after interaction.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class ItemBase : MonoBehaviour, IResetable
{
    protected ESpawnType spawnType;
    
    [SerializeField] protected LayerMask bgPlayerLayer;
    [SerializeField] protected LayerMask fgPlayeLayer;
    [SerializeField] protected bool isBackground;
    protected SpriteRenderer _spriteRender;

    /// <summary>
    /// Handles interaction logic. Can be overriden by the child classes.
    /// </summary>
    /// <param name="other">the object this item has collided with</param>
    /// <returns>true if the interaction has been processed successfully</returns>
    protected abstract bool OnInteract(Collider2D other);

    protected virtual void Start()
    {
        LevelManager.Instance.RegisterInstance(this);

        _spriteRender = GetComponent<SpriteRenderer>();

        int bgLayer = LayerMask.NameToLayer("Background_Platform");
        int fgLayer = LayerMask.NameToLayer("Foreground_Platform");
        string myLayer = "Interactables";

        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, _spriteRender.bounds.extents.y / 2f), Vector2.down, 10.0f, bgLayer | fgLayer);
        //Debug.DrawRay(transform.position - new Vector3(0, _spriteRender.bounds.extents.y / 2f), Vector2.down * 10f, new Color(1, 1, 0), 1000f);

        Debug.Log(hit.collider);

        string groundLayerName = LayerMask.LayerToName(hit.collider != null ? hit.collider.gameObject.layer : gameObject.layer);

        if (groundLayerName.Contains("_"))
        {
            // 1. Check the ground is Foreground or Background
            string groundPrefix = groundLayerName.Split('_')[0];
            isBackground = (groundPrefix == "Background");

            // 2. Request a layer number of the "player/enemy" (e.g., "floor") from the manager.
            // 예: 바닥이 Background_Platform이면, 나는 Background_Player 레이어 번호를 가져옴
            int nextMyLayer = LayerManager.Instance.GetLayer(isBackground, myLayer);
           
            gameObject.layer = nextMyLayer;
            
            Debug.Log($"{GetType().Name}: Because the layer of the platform is {groundLayerName}, change my layer as {LayerMask.LayerToName(nextMyLayer)}.");
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnInteract(collision)) 
            //PoolObjectManager.Instance.Return(spawnType, this.gameObject);
            ProcessCollection();
    }

    protected virtual void ProcessCollection()
    {
        gameObject.SetActive(false);
    }

    public virtual void ResetState()
    {
        //PoolObjectManager.Instance.Get(spawnType);
        gameObject.SetActive(true);
    }
}
