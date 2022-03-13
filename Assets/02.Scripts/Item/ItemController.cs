using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public Item item;
    public int num = 1;

    public bool pickUpEnable = false;
    public bool isPickedUp = false;

    
    Rigidbody2D rb;
    BoxCollider2D trigger;
    BoxCollider2D col;
    Transform rendererTransform;
    public bool doFloatingEffect;
    public float rotateSpeed = 10f;
    public float popForce = 2f;
    public float floatingSpeed = 5f;
    public float floatingHeight = 0.02f;
    public LayerMask groundLayer;

    // effect
    private float pickUpTimer = 1f;
    private float elapsedTime;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trigger = GetComponent<BoxCollider2D>();
        col = transform.Find("Collider").GetComponent<BoxCollider2D>();
        rendererTransform = transform.Find("Renderer");
    }
    private void OnEnable()
    {
        StartCoroutine(E_ShowEffect());
    }
    private void Update()
    {
        if (doFloatingEffect)
            FloatingEffect();
    }
    public virtual void OnUseEvent()
    {

    }
    IEnumerator E_ShowEffect()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(0f, popForce), ForceMode2D.Impulse);
        while (doFloatingEffect == false)
        {
            doFloatingEffect = Physics2D.OverlapBox(new Vector2(rb.position.x,rb.position.y - (trigger.size.y /2 + 0.04f) * transform.lossyScale.y) , 
                                              new Vector2(trigger.size.x * transform.lossyScale.x, 0.02f * transform.lossyScale.y) , 0,groundLayer);
            rendererTransform.Rotate(new Vector3(0f, 0f, rotateSpeed));
            yield return null;
        }
        rendererTransform.eulerAngles = new Vector3(0f, 0f, 0f);
        pickUpEnable = true;
    }
    
    void FloatingEffect()
    {
        rendererTransform.localPosition = new Vector3(0f, floatingHeight* Mathf.Sin(floatingSpeed * elapsedTime), 0f);
        elapsedTime += Time.deltaTime;
    }
    public void PickUp(Player player)
    {
        if (pickUpEnable == false || isPickedUp) return;
        isPickedUp = true;
        InventoryView.instance.GetItemsViewByItemType(item.type).AddItem(item, num);
        StartCoroutine(E_PickUpEffect(player));

    }
    IEnumerator E_PickUpEffect(Player player)
    {
        col.enabled = false;
        // pop 
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(0f, popForce), ForceMode2D.Impulse);
        yield return new WaitForEndOfFrame();

        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        bool isReachedToPlayer = false;
        float fadeAlpha = 1f;
        Color fadeColor = rendererTransform.GetComponent<SpriteRenderer>().color;
        
        while (pickUpTimer > 0 && isReachedToPlayer == false)
        {
            if ((Mathf.Abs(player.transform.position.x - rb.position.x) < 0.1f) &&
                ((player.transform.position.y - rb.position.y) < 0))
            {
                isReachedToPlayer = true;
            }
            Vector2 distance = (Vector2)player.transform.position - rb.position;
            Vector2 moveVelocity = distance;
            if (distance.magnitude < distance.normalized.magnitude)
                moveVelocity = distance.normalized;

            rb.position += moveVelocity * Time.deltaTime;
            
            fadeAlpha -= Time.deltaTime;
            fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fadeAlpha);
            rendererTransform.GetComponent<SpriteRenderer>().color = fadeColor;

            pickUpTimer -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y -(GetComponent<BoxCollider2D>().size.y / 2 + 0.04f) * transform.lossyScale.y) ,
                                              new Vector2(GetComponent<BoxCollider2D>().size.x * transform.lossyScale.x, 0.02f * transform.lossyScale.y) );
    }
}
