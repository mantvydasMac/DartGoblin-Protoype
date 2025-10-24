using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class Room : MonoBehaviour
{
    public Vector2 size = new Vector2(10f, 6f);
    public Color gizmoColor = new Color(0f, 1f, 0f, 0.3f);

    private Boundary boundary;
    public Boundary getBoundary() {return boundary;}

    public Transform playerRespawn;

    private LayerMask objectsLayerMask;

    private List<IResetable> roomObjects = new List<IResetable>();

    void Start()
    {
        boundary = new Boundary(new Vector2(transform.position.x - (size.x/2), transform.position.y + (size.y/2)),
                                new Vector2(transform.position.x + (size.x/2), transform.position.y + (size.y/2)),
                                new Vector2(transform.position.x - (size.x/2), transform.position.y - (size.y/2)),
                                new Vector2(transform.position.x + (size.x/2), transform.position.y - (size.y/2)));
        


        // objectsLayerMask = LayerMask.GetMask("Object");
        var colliders = Physics2D.OverlapAreaAll(boundary.topLeft, boundary.bottomRight);


        for(int i = 0 ;i<colliders.Length;++i)
        {
            var resetable = colliders[i].GetComponentInParent<IResetable>();
            if (resetable != null)
                roomObjects.Add(resetable);
        }
    }


    void FixedUpdate()
    {

    }

    public void resetRoom(GameObject player)
    {
        if(player != null)
        {
            player.transform.position = playerRespawn.position;
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        foreach(var obj in roomObjects)
        {
            obj.Reset();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, size);


        // Gizmos.color = new Color(1f, 0f, 1f, 0.3f);
        // Gizmos.DrawLine(boundary.topLeft, boundary.topRight);
        // Gizmos.DrawLine(boundary.topRight, boundary.bottomRight);
        // Gizmos.DrawLine(boundary.bottomRight, boundary.bottomLeft);
        // Gizmos.DrawLine(boundary.bottomLeft, boundary.topLeft);
    }
}