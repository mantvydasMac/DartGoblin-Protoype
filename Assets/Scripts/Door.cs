using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IResetable
{
    public Vector3 originalPosition {get; set;}

    public GameObject[] triggerObjects;
    private ITrigger[] triggers;

    public GameObject doorBody;
    public Transform openPoint;
    public Transform closedPoint;

    private Rigidbody2D doorBodyRb;
    private float moveSpeed = 2.5f;

    private float targetY;

    private AudioSource audioSource;
    public AudioClip moveSound;
    public AudioClip startSound;
    public AudioClip endSound;

    public ParticleSystem psLeft;
    public ParticleSystem psRight;

    private bool activated = false;
    private bool prevActivated = false;

    public enum TriggerType {
        AND,
        OR,
        XOR
    }
    public TriggerType triggerType;

    void Start()
    {
        stopSparkParticles();
        audioSource = GetComponent<AudioSource>();

        doorBody.GetComponent<Collider2D>().excludeLayers = LayerMask.GetMask("Ground");

        doorBodyRb = doorBody.GetComponent<Rigidbody2D>();
        targetY = closedPoint.position.y;

        triggers = new ITrigger[triggerObjects.Length];
        for(int i = 0; i<triggers.Length;++i)
        {
            triggers[i] = triggerObjects[i].GetComponent<ITrigger>();
        }
    }

    void FixedUpdate()
    {
        activated = isActivated();

        if(activated != prevActivated)
        {
            startSparkParticles();
            audioSource.Stop();
            
            if(isOpen() || isClosed())
            {
                audioSource.PlayOneShot(startSound);
            }

            audioSource.PlayOneShot(moveSound);
            if(activated)
            {
                doorBodyRb.linearVelocity = new Vector2(0f, moveSpeed);
            }
            else
            {
                doorBodyRb.linearVelocity = new Vector2(0f, -moveSpeed);
            }
        }
        

        if(doorBody.transform.position.y > openPoint.position.y)
        {
            doorBodyRb.linearVelocity = Vector2.zero;
            doorBody.transform.position = openPoint.position;
            audioSource.Stop();
            audioSource.PlayOneShot(endSound);
            stopSparkParticles();
        }
        else if(doorBody.transform.position.y < closedPoint.position.y)
        {
            doorBodyRb.linearVelocity = Vector2.zero;
            doorBody.transform.position = closedPoint.position;
            audioSource.Stop();
            audioSource.PlayOneShot(endSound);
            stopSparkParticles();
        }


        prevActivated = activated;
    }

    bool isOpen()
    {
        return doorBody.transform.position.y == openPoint.position.y;
    }

    bool isClosed()
    {
        return doorBody.transform.position.y == closedPoint.position.y;
    }

    void startSparkParticles()
    {
        psLeft.Play();
        psRight.Play();
    }

    void stopSparkParticles()
    {
        psLeft.Stop();
        psRight.Stop();
    }

    bool isActivated()
    {
        switch (triggerType) {
            case TriggerType.AND:

                for(int i = 0; i<triggers.Length;++i)
                {
                    if(!triggers[i].activated)
                    {
                        return false;
                    }
                }
                return true;

            case TriggerType.OR:
                for(int i = 0; i<triggers.Length;++i)
                {
                    if(triggers[i].activated)
                    {
                        return true;
                    }
                }
                return false;

            case TriggerType.XOR:
                bool oneActivated = false;
                for(int i = 0; i<triggers.Length;++i)
                {
                    if(!oneActivated && triggers[i].activated)
                    {
                        oneActivated = true;
                    }
                    else if(oneActivated && triggers[i].activated)
                    {
                        return false;
                    }
                }
                return oneActivated;

        }
        return false;
    }


    public void Reset()
    {
        doorBodyRb.linearVelocity = Vector2.zero;
        doorBody.transform.position = closedPoint.position;
        activated = false;
        prevActivated = false;
        stopSparkParticles();
    }
}