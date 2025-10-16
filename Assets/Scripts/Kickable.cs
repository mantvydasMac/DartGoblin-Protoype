using UnityEngine;

public class Kickable : MonoBehaviour
{
    public float hitstopTime;
    Rigidbody2D rb;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }



    public void kick(Vector2 vel)
    {
        rb.linearVelocity = vel;
    }

}