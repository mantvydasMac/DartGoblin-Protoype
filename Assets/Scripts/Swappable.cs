using UnityEngine;
using System.Collections;

public class Swappable : MonoBehaviour
{

    public void swap(Vector3 pos)
    {
        transform.position = pos;
    }

}