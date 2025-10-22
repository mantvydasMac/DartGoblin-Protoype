using UnityEngine;
using System.Collections;

public class EntangledBallPO  : MonoBehaviour, Resetable
{
    public Vector3 originalPosition { get; set; }


    Rigidbody2D rb;
    AudioSource audioSource;
    CircleCollider2D coll;
    
    private float airFriction = 8;
    private float teleportDelay = 2;

    private float teleportLaunchSpeed = 5;
    private int teleportLaunchExcludeLayerMask;

    private Vector3 originalScale;
    private float radius;
    private bool teleporting = false;

    private enum Mode {
        NONE,
        SHRINKING,
        EXPANDING
    }

    private Mode mode;

    private Vector2 velocity;
    private Vector3 scale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        rb.gravityScale = 0f;
        coll = GetComponent<CircleCollider2D>();
        radius = coll.radius * transform.lossyScale.x;

        int exclude = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("RoomBoundary"));
        teleportLaunchExcludeLayerMask = ~exclude;

        originalPosition = transform.position;
        originalScale = transform.localScale;
    }


    void FixedUpdate()
    {
        velocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, airFriction * Time.fixedDeltaTime);
        rb.linearVelocity = velocity;


        if(!teleporting && transform.position != originalPosition && rb.linearVelocity == Vector2.zero)
        {
            StartCoroutine(TeleportCoroutine());
        }
        else if(teleporting)
        {
            if(mode == Mode.SHRINKING)
            {
                scale = Vector3.MoveTowards(transform.localScale, Vector3.zero, 8*Time.fixedDeltaTime);
                transform.localScale = scale;
            }
            else if(mode == Mode.EXPANDING)
            {
                scale = Vector3.MoveTowards(transform.localScale, originalScale, 8*Time.fixedDeltaTime);
                transform.localScale = scale;
            }
        }
    }



    IEnumerator TeleportCoroutine() 
    {
        teleporting = true;
        mode = Mode.NONE;
        yield return new WaitForSeconds(teleportDelay);

        coll.enabled = false;

        mode = Mode.SHRINKING;

        yield return new WaitUntil(() => transform.localScale.magnitude <= 0.01f);
        transform.localScale = Vector3.zero;

        rb.linearVelocity = Vector2.zero;
        transform.position = originalPosition;

        Collider2D[] objectsInKickRange = Physics2D.OverlapCircleAll(transform.position, radius, teleportLaunchExcludeLayerMask);

        foreach(Collider2D obj in objectsInKickRange) 
        {
            Vector2 direction = (new Vector2(obj.gameObject.transform.position.x - transform.position.x, obj.gameObject.transform.position.y - transform.position.y)).normalized;

            obj.attachedRigidbody.linearVelocity = direction * teleportLaunchSpeed;
        }

        mode = Mode.EXPANDING;
        yield return new WaitUntil(() => (transform.localScale - originalScale).magnitude <= 0.01f);
        transform.localScale = originalScale;

        mode = Mode.NONE;
        coll.enabled = true;
        teleporting = false;
    }

    public void Reset()
    {
        StopAllCoroutines();
        transform.position = originalPosition;
        transform.localScale = originalScale;
        rb.linearVelocity = Vector2.zero;
        mode = Mode.NONE;
        coll.enabled = true;
        teleporting = false;

    }
}