using UnityEngine;

public class StageCompleteTrigger : MonoBehaviour, ITrigger
{
    public Player player;
    public bool activated { get; set; }

    void Start()
    {
        activated = false;
    }

    void Update()
    {
        if (player.transform.position.x >= transform.position.x)
        {
            activated = true;
        }
    }

}
