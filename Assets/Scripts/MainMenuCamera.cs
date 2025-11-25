using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    public Camera mainCamera;
    public float cameraSpeed = 1f;

    void Update()
    {
        mainCamera.transform.position += new Vector3(cameraSpeed * Time.deltaTime, 0f, 0f);
    }
}
