using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InfoText : MonoBehaviour
{
    public TMP_Text text;
    public GameObject player;
    public float turnOffDistance = 3f;
    public Light2D signLight;

    private int offFrameCounter = 30;
    private bool isVisible = true;

    private readonly float max = 0.8f;
    private readonly float min = 0.5f;
    private readonly double changeToBeStable = 0.997;

    void Update()
    {
        if (isTextVisible())
            FlickerSignText();
    }

    bool isTextVisible()
    {
        if (player == null)
        {
            return true;
        }

        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2 signPos = new Vector2(transform.position.x, transform.position.y);

        if (Vector2.Distance(playerPos, signPos) > turnOffDistance)
        {
            Color color = text.color;
            color.a = 0f;
            text.color = color;
            return false;
        }

        return true;
    }

    void FlickerSignText()
    {
        if (!isVisible && offFrameCounter > 0)
        {
            offFrameCounter--;
            return;
        }

        offFrameCounter = 30;

        Color color = text.color;

        isVisible = new System.Random().NextDouble() < changeToBeStable;
        color.a = isVisible ? max : min;
        signLight.enabled = isVisible;

        text.color = color;
    }
}
