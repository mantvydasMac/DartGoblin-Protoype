using UnityEngine;

public interface IKickable
{
    public float hitstopDuration { get; set;}

    public void kick(Vector2 vel);
}