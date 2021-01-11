using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Calibration : MonoBehaviour
{
    public Button leftEar;
    public Button rightEar;
    public Button stop;
    private float left;
    private float right;
    private AudioSource audioSource;
    private AudioPlayerUpdated audioPlayer;


    private float caldBSPL;
    private float currentdB_SPL;
    private int numSamples;
    private float[] samples;
    private float sum;
    private float rmsValue;
    private float adjustedDBValue;
    private float[] calibratedSamples;
    private float MCL;
    private float[] originalSamples;


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

        ;

        audioSource = GameObject.Find("Control").GetComponent<AudioSource>();
        audioSource.volume = DecibelToLinear(50f);


        originalSamples = new float[audioSource.clip.samples];
        audioSource.clip.GetData(originalSamples, 0);

        caldBSPL = 103;
        currentdB_SPL = 55;

        ChangeCalibratedVolume(currentdB_SPL);

        audioPlayer = new AudioPlayerUpdated(audioSource, left);
        //audioPlayer.SetOffsetAngle(left);
        audioPlayer.Play();
    }
    public void rightClicked()
    {
        right = 90f;

        


        audioSource = GameObject.Find("Control").GetComponent<AudioSource>();

        originalSamples = new float[audioSource.clip.samples];
        audioSource.clip.GetData(originalSamples, 0);


        audioSource.volume = DecibelToLinear(50f);
        audioPlayer = new AudioPlayerUpdated(audioSource, right);

        caldBSPL = 103;
        currentdB_SPL = 55;


        ChangeCalibratedVolume(currentdB_SPL);

        //audioPlayer.SetOffsetAngle(right);

        audioPlayer.Play();
    }
    public void stopClicked()
    {
        audioPlayer.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeCalibratedVolume(float desiredDB)
    {
        MCL = 80;
        if (desiredDB > MCL)         
            desiredDB = MCL;

        numSamples = audioSource.clip.samples;
        samples = new float[numSamples];
        audioSource.clip.GetData(samples, 0);
        


        sum = 0;
        for (int i = 0; i < numSamples; i++)
        {
            sum += originalSamples[i] * originalSamples[i];
        }

        rmsValue = Mathf.Sqrt(sum / numSamples);

       
        calibratedSamples = new float[numSamples];
        for (int i = 0; i < numSamples; i++)
        {
            calibratedSamples[i] = originalSamples[i] * Mathf.Pow(10, (desiredDB + caldBSPL) / 20);  //x2 = x/(rms)*20*log10(CalibratedNum - desireddB)
            

        }


        Debug.Log(caldBSPL);
        Debug.Log(desiredDB);

        
        audioSource.clip.SetData(calibratedSamples, 0);

      
    }
}
