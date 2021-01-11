using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test123 : MonoBehaviour
{
    AudioSource myAudio;
    public int position = 0;
    public int samplerate = 19200;
    public float frequency = 1000;

    void Start()
    {
        generateSound();
    }
    public void generateSound()
    {
        // parameter 6 is calling the callback function to generate sound
        AudioClip myClip = AudioClip.Create("MySinusoid", samplerate * 1, 1, samplerate, false, OnAudioRead, OnAudioSetPosition);
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = myClip;
        aud.Play();
    }
    // Callback function that generates the sound
    void OnAudioRead(float[] data)
    {
        //Debug.Log(data.Length);
        int count = 0;
        while (count < data.Length)
        {
            data[count] = Mathf.Sin(2 * Mathf.PI * frequency * count / samplerate);
            //position++; //IDK WHAT THIS IS BUT I COMMENTED IT OUT, INSERTED IT LATER
            count++;
        }

        Debug.Log(count);

        data = new float[samplerate * 1];

        //Generate ramp
        float rampDuration = 20 * Mathf.Pow(10, -3); //onset and offset ramps in seconds(20 ms is a good default)
        int rampLength = (int)(rampDuration * samplerate);

        float[] actualramp = new float[rampLength];
        count = 0;
        while (count < rampLength)
        {
            actualramp[count] = Mathf.Pow(Mathf.Sin(Mathf.PI / 2 * count / rampLength), 2);
            count++;
        }

        float[] rampdata = new float[data.Length];
        count = 0;
        while (count < rampdata.Length)
        {
            rampdata[count] = 1;
            count++;
        }

        //Insert beginning of ramp
        count = 0;
        while (count < rampLength)
        {
            rampdata[count] = actualramp[count];
            count++;
        }

        //Insert end of ramp
        count = 0;
        while (count < rampLength)
        {
            rampdata[rampLength - count - 1] = actualramp[rampLength - count - 1];
            count++;
        }

        //Modify data with ramp
        count = 0;
        while (count < data.Length)
        {
            data[count] = data[count] * rampdata[count];
            position++;
            count++;
        }
    }

    void OnAudioSetPosition(int newPosition)
    {
        Debug.Log(newPosition);
        position = newPosition;
    }
    // To read from Resources
    public void playmysound()
    {
        myAudio = GetComponent<AudioSource>();
        myAudio.clip = Resources.Load<AudioClip>("gameover");
        myAudio.Play();
    }
    // Update is called once per frame
    void Update()
    {

    }
}

// https://docs.unity3d.com/ScriptReference/AudioClip.Create.html
