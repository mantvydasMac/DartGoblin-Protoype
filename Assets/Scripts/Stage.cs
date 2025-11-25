using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class Stage : MonoBehaviour
{
    public GameObject cam;
    public GameObject player;
    public GameObject screenFade;
    private SpriteRenderer screenFadeRenderer;
    private Player playerScript;

    private enum FadeStage {
        NONE,
        FADE_OUT,
        FADE_IN,
        WAITING
    }

    private FadeStage fadeStage;
    private int waitCounter = 0;
    private int waitUntil = 0;

    private bool allowReset = true;
    public AudioClip deathSound;

    private float fadeSpeed = 5;

    private PlayerInput playerInput;
    private InputAction resetAction;
    private InputAction roomScopeAction;

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

    private bool isRoomScopePressed = false;

    void OnEnable()
    {
        if (player == null) return;

        playerInput = player.GetComponent<PlayerInput>();
        resetAction = playerInput.actions["Reset"];
        resetAction.performed += OnReset;

        roomScopeAction = playerInput.actions["RoomScope"];
        roomScopeAction.started += OnRoomScopeStarted;
        roomScopeAction.canceled += OnRoomScopeCanceled;
    }

    void OnDisable()
    {
        if (player == null) return;
        resetAction.performed -= OnReset;
        roomScopeAction.started -= OnRoomScopeStarted;
        roomScopeAction.canceled -= OnRoomScopeCanceled;
    }

    void Start()
    {   
        cameraSettings = cam.GetComponent<Camera>();
        cameraAspectRatio = cameraSettings.aspect;
        cameraMaxSize = cameraSettings.orthographicSize;

        screenFade.transform.localScale = new Vector3(cameraMaxSize * 2 * cameraAspectRatio, cameraMaxSize*2, screenFade.transform.localScale.z);
        screenFadeRenderer = screenFade.GetComponent<SpriteRenderer>();
        screenFadeRenderer.color = new Color(0f, 0f, 0f, 1f);

        playerScript = player.GetComponent<Player>();

        var roomObjects = GameObject.FindGameObjectsWithTag("Room");
        rooms = new Room[roomObjects.Length];

        for(int i = 0;i<roomObjects.Length;++i)
        {
            rooms[i] = roomObjects[i].GetComponent<Room>();
        }

        StartCoroutine(StageStartupCoroutine());
    }


    void FixedUpdate()
    {
        if (player == null) return;

        try
        {   
            ScreenFading();

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

            cameraTargetPos = getCameraTargetPos(room, isRoomScopePressed ? playerScript.getMouseWorldPos() : player.transform.position);

            Vector3 newPos = Vector3.MoveTowards(cam.transform.position, cameraTargetPos, cameraMoveSpeed * Time.fixedDeltaTime);
            cam.transform.position = newPos;
            screenFade.transform.position = new Vector3(newPos.x, newPos.y, -9f);
            cameraSettings.orthographicSize = Mathf.MoveTowards(cameraSettings.orthographicSize, cameraTargetSize, cameraZoomSpeed * Time.fixedDeltaTime);


            prevPlayerRoom = playerRoom;

        }
        catch (Exception)
        {
            playerRoom = prevPlayerRoom;
            OnDeath();
        }
    }

    void ScreenFading()
    {
        switch (fadeStage)
        {
            case FadeStage.FADE_OUT:
                screenFadeRenderer.color = new Color(screenFadeRenderer.color.r, screenFadeRenderer.color.g, screenFadeRenderer.color.b, 
                                            Mathf.MoveTowards(screenFadeRenderer.color.a, 1f, fadeSpeed * Time.fixedDeltaTime));
                break;

            case FadeStage.FADE_IN:
                screenFadeRenderer.color = new Color(screenFadeRenderer.color.r, screenFadeRenderer.color.g, screenFadeRenderer.color.b, 
                                            Mathf.MoveTowards(screenFadeRenderer.color.a, 0f, fadeSpeed * Time.fixedDeltaTime));
                break;

            case FadeStage.WAITING:
                if(waitCounter < waitUntil)
                {
                    waitCounter++;
                }
                
                break;
        }
    }

    Vector3 getCameraTargetPos(Room room, Vector3 position)
    {
        float camHeight = cameraTargetSize * 2;
        float camWidth = 2 * cameraAspectRatio * cameraTargetSize;

        Boundary b = room.getBoundary();
        Vector2 bl = b.bottomLeft;
        Vector2 tr = b.topRight;

        float distToLeftWall = Mathf.Abs(bl.x - position.x);
        float distToRightWall = Mathf.Abs(tr.x - position.x);
        float distToFloor = Mathf.Abs(bl.y - position.y);
        float distToCeil = Mathf.Abs(tr.y - position.y);
        
        float camX = position.x;
        float camY = position.y;

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
        if(allowReset)
        {
            StartCoroutine(ManualResetCoroutine());
        }
    }

    public void OnDeath()
    {
        if(allowReset)
        {
            StartCoroutine(DeathResetCoroutine());
        }
    }

    IEnumerator ManualResetCoroutine()
    {
        allowReset = false;

        fadeSpeed = 5;
        screenFadeRenderer.color = new Color(0f, 0f, 0f, 0f);

        fadeStage = FadeStage.FADE_OUT;

        yield return new WaitUntil(() => screenFadeRenderer.color.a >= 0.99f);

        screenFadeRenderer.color = new Color(screenFadeRenderer.color.r, screenFadeRenderer.color.g, screenFadeRenderer.color.b, 1f);

        rooms[playerRoom].resetRoom(player);
        
        Vector3 newPos = getCameraTargetPos(rooms[playerRoom], player.transform.position);
        cam.transform.position = newPos;
        screenFade.transform.position = new Vector3(newPos.x, newPos.y, -9f);

        fadeStage = FadeStage.WAITING;
        waitCounter = 0;
        waitUntil = 12;

        yield return new WaitUntil(() => waitCounter >= waitUntil);

        waitCounter = 0;

        fadeStage = FadeStage.FADE_IN;

        yield return new WaitUntil(() => screenFadeRenderer.color.a <= 0.01f);

        screenFadeRenderer.color = new Color(screenFadeRenderer.color.r, screenFadeRenderer.color.g, screenFadeRenderer.color.b, 0f);

        fadeStage = FadeStage.NONE;
        
        allowReset = true;
    }

    IEnumerator DeathResetCoroutine()
    {
        allowReset = false;

        fadeSpeed = 10;
        screenFadeRenderer.color = new Color(1f, 0.6f, 0.6f, 0f);

        player.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(UnityEngine.Random.Range(-5, 5), 5f);
        PlayAtPoint(deathSound, player.transform.position);

        fadeStage = FadeStage.FADE_OUT;

        yield return new WaitUntil(() => screenFadeRenderer.color.a >= 0.5f);

        screenFadeRenderer.color = new Color(screenFadeRenderer.color.r, screenFadeRenderer.color.g, screenFadeRenderer.color.b, 1f);

        fadeStage = FadeStage.FADE_IN;

        yield return new WaitUntil(() => screenFadeRenderer.color.a <= 0.01f);

        screenFadeRenderer.color = new Color(screenFadeRenderer.color.r, screenFadeRenderer.color.g, screenFadeRenderer.color.b, 0f);

        fadeStage = FadeStage.WAITING;
        waitCounter = 0;
        waitUntil = 8;

        //reset fade
        fadeSpeed = 5;
        screenFadeRenderer.color = new Color(0f, 0f, 0f, 0f);

        fadeStage = FadeStage.FADE_OUT;

        yield return new WaitUntil(() => screenFadeRenderer.color.a >= 0.99f);

        screenFadeRenderer.color = new Color(screenFadeRenderer.color.r, screenFadeRenderer.color.g, screenFadeRenderer.color.b, 1f);

        rooms[playerRoom].resetRoom(player);
        
        Vector3 newPos = getCameraTargetPos(rooms[playerRoom], player.transform.position);
        cam.transform.position = newPos;
        screenFade.transform.position = new Vector3(newPos.x, newPos.y, -9f);

        fadeStage = FadeStage.WAITING;
        waitCounter = 0;
        waitUntil = 8;

        yield return new WaitUntil(() => waitCounter >= waitUntil);

        waitCounter = 0;

        fadeStage = FadeStage.FADE_IN;

        yield return new WaitUntil(() => screenFadeRenderer.color.a <= 0.01f);

        screenFadeRenderer.color = new Color(screenFadeRenderer.color.r, screenFadeRenderer.color.g, screenFadeRenderer.color.b, 0f);

        fadeStage = FadeStage.NONE;

        allowReset = true;
    }

    IEnumerator StageStartupCoroutine()
    {
        allowReset = false;

        fadeSpeed = 4;

        fadeStage = FadeStage.FADE_IN;

        yield return new WaitUntil(() => screenFadeRenderer.color.a <= 0.01f);

        screenFadeRenderer.color = new Color(screenFadeRenderer.color.r, screenFadeRenderer.color.g, screenFadeRenderer.color.b, 0f);

        fadeStage = FadeStage.NONE;
        
        allowReset = true;
    }

    private void OnRoomScopeStarted(InputAction.CallbackContext ctx)
    {
        isRoomScopePressed = true;
    }

    private void OnRoomScopeCanceled(InputAction.CallbackContext ctx)
    {
        isRoomScopePressed = false;
    }

    public static void PlayAtPoint(AudioClip clip, Vector3 pos, float volume = 1.0f)
    {
        GameObject go = new GameObject("OneShotAudio");
        go.transform.position = pos;

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;

        src.spatialBlend = 1f;      // 3D sound
        src.minDistance = 0.2f;     // MUCH louder up close
        src.maxDistance = 30f;
        src.rolloffMode = AudioRolloffMode.Linear;

        src.Play();
        GameObject.Destroy(go, clip.length / src.pitch);
    }
}