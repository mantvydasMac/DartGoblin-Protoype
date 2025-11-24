using UnityEngine;
using System.Collections;

public interface ISwappable
{   
    Focusable Focusable { get; }
    public void swap(Vector3 pos);
}