using UnityEngine;


public class Swappable : MonoBehaviour
{
    [SerializeField] public Focusable focusable;

    public Focusable Focusable => focusable;

    public void swap(Vector3 pos)
    {
        transform.position = pos;
    }

}