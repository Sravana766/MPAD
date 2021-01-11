using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class iPadCali : MonoBehaviour
{
    public Button leftEar;
    public Button rightEar;
    public Button stop;
    private float left;
    private float right;
    private AudioSource audioSource;
    private AudioPlayerUpdated audioPlayer;
    private AudioClip mls;
    private AudioClip sync;
    private AudioClip gap;
    public AudioClip[] adClipsInOrder;
    private AudioClip result;

    private float cal0dBSPL;
    private float stimdBSPL;
    private float syncSPL;
    private int numSamplesMLS;
    private float[] samplesMLS;
    private int numSamplesSync;
    private float[] samplesSync;
    private float sum;
    private float rmsValue;
    private float adjustedDBValue;
    private float[] calibratedSamplesMLS;
    private float[] calibratedSamplesSync;
    private float MCL;
    private float[] originalSamplesMLS;
    private float[] originalSamplesSync;

    // Start is called before the first frame update
    void Start()
    {
        InitializeButtons();
    }
    public void InitializeButtons()
    {
        leftEar = GameObject.Find("Left").GetComponent<Button>();
        leftEar.onClick.AddListener(leftClicked);
        rightEar = GameObject.Find("Right").GetComponent<Button>();
        rightEar.onClick.AddListener(rightClicked);
        stop = GameObject.Find("Stop").GetComponent<Button>();
        stop.onClick.AddListener(stopClicked);

    }

    private float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB / 20.0f);

        return linear;
    }
    public void leftClicked()
    {
        left = -90f;

        
        audioSource = GameObject.Find("Control").GetComponent<AudioSource>();
        sync = Resources.Load("sync") as AudioClip;
        gap = Resources.Load("gap") as AudioClip;
        mls = Resources.Load("mls18") as AudioClip;
        audioSource.volume = DecibelToLinear(50f);


        originalSamplesMLS = new float[mls.samples];
        mls.GetData(originalSamplesMLS, 0);

        originalSamplesSync = new float[sync.samples];
        sync.GetData(originalSamplesSync, 0);

        cal0dBSPL = -20;
        stimdBSPL= 55;

        ChangeCalibratedVolume(stimdBSPL);

        adClipsInOrder = new AudioClip[4];
        adClipsInOrder[0] = sync;
        adClipsInOrder[1] = gap;
        adClipsInOrder[2] = mls;
        adClipsInOrder[3] = gap;
        AudioClipCombine(adClipsInOrder);
        audioSource.clip = result;
        audioPlayer = new AudioPlayerUpdated(audioSource,  left);
        audioPlayer.Play();

    }

    public void rightClicked()
    {
        left = +90f;


        audioSource = GameObject.Find("Control").GetComponent<AudioSource>();
        sync = Resources.Load("sync") as AudioClip;
        gap = Resources.Load("gap") as AudioClip;
        mls = Resources.Load("mls18") as AudioClip;
        audioSource.volume = DecibelToLinear(50f);


        originalSamplesMLS = new float[mls.samples];
        mls.GetData(originalSamplesMLS, 0);

        originalSamplesSync = new float[sync.samples];
        sync.GetData(originalSamplesSync, 0);

        cal0dBSPL = -20; //need to change this in lab after calibration.  
        stimdBSPL = 55;

        ChangeCalibratedVolume(stimdBSPL);



        adClipsInOrder = new AudioClip[4];
        adClipsInOrder[0] = sync;
        adClipsInOrder[1] = gap;
        adClipsInOrder[2] = mls;
        adClipsInOrder[3] = gap;
        AudioClipCombine(adClipsInOrder);
        audioSource.clip = result;
        audioPlayer = new AudioPlayerUpdated(audioSource, right);
        audioPlayer.Play();
    }
    public void ChangeCalibratedVolume(float desiredDB)
    {
        MCL = 80;
        if (desiredDB > MCL) { 
        desiredDB = MCL;
        }
        desiredDB = stimdBSPL;
        if (desiredDB > 55)
            syncSPL = 75;
        else if (desiredDB > 20)
            syncSPL = 65;
        else
            syncSPL = 55;



        numSamplesMLS = mls.samples;
        numSamplesSync = sync.samples;
        

        sum = 0;
        for (int i = 0; i < numSamplesMLS; i++)
        {
            sum += originalSamplesMLS[i] * originalSamplesMLS[i];
        }

        rmsValue = Mathf.Sqrt(sum / numSamplesMLS);


        calibratedSamplesMLS = new float[numSamplesMLS];
        for (int i = 0; i < numSamplesMLS; i++)
        {
            calibratedSamplesMLS[i] = originalSamplesMLS[i] * Mathf.Pow(10, (desiredDB + cal0dBSPL) / 20);  //x2 = x/(rms)*20*log10(CalibratedNum - desireddB)
        }

        calibratedSamplesSync = new float[numSamplesSync];
        for (int i = 0; i < numSamplesSync; i++)
        {
            calibratedSamplesSync[i] = originalSamplesSync[i] * Mathf.Pow(10, (syncSPL + cal0dBSPL) / 20);  //x2 = x/(rms)*20*log10(CalibratedNum - desireddB)
        }

        mls.SetData(calibratedSamplesMLS, 0);
        sync.SetData(calibratedSamplesSync, 0);


    }

    public void stopClicked()
    {
        audioPlayer.Stop();
    }

    public void AudioClipCombine(AudioClip[] clips)
    {
        int length = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;

            length += clips[i].samples * clips[i].channels;
        }

        float[] data = new float[length];
        length = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;

            float[] buffer = new float[clips[i].samples * clips[i].channels];
            clips[i].GetData(buffer, 0);
            //System.Buffer.BlockCopy(buffer, 0, data, length, buffer.Length);
            buffer.CopyTo(data, length);
            length += buffer.Length;
        }



        result = AudioClip.Create("Combine", length, 1, 44100, false, false);
        result.SetData(data, 0);

    }

    // Update is called once per frame
    void Update()
    {

    }
}




