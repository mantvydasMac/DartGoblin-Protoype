using UnityEngine;
using System.Collections;

public class CrystalBallPO : MonoBehaviour, IResetable
{
    public Vector3 originalPosition { get; set; }
    private Quaternion originalRotation;
    private float originalGravity;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Color defaultColor = new Color(1f, 1f, 1f, 1f);
    public Color kickedColor;

    public GameObject wallCrystalObject;

    private GameObject createdWallCrystal = null;

    CrystalBallKickable kickable;

    Vector2 currentVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        originalPosition = transform.position;
        originalGravity = rb.gravityScale;
        kickable = GetComponent<CrystalBallKickable>();
        sr = GetComponent<SpriteRenderer>();
        
    }

    void FixedUpdate()
    {
        currentVelocity = rb.linearVelocity;
        if(kickable.kicked)
        {
            sr.color = kickedColor;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (kickable.kicked && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Vector2 velocityAfterHit = rb.linearVelocity;
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 offset = new Vector2(0f, 0f);
            float colliderRadius = wallCrystalObject.GetComponent<CircleCollider2D>().radius * wallCrystalObject.transform.lossyScale.x;

            Debug.Log($"before: {currentVelocity} | after: {velocityAfterHit}");

            if(Mathf.Abs(velocityAfterHit.x) < 0.001f && Mathf.Abs(currentVelocity.x) > 0.001f)
            {
                if(currentVelocity.x > 0)
                {
                    rotation = Quaternion.Euler(0f, 0f, 90f);
                    offset = new Vector2(-colliderRadius, 0f);
                }
                else
                {
                    rotation = Quaternion.Euler(0f, 0f, -90f);
                    offset = new Vector2(colliderRadius, 0f);
                }
            }
            else if(Mathf.Abs(velocityAfterHit.y) < 0.001f && Mathf.Abs(currentVelocity.y) > 0.001f)
            {
                if(currentVelocity.y > 0)
                {
                    rotation = Quaternion.Euler(0f, 0f, 180f);
                    offset = new Vector2(0f, -colliderRadius);
                }
                else
                {
                    rotation = Quaternion.Euler(0f, 0f, 0f);
                    offset = new Vector2(0f, colliderRadius);
                }
            }

            

            disable();
            createdWallCrystal = Instantiate(wallCrystalObject, contactPoint + offset, rotation);
        }
    }

    void disable()
    {
        sr.enabled = false;
		GetComponent<Collider2D>().enabled = false;
		rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
    }

    void enable()
    {
        sr.enabled = true;
		GetComponent<Collider2D>().enabled = true;
        rb.gravityScale = originalGravity;
    }

    public void Reset()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        enable();
        kickable.kicked = false;
        sr.color = defaultColor;

        if(createdWallCrystal != null)
        {
            Destroy(createdWallCrystal);
            createdWallCrystal = null;
        }
    }
}