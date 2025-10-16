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
        float prevAngularVelocity = rb.angularVelocity;
        rb.Sleep();

        yield return new WaitForSeconds(duration);

        rb.WakeUp();
        rb.linearVelocity = vel;
        rb.angularVelocity = prevAngularVelocity;
    }

}