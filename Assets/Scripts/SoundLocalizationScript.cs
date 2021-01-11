using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundLocalizationScript : MonoBehaviour
{
    private readonly int numTouches;
    private AudioPlayer player;
    private AudioSource audio;
    private float localizationFactor, pitchFactor;
    private bool lockTouch, istimeLimitReached, isFirstSamplePlaying;
    private Transform locationMarkerTransform;
    private SpriteRenderer locationMarkerSR;

    private float startTime, sampleOffsetAngle, samplePitch, score, maxTime;

    private int numTrials, maxTrials;

    private SoundLocalizationData dataObject = new SoundLocalizationData();

    private float[] localizationFactors = new float[4];

    // Start is called before the first frame update
    void Start()
    {
        numTrials = 0;
        maxTrials = 4;

        localizationFactors[0] = 90;
        localizationFactors[1] = -90;
        localizationFactors[2] = 90;
        localizationFactors[3] = -90;

        Button right = GameObject.Find("Right").GetComponent<Button>();
        Button left = GameObject.Find("Left").GetComponent<Button>();

        audio = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        //player = new AudioPlayer(audio);

        player = new AudioPlayer(audio, sampleOffsetAngle, samplePitch); //moved up here
        StartNewTrial();
    }

    private void StartNewTrial()
    {
        //locationMarkerTransform.position = new Vector3(291, 520);
        numTrials++;

        if (numTrials <= maxTrials)
        {
            player.Reset();
            lockTouch = true;

            localizationFactor = localizationFactors[numTrials - 1];
            player.SetPitch(1);
            player.SetOffsetAngle(localizationFactor);
            player.Play();
        }

    }

    //-90 implies left
    public void OnRightButtonClick()
    {
        if (localizationFactor == 90)
            dataObject.SetWasCorrect(numTrials, true);
        else
            dataObject.SetWasCorrect(numTrials, false);
        dataObject.SetWasLeft(numTrials, false);

        StartNewTrial();
    }

    public void OnLeftButtonClick()
    {
        if (localizationFactor == -90)
            dataObject.SetWasCorrect(numTrials, true);
        else
            dataObject.SetWasCorrect(numTrials, false);
        dataObject.SetWasLeft(numTrials, true);

        StartNewTrial();
    }


    // Update is called once per frame
    void Update()
    {

    }
}
