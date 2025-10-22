using UnityEngine;

public interface Resetable
{
    public Vector3 originalPosition { get; set;}

    public void Reset();
}