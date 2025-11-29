using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour
{
    public AudioSource as1;
    public AudioSource as2;

    public float regularVolume;

    public AudioClip firstPart;
    public AudioClip intermediaryPart;
    public AudioClip secondPart;

    private float part1LoopStartTime = 5f;
    private float part1LoopEndTime = 20f;
    private float part2LoopStartTime = 5f;
    private float part2LoopEndTime = 20f;

    private int part1LoopStartSample;
    private int part1LoopEndSample;
    private int part2LoopStartSample;
    private int part2LoopEndSample;

    public GameObject triggerObj1;
    public GameObject triggerObj2;

    private ITrigger trigger1;
    private ITrigger trigger2;

    private float targetVol1;
    private float targetVol2;
    private float targetVol3;

    private float fadeSpeed = 0.3f;


    private enum Stage {
        PART1,
        INTER,
        PART2
    }

    private Stage stage;



    void Start()
    {
        part1LoopStartSample = Mathf.FloorToInt(part1LoopStartTime * firstPart.frequency);
        part1LoopEndSample = Mathf.FloorToInt(part1LoopEndTime * firstPart.frequency);
        part2LoopStartSample = Mathf.FloorToInt(part2LoopStartTime * secondPart.frequency);
        part2LoopEndSample = Mathf.FloorToInt(part2LoopEndTime * secondPart.frequency);


        trigger1 = triggerObj1.GetComponent<ITrigger>();
        trigger2 = triggerObj2.GetComponent<ITrigger>();

        as1.clip = firstPart;

        as2.clip = intermediaryPart;
        as2.loop = true;

        regularVolume = as1.volume;
        as2.volume = 0f;

        targetVol1 = regularVolume;
        targetVol2 = 0f;

        as1.Play();
    }

    void Update()
    {
        if (stage == Stage.PART1 && as1.timeSamples >= part1LoopEndSample)
        {
            as1.timeSamples = part1LoopStartSample;
        }
        else if(stage == Stage.PART2 && as1.timeSamples >= part2LoopEndSample)
        {
            as1.timeSamples = part2LoopStartSample;
        }

        as1.volume = Mathf.MoveTowards(as1.volume, targetVol1, Time.deltaTime * fadeSpeed);
        as2.volume = Mathf.MoveTowards(as2.volume, targetVol2, Time.deltaTime * fadeSpeed);
    }
    

    void FixedUpdate()
    {
        if(stage == Stage.PART1 && trigger1.activated)
        {
            targetVol1 = 0f;
            targetVol2 = regularVolume;
            as2.Play();

            stage = Stage.INTER;
        }

        if(stage == Stage.INTER && as1.volume == 0)
        {
            as1.Stop();
        }

        if(stage == Stage.INTER && trigger2.activated)
        {
            targetVol2 = 0f;

            targetVol1 = regularVolume;

            as1.clip = secondPart;

            as1.Play();

            stage = Stage.PART2;
        }

        if(stage == Stage.PART2 && as2.volume == 0)
        {
            as2.Stop();
        }
    }
}