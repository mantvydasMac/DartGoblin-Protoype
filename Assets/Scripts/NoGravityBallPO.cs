using UnityEngine;
using System.Collections;

public class NoGravityBallPO : MonoBehaviour, Resetable
{
    public Vector3 originalPosition {get; set; }

    Rigidbody2D rb;
    AudioSource audioSource;
    
    private float airFriction = 6;

    private Vector2 velocity;

    void Start()
    {
        originalPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        rb.gravityScale = 0f;
    }


    void FixedUpdate()
    {
        audioSource.pitch = pitchShift(rb.linearVelocity.magnitude);


        velocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, airFriction * Time.fixedDeltaTime);

        rb.linearVelocity = velocity;
    }


    float pitchShift(float magnitude)
    {
        return 0.5f + (0.06f*magnitude);
    }

    public void Reset()
    {
        transform.position = originalPosition;
        rb.linearVelocity = Vector2.zero;
    }
}