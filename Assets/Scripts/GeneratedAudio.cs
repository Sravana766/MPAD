using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedAudio : MonoBehaviour
{
    AudioSource myAudio;
    public int position = 0;
    public int samplerate = 44100; //192000
    public float frequency = 440;

    void Start()
    {
        generateSound();
    }
    public void generateSound()
    {
        // parameter 6 is calling the callback function to generate sound
        AudioClip myClip = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = myClip;
        aud.Play();
    }
    // Callback function that generates the sound
    void OnAudioRead(float[] data)
    {
        //Debug.Log(data.Length);

        int rampLength = (int)(data.Length / 30); //ramp is 10% of the length of one pulse, which is 33%
        int pulseLength = rampLength * 10; //10% of time to ramp up, 80% at 1, and 10% to ramp down
        int count = 0;


        //IDK ABOUT THIS FIX LATER DEPENDING ON THE LENGTH
        int rampfrequency = 30; //Ramp will last for 0.03333 sec or about 10% of the length
        float[] rampData = new float[pulseLength];

        //Initialize with all 1's, will fix the ends later
        while (count < pulseLength)
        {
            rampData[count] = 1;
        }

        count = 0;
        while (count < rampLength)
        {
            rampData[count] = Mathf.Pow(Mathf.Sin(2 * Mathf.PI * rampfrequency * count / samplerate), 2);
            rampData[pulseLength - 1 - count] = Mathf.Pow(Mathf.Sin(2 * Mathf.PI * rampfrequency * count / samplerate), 2);
            count++;
        }

        float[] pulseData = new float[pulseLength];
        count = 0;
        while (count < pulseLength)
        {
            pulseData[count] = rampData[count] * Mathf.Sin(2 * Mathf.PI * frequency * count / samplerate);
            count++;
        }

        int totalSoundLength = pulseLength * 3;

        //Debug.Log(data.Length);

        count = 0;
        int secondCounter = 0;
        while (count < totalSoundLength)
        {
            if (secondCounter >= pulseLength) //This allows pulseData to be repeated 3 times to make the pulse
                secondCounter -= pulseLength;

            data[count] = pulseData[secondCounter];
            position++; //Position is used in OnAudioSetPosition?
            count++;
            secondCounter++;
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
