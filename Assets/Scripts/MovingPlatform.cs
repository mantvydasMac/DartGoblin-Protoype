using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TriggerType {
    AND,
    OR,
    XOR
}

public enum TriggerFallbackActionType
{
    MOVE_BACK_TO_START,
    STOP_MOVING
}

public class MovingPlatform : MonoBehaviour, IResetable
{
    [SerializeField] private List<GameObject> initialTriggers;
    [SerializeField] private List<Transform> stopPoints;
    [SerializeField] private float moveSpeed = 0.01f;
    [SerializeField] private TriggerType triggerType;
    [SerializeField] private TriggerFallbackActionType triggerFallbackActionType;

    private int stopPointIndex = 1;
    private Player player;
    private List<ITrigger> triggers;

    private Rigidbody2D rigidbody;
    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        triggers = initialTriggers.Select(gObj => gObj.GetComponent<ITrigger>()).ToList();
        if (stopPoints.Count > 0) rigidbody.MovePosition(stopPoints[0].position);
    }

    private float Threshold => moveSpeed * 0.2f;
    private void FixedUpdate()
    {
        if (stopPoints.Count <= 1) return;
        bool isTriggerConditionMet = IsTriggerConditionFulfilled(triggerType);
        if (!isTriggerConditionMet)
        {
            if (triggerFallbackActionType == TriggerFallbackActionType.STOP_MOVING)
            {
                if (player != null) player.ParentVelocity = Vector2.zero;
                rigidbody.linearVelocity = Vector2.zero;
                return;
            }
            if (triggerFallbackActionType == TriggerFallbackActionType.MOVE_BACK_TO_START)
            {
                stopPointIndex = 0;
            }
        }

        Transform targetStop = stopPoints[stopPointIndex];
        Vector2 targetPos =  targetStop.position;
        Vector2 currentPos = rigidbody.position;
        
        Vector2 distanceToTarget = targetPos - currentPos;
        Vector2 movement = distanceToTarget.normalized * moveSpeed;
        
        rigidbody.linearVelocity = movement;
        if (player != null) player.ParentVelocity = movement;
        
        if (distanceToTarget.magnitude < Threshold)
        {
            if (!isTriggerConditionMet)
            {
                rigidbody.linearVelocity = Vector2.zero;
                if (player != null) player.ParentVelocity = Vector2.zero;
                return;
            };
            stopPointIndex++;
            if (stopPointIndex == stopPoints.Count) stopPointIndex = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        player = other.gameObject.GetComponent<Player>();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            player.ParentVelocity = Vector2.zero;
            player = null;
        }
    }

    private bool IsTriggerConditionFulfilled(TriggerType triggerType) => triggerType switch
    {
        TriggerType.AND => triggers.All(trigger => trigger.activated),
        TriggerType.OR => triggers.Any(trigger => trigger.activated),
        TriggerType.XOR => triggers.Any(trigger => trigger.activated) && !triggers.All(trigger => trigger.activated),
        _ => throw new ArgumentOutOfRangeException(nameof(triggerType), triggerType, null)
    };

    public Vector3 originalPosition
    {
        get => stopPoints.Count > 0 ? stopPoints[0].position : transform.position;
        set { }
    }

    public void Reset()
    {
        if (stopPoints.Count > 0) transform.position = stopPoints[0].position;
    }
}
