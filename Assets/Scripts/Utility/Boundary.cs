using UnityEngine;

public struct Boundary 
{
    public Vector2 topLeft, topRight, bottomLeft, bottomRight;

    public Boundary(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
    {
        this.topLeft = topLeft;
        this.topRight = topRight;
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
    }
}