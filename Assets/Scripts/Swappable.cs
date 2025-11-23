using UnityEngine;
using System.Collections;

public class Swappable : MonoBehaviour, ISwappable
{

    public void swap(Vector3 pos)
    {
        transform.position = pos;
    }

}