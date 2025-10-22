using UnityEngine;

public interface PhysicsObject
{
    public Vector3 originalPosition { get; set;}

    public void Reset();
}