using UnityEngine;

public interface IResetable
{
    public Vector3 originalPosition { get; set;}

    public void Reset();
}