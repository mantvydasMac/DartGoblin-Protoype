using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Stage : MonoBehaviour
{
    public GameObject cam;
    public GameObject player;

    private PlayerInput playerInput;
    private InputAction resetAction;

    private Camera cameraSettings;
    private float cameraAspectRatio;
    private float cameraMaxSize = 5;

    private Vector3 cameraTargetPos;
    private float cameraTargetSize;
    private float cameraMoveSpeed = 25;
    private float cameraZoomSpeed = 2;

    private Room[] rooms;
    private int playerRoom = -2;
    private int prevPlayerRoom = -2;

    void OnEnable()
    {
        playerInput = player.GetComponent<PlayerInput>();
        resetAction = playerInput.actions["Reset"];
        resetAction.performed += OnReset;
    }

    void OnDisable()
    {
        resetAction.performed -= OnReset;
    }

    void Start()
    {   
        cameraSettings = cam.GetComponent<Camera>();
        cameraAspectRatio = cameraSettings.aspect;

        var roomObjects = GameObject.FindGameObjectsWithTag("Room");
        rooms = new Room[roomObjects.Length];

        for(int i = 0;i<roomObjects.Length;++i)
        {
            rooms[i] = roomObjects[i].GetComponent<Room>();
        }
    }


    void FixedUpdate()
    {
        try
        {
            playerRoom = getRoomWithPlayer();
            Room room = rooms[playerRoom];

            if (playerRoom != prevPlayerRoom)
            {
                Boundary b = room.getBoundary();

                float roomWidth = b.topRight.x - b.topLeft.x;

                float size = roomWidth / (2 * cameraAspectRatio);

                if (size <= cameraMaxSize)
                {
                    cameraTargetSize = size;
                }
                else
                {
                    cameraTargetSize = cameraMaxSize;
                }
            }

            cameraTargetPos = getCameraTargetPos(room);

            cam.transform.position = Vector3.MoveTowards(cam.transform.position, cameraTargetPos, cameraMoveSpeed * Time.fixedDeltaTime);
            cameraSettings.orthographicSize = Mathf.MoveTowards(cameraSettings.orthographicSize, cameraTargetSize, cameraZoomSpeed * Time.fixedDeltaTime);


            prevPlayerRoom = playerRoom;

        }
        catch (Exception)
        {
            playerRoom = prevPlayerRoom;
            OnReset();
        }
    }

    Vector3 getCameraTargetPos(Room room)
    {
        float camHeight = cameraTargetSize * 2;
        float camWidth = 2 * cameraAspectRatio * cameraTargetSize;

        Boundary b = room.getBoundary();
        Vector2 bl = b.bottomLeft;
        Vector2 tr = b.topRight;

        float distToLeftWall = Mathf.Abs(bl.x - player.transform.position.x);
        float distToRightWall = Mathf.Abs(tr.x - player.transform.position.x);
        float distToFloor = Mathf.Abs(bl.y - player.transform.position.y);
        float distToCeil = Mathf.Abs(tr.y - player.transform.position.y);
        
        float camX = player.transform.position.x;
        float camY = player.transform.position.y;

        // x adjust
        if(distToLeftWall < camWidth/2)
        {
            camX += ((camWidth/2) - distToLeftWall);
        }
        else if(distToRightWall < camWidth/2)
        {
            camX -= ((camWidth/2) - distToRightWall);
        }

        // y adjust
        if(distToFloor < camHeight/2)
        {
            camY += ((camHeight/2) - distToFloor);
        }
        else if(distToCeil < camHeight/2)
        {
            camY -= ((camHeight/2) - distToCeil);
        }

        return new Vector3(camX, camY, -10f);
    }

    int getRoomWithPlayer()
    {
        for(int i = 0;i<rooms.Length;++i)
        {
            Vector2 pos = player.transform.position;
            Vector2 bl = rooms[i].getBoundary().bottomLeft;
            Vector2 tr = rooms[i].getBoundary().topRight;

            if (pos.x >= bl.x && pos.x <= tr.x &&
                pos.y >= bl.y && pos.y <= tr.y)
            {
                return i;
            }
        }
        return -1;
    }

    void OnReset(InputAction.CallbackContext ctx)
    {
        rooms[playerRoom].resetRoom(player);
    }

    void OnReset()
    {
        rooms[playerRoom].resetRoom(player);
    }
}