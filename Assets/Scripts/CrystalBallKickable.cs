using UnityEngine;
using System.Collections;

public class CrystalBallKickable : MonoBehaviour, IKickable
{
    [field: SerializeField]
    public float hitstopDuration {get; set;}
    Rigidbody2D rb;

    public bool kicked = false;
    public bool getKicked() {return kicked;}



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
        kicked = true;
    }


}