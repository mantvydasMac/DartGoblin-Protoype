using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.Rendering.DebugUI;

public class UIController : MonoBehaviour
{
    public Camera cam;
    private readonly int pixelsPerUnit = 100;

    void LateUpdate()
    {
        var uiDoc = GetComponent<UIDocument>();
        var ps = uiDoc.panelSettings;

        int size = Mathf.RoundToInt(cam.orthographicSize * 2f * pixelsPerUnit);
        int width = Mathf.RoundToInt(size * cam.aspect);

        ps.referenceResolution = new Vector2Int(width, size);

        var root = uiDoc.rootVisualElement;
        root.style.width = width;
        root.style.height = size;

        transform.position = cam.transform.position + cam.transform.forward;
    }
}
