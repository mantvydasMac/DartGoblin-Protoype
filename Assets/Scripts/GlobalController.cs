using UnityEngine;

public class GlobalController : MonoBehaviour
{
    public static int completedLevels;

    public static int stage1Deaths;
    public static int stage2Deaths;
    public static int totalDeaths;

    void Start()
    {
        completedLevels = PlayerPrefs.GetInt("completedLevels", 0);
        stage1Deaths = PlayerPrefs.GetInt("stage1Deaths", 0);
        stage2Deaths = PlayerPrefs.GetInt("stage2Deaths", 0);
        totalDeaths = PlayerPrefs.GetInt("totalDeaths", 0);
    }
}
