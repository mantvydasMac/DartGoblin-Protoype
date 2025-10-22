using UnityEngine;

[ExecuteAlways]
public class Room : MonoBehaviour
{
    public Vector2 size = new Vector2(10f, 6f);
    public Color gizmoColor = new Color(0f, 1f, 0f, 0.3f);

    private Boundary boundary;
    public Boundary getBoundary() {return boundary;}

    public Transform playerRespawn;

    private LayerMask objectsLayerMask;

    private GameObject[] roomObjects;
    private Transform[] roomObjectOriginalTransforms;

    void Start()
    {
        boundary = new Boundary(new Vector2(transform.position.x - (size.x/2), transform.position.y + (size.y/2)),
                                new Vector2(transform.position.x + (size.x/2), transform.position.y + (size.y/2)),
                                new Vector2(transform.position.x - (size.x/2), transform.position.y - (size.y/2)),
                                new Vector2(transform.position.x + (size.x/2), transform.position.y - (size.y/2)));
        


        objectsLayerMask = LayerMask.GetMask("Object");
        var colliders = Physics2D.OverlapAreaAll(boundary.topLeft, boundary.bottomRight, objectsLayerMask);

        roomObjects = new GameObject[colliders.Length];
        roomObjectOriginalTransforms = new Transform[colliders.Length];
        for(int i = 0 ;i<colliders.Length;++i)
        {
            roomObjects[i] = colliders[i].gameObject;
            roomObjectOriginalTransforms[i] = colliders[i].gameObject.transform;
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
        }

        for(int i = 0;i<roomObjects.Length;++i)
        {
            roomObjects[i].GetComponent<Resetable>().Reset();
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