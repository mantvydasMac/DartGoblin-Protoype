using UnityEngine;
using System.Collections;

public class EntangledBallPO : MonoBehaviour
{
    Rigidbody2D rb;
    AudioSource audioSource;
    Collider2D coll;
    
    private float airFriction = 8;
    private float teleportDelay = 2;

    private Vector3 originalPosition;
    private Vector3 originalScale;
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
                scale = Vector3.MoveTowards(transform.localScale, Vector3.zero, 3*Time.fixedDeltaTime);
                transform.localScale = scale;
            }
            else if(mode == Mode.EXPANDING)
            {
                scale = Vector3.MoveTowards(transform.localScale, originalScale, 3*Time.fixedDeltaTime);
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

        Debug.Log("shrinking");
        yield return new WaitUntil(() => transform.localScale.magnitude <= 0.01f);
        transform.localScale = Vector3.zero;

        rb.linearVelocity = Vector2.zero;
        transform.position = originalPosition;


        mode = Mode.EXPANDING;
        Debug.Log("expanding");
        yield return new WaitUntil(() => (transform.localScale - originalScale).magnitude <= 0.01f);
        transform.localScale = originalScale;


        coll.enabled = true;
        teleporting = false;
    }
}