using UnityEngine;
using System.Collections;

public class Kickable : MonoBehaviour
{
    public float hitstopDuration;
    Rigidbody2D rb;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }



    public void kick(Vector2 vel)
    {
        StartCoroutine(KickCoroutine(vel, hitstopDuration));
    }


    IEnumerator KickCoroutine(Vector2 vel, float duration)
    {
        float prevGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        float prevAngularVelocity = rb.angularVelocity;
        rb.freezeRotation = true;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = vel;
        rb.gravityScale = prevGravity;
        rb.freezeRotation = false;
        rb.angularVelocity = prevAngularVelocity;
    }

}