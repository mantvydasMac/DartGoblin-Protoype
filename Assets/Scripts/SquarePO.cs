using UnityEngine;
using System.Collections;

public class SquarePO : MonoBehaviour, IResetable
{
    public Vector3 originalPosition { get; set; }
    private Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.position;
    }

    public void Reset()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0f;
    }
}