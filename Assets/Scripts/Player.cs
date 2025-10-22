using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Data;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Animator kickAnim;


    public GameObject kickAnimationObject;
    public GameObject attachedCamera;
    public ParticleSystem tpParticles;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.1f;

    private float gravityValue = -13f;   // custom gravity (stronger than default)
    private bool groundedPlayer;
    private Vector2 velocity;

    private Vector2 moveInput;
    private Vector2 mousePos;

    private bool jumpPressed;
    private float groundSpeed = 4;
    private float airSpeed = 4;
    private float airFriction = 25;

    private Vector3 mouseWorldPos;

    public GameObject sightlineEndpoint;
    public Transform sightlineStartPos;
    private float sightlineLength = 5;
    private Transform targetedObject = null;

    

    private float swapJumpVelocity = 5;
    private int swapJumpLimit = 2;
    private int swapJumpLeft;

    private bool facingLeft = false;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip kickSound;
    public AudioClip swapSound;

    private float kickRange = 1.5f;
    private float groundKickHeight = 2f;
    private Collider2D[] objectsInKickRange;
    private float kickSpeed = 10f;
    private float kickRecoilSpeed = 8f;
    private float stompAngle = 15f;

    private float groundKickStartupTime = 0.02f;
    private float groundKickActiveTime = 0.02f;
    private float airKickStartupTime = 0.02f;
    private float airKickActiveTime = 0.12f;
    private int kickingStage = 0;
    private float kickTimeSum = 0f;
    private bool groundedKick = true;
    private HashSet<Collider2D> prevKickedCols = new HashSet<Collider2D>();
    private float kickLookingDirection;
    private Vector3 kickMousePos;
    private Vector2 airKickCenter;
    private bool kickFacingLeft;
    
    private bool jumpAllowed = true; 

    private bool paused = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // we control gravity manually
        rb.freezeRotation = true; // prevents tipping over

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.8f;
        col = GetComponent<BoxCollider2D>();
        col.excludeLayers = LayerMask.GetMask("RoomBoundary");
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        kickAnim = kickAnimationObject.GetComponent<Animator>();

        swapJumpLeft = swapJumpLimit;
    }

    void FixedUpdate()
    {
        if(!paused) 
        {
            // camera attach
            if(attachedCamera != null)
                attachedCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

            LayerMask layers = LayerMask.GetMask("Ground","Object");
            // Ground check with OverlapCircle
            // groundedPlayer = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
            Vector2 boxSize = new Vector2(col.size.x * transform.lossyScale.x * 0.95f, 0.1f);
            Vector2 boxCenter = (Vector2)transform.position 
                                + Vector2.Scale(col.offset, transform.lossyScale) 
                                + Vector2.down * (col.size.y * transform.lossyScale.y * 0.5f + 0.05f);

            groundedPlayer = Physics2D.OverlapBox(boxCenter, boxSize, 0f, layers);

            UpdateAirborneAnimations();

            if (groundedPlayer && velocity.y < 0)
            {
                //GROUNDED
                swapJumpLeft = swapJumpLimit;

                velocity.y = -1f; // small downward bias keeps player snapped without sinking

                velocity.x = groundSpeed * moveInput.x;

                if(jumpAllowed && jumpPressed)
                {
                    velocity.y = 5f;
                }
            }
            else
            {
                //AIRBORNE
                velocity.y = rb.linearVelocity.y + (gravityValue * Time.fixedDeltaTime);
                velocity.x = airVelocity(airSpeed * moveInput.x);
            }

            // Apply to rigidbody
            rb.linearVelocity = new Vector2(velocity.x, velocity.y);


            //sightline
            mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            mouseWorldPos.z = 0f;

            //facing
            facingLeft = !(mouseWorldPos.x > transform.position.x);
            spriteRenderer.flipX = facingLeft;

            // walking animation
            if(groundedPlayer)
            {
                if(moveInput.x != 0 && (Mathf.Sign(moveInput.x) != Mathf.Sign(mouseWorldPos.x-transform.position.x)))
                {
                    anim.SetBool("walkingBack", true);
                    anim.SetBool("walkingFwd", false);
                }
                else if(moveInput.x != 0 && (Mathf.Sign(moveInput.x) == Mathf.Sign(mouseWorldPos.x-transform.position.x)))
                {
                    anim.SetBool("walkingBack", false);
                    anim.SetBool("walkingFwd", true);
                }
                else
                {
                    anim.SetBool("walkingBack", false);
                    anim.SetBool("walkingFwd", false);
                }
            }
            else
            {
                anim.SetBool("walkingBack", false);
                anim.SetBool("walkingFwd", false);
            }


            float playerToMouseVectorLength = Mathf.Sqrt(Mathf.Pow(mouseWorldPos.x-sightlineStartPos.transform.position.x, 2) + Mathf.Pow(mouseWorldPos.y-sightlineStartPos.transform.position.y, 2));
            float sightlineLengthProportion = sightlineLength/playerToMouseVectorLength;

            Vector2 sightlineEndpointVectorEnd = new Vector2(mouseWorldPos.x-sightlineStartPos.transform.position.x, mouseWorldPos.y-sightlineStartPos.transform.position.y) * sightlineLengthProportion + new Vector2(sightlineStartPos.transform.position.x, sightlineStartPos.transform.position.y);

            LayerMask raycastLayers = LayerMask.GetMask("Ground","Object","RoomBoundary");
            RaycastHit2D raycast = Physics2D.Raycast(new Vector2(sightlineStartPos.position.x, sightlineStartPos.position.y), 
                                                    new Vector2(mouseWorldPos.x-sightlineStartPos.position.x, mouseWorldPos.y-sightlineStartPos.position.y), 
                                                    sightlineLength, raycastLayers);

            Debug.DrawLine(sightlineStartPos.transform.position, new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f), Color.red, Time.fixedDeltaTime);

            if(raycast)
            {
                Debug.DrawLine(sightlineStartPos.transform.position, new Vector3(raycast.point.x, raycast.point.y, 0f), Color.green, Time.fixedDeltaTime);

                if(raycast.collider.gameObject.GetComponent<Swappable>() != null)
                {
                    sightlineEndpoint.transform.position = new Vector3(raycast.transform.position.x, raycast.transform.position.y, -5); //raycast.transform.position;
                    targetedObject = raycast.transform;
                }
                else
                {
                    sightlineEndpoint.transform.position = new Vector3(raycast.point.x, raycast.point.y, 0f);
                    targetedObject = null;
                }
            }
            else
            {
                targetedObject = null;
                sightlineEndpoint.transform.position = new Vector3(sightlineEndpointVectorEnd.x, sightlineEndpointVectorEnd.y, 0f);
            }

            //kick
            if(kickingStage == 1)
            {// Startup
                kickTimeSum += Time.fixedDeltaTime;

                if((groundedKick && kickTimeSum >= groundKickStartupTime) || (!groundedKick && kickTimeSum >= airKickStartupTime))
                {
                    kickTimeSum = 0f;
                    kickingStage = 2;
                }
            }
            else if(kickingStage == 2)
            {// Active
                kickTimeSum += Time.fixedDeltaTime;


                if(groundedKick)
                {
                    objectsInKickRange = Physics2D.OverlapAreaAll(new Vector2(groundCheck.position.x, groundCheck.position.y), 
                                                        new Vector2(groundCheck.position.x + (kickFacingLeft ? -kickRange/2 : kickRange/2), groundCheck.position.y + groundKickHeight));

                    Debug.DrawLine(new Vector2(groundCheck.position.x, groundCheck.position.y), new Vector2(groundCheck.position.x + (kickFacingLeft ? -kickRange/2 : kickRange/2), groundCheck.position.y), Color.purple, Time.fixedDeltaTime);
                    Debug.DrawLine(new Vector2(groundCheck.position.x, groundCheck.position.y), new Vector2(groundCheck.position.x, groundCheck.position.y + groundKickHeight), Color.purple, Time.fixedDeltaTime);
                    Debug.DrawLine(new Vector2(groundCheck.position.x + (kickFacingLeft ? -kickRange/2 : kickRange/2), groundCheck.position.y), new Vector2(groundCheck.position.x + (kickFacingLeft ? -kickRange/2 : kickRange/2), groundCheck.position.y + groundKickHeight), Color.purple, Time.fixedDeltaTime);
                    Debug.DrawLine(new Vector2(groundCheck.position.x + (kickFacingLeft ? -kickRange/2 : kickRange/2), groundCheck.position.y + groundKickHeight), new Vector2(groundCheck.position.x, groundCheck.position.y + groundKickHeight), Color.purple, Time.fixedDeltaTime);
                    
                    if(kickTimeSum >= groundKickActiveTime)
                    {
                        kickingStage = 0;
                    }
                }
                else
                {
                    Vector2 kickCircleCenter = airKickCenter + new Vector2(transform.position.x, transform.position.y);

                    Debug.DrawLine(kickCircleCenter - new Vector2(kickRange/2, 0), kickCircleCenter + new Vector2(kickRange/2, 0), Color.purple, Time.fixedDeltaTime);
                    Debug.DrawLine(kickCircleCenter - new Vector2(0f, kickRange/2), kickCircleCenter + new Vector2(0f, kickRange/2), Color.purple, Time.fixedDeltaTime);

                    objectsInKickRange = Physics2D.OverlapCircleAll(kickCircleCenter, kickRange/2);

                    if(kickTimeSum >= airKickActiveTime)
                    {
                        kickingStage = 0;
                    }
                }
                
                foreach(Collider2D collider in objectsInKickRange)
                {
                    if(collider.gameObject.GetComponent<Kickable>() != null && !prevKickedCols.Contains(collider))
                    {
                        Vector2 kickDirection;
                        
                        if(groundedKick)
                        {
                            kickDirection = new Vector2(kickMousePos.x - collider.gameObject.transform.position.x, kickMousePos.y - collider.gameObject.transform.position.y);
                        }
                        else 
                        {
                            kickDirection = new Vector2(kickMousePos.x - transform.position.x, kickMousePos.y - transform.position.y);
                        }

                        collider.gameObject.GetComponent<Kickable>().kick(kickDirection.normalized * kickSpeed);

                        if(!groundedPlayer)
                        {
                            if(kickLookingDirection >= -90 - stompAngle && kickLookingDirection <= -90 + stompAngle)
                            {
                                rb.linearVelocity = new Vector2(rb.linearVelocity.x, kickRecoilSpeed);
                                // kickRecoilVerticalStaling -= 0.25f;
                            }
                        }

                        audioSource.pitch = Random.Range(0.95f, 1.05f);
                        audioSource.PlayOneShot(kickSound);
                        StartCoroutine(HitstopCoroutine(collider.gameObject.GetComponent<Kickable>().hitstopDuration));

                        prevKickedCols.Add(collider);
                    }
                }
            }
        }
    }

    float airVelocity(float targetVelocity)
    {
        return Mathf.MoveTowards(rb.linearVelocity.x, targetVelocity, airFriction * Time.fixedDeltaTime);
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        mousePos = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        jumpPressed = value.isPressed;
    }

    void OnKick()
    {
        if(kickingStage == 0)
        {
            kickingStage = 1;
            kickTimeSum = 0f;
            prevKickedCols.Clear();
            kickMousePos = mouseWorldPos;
            groundedKick = groundedPlayer;
            kickFacingLeft = facingLeft;



            //kick direction vector
            Vector2 direction = new Vector2(mouseWorldPos.x-transform.position.x, mouseWorldPos.y-transform.position.y);

            float kickRangeProportion = kickRange/(2*direction.magnitude);
            airKickCenter = direction * kickRangeProportion;


            direction.Normalize();
            kickLookingDirection = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


            // ANIMATION

            if(groundedPlayer)
            {
                //GROUNDED

                kickAnimationObject.GetComponent<SpriteRenderer>().flipX = facingLeft;
                kickAnimationObject.transform.localPosition = new Vector3((facingLeft ? -0.1f : 0.1f), 0.1f, 0);
                kickAnimationObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                kickAnim.SetTrigger("Kick");
            }
            else
            {
                //AIRBORNE
                kickAnimationObject.GetComponent<SpriteRenderer>().flipX = facingLeft;

                if(IsLookingDown(kickLookingDirection))
                {
                    kickAnimationObject.transform.localPosition = new Vector3(0, -0.2f, 0);
                    kickAnimationObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    kickAnim.SetTrigger("Stomp");
                }
                else 
                {
                    kickAnimationObject.transform.localPosition = new Vector3(direction.x * 0.1f, direction.y * 0.1f, 0);
                    float rotation = Mathf.Atan(direction.y/direction.x) * Mathf.Rad2Deg;
                    kickAnimationObject.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
                    kickAnim.SetTrigger("AirKick");
                }
                
            }
        }
    }

    void OnSwap()
    {
        if(targetedObject != null)
        {
            Vector3 playerTargetPos = targetedObject.position;
            targetedObject.gameObject.GetComponent<Swappable>().swap(transform.position);
            transform.position = playerTargetPos;
            targetedObject = null;

            if (moveInput.y > 0 && swapJumpLeft > 0)
            {
                rb.linearVelocity = new Vector2(0f, swapJumpVelocity);
                swapJumpLeft--;
            }
            tpParticles.Play();
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(swapSound);
        }
    }

    IEnumerator HitstopCoroutine(float duration)
    {
        paused = true;
        Vector2 prevVelocity = rb.linearVelocity;
        rb.linearVelocity = Vector2.zero;
        anim.speed = 0f;
        kickAnim.speed = 0.5f;
        

        yield return new WaitForSeconds(duration);


        rb.linearVelocity = prevVelocity;
        anim.speed = 1f;
        kickAnim.speed = 1f;
        paused = false;
    }

    void OnDrawGizmos()
    {
        if (col == null) return;

        // World-space size
        Vector2 boxSize = new Vector2(col.size.x * transform.lossyScale.x * 0.95f, 0.1f);
        Vector2 boxCenter = (Vector2)transform.position
                            + Vector2.Scale(col.offset, transform.lossyScale)
                            + Vector2.down * (col.size.y * transform.lossyScale.y * 0.5f + 0.05f);

        Gizmos.color = groundedPlayer ? Color.green : Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    private void UpdateAirborneAnimations()
    {
        if (groundedPlayer)
        {
            anim.SetBool("grounded", true);
            return;
        }
        
        Vector2 direction = GetDirection();
        float lookingRotation = GetLookingRotation(direction);

        if (IsLookingDown(lookingRotation))
        {
            anim.SetBool("Stomp", true);
        }
        else
        {
            anim.SetBool("Stomp", false);
        }

        anim.SetBool("grounded", false);
        anim.SetFloat("velocity", rb.linearVelocity.y);
    }

    private Vector2 GetDirection()
    {
        Vector2 direction = new Vector2(mouseWorldPos.x - transform.position.x, mouseWorldPos.y - transform.position.y);
        direction.Normalize();
        return direction;
    }

    private float GetLookingRotation(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
    
    private bool IsLookingDown(float lookingRotation)
    {
        return lookingRotation >= -90 - stompAngle && lookingRotation <= -90 + stompAngle;
    }
}
