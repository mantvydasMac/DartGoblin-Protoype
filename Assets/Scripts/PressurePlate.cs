using UnityEngine;
using System.Collections;

public class PressurePlate : MonoBehaviour, IResetable, ITrigger
{
    public Vector3 originalPosition {get; set;}
    public bool activated {get; set;}
    private bool prevActivated;

    public Transform activatePoint;
    private LayerMask activatableObjects;

    private SpriteRenderer spriteRenderer; 
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip activateSound;
    public AudioClip deactivateSound;

    void Start()
    {
        activated = false;
        prevActivated = activated;
        activatableObjects = LayerMask.GetMask("Player", "Object");
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        activated = Physics2D.OverlapPoint(new Vector2(activatePoint.position.x, activatePoint.position.y), activatableObjects) == true;

        if(activated != prevActivated)
        {
            if(activated)
            {
                spriteRenderer.sprite = activeSprite;
                audioSource.PlayOneShot(activateSound);
            }
            else
            {
                spriteRenderer.sprite = inactiveSprite;
                audioSource.PlayOneShot(deactivateSound);
            }
        }
        
        prevActivated = activated;
    }

    public void Reset()
    {
        
    }
}