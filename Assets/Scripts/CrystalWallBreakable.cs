using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalWallBreakable : MonoBehaviour, IBreakable, IResetable
{
    public Vector3 originalPosition { get; set;}

    private SpriteRenderer sr;
    private Collider2D col;
    private AudioSource audioSource;
    
    public ParticleSystem ps;



    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
    }


    void disable()
    {
        sr.enabled = false;
		col.enabled = false;
    }

    void enable()
    {
        sr.enabled = true;
		col.enabled = true;
    }

    public void Break()
    {
        audioSource.Play();
        ps.Play();
        disable();
    }

    public void Reset()
    {
        enable();
    }
}