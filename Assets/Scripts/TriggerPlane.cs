using UnityEngine;
using UnityEngine.InputSystem;

public class TriggerPlane : MonoBehaviour, ITrigger
{
    public bool activated { get; set;}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            activated = true;
        }
    }
}