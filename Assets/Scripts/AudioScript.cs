using UnityEngine;
using System.Collections;

public class AudioScript : MonoBehaviour
{
    public int position = 0;
    public int samplerate = 44100;
    public int sampleLength = 44100 * 2;
    public float frequency = 440;

    void Start()
    {
        AudioClip myClip = AudioClip.Create("MySinusoid", sampleLength, 1, samplerate, false, OnAudioRead, OnAudioSetPosition);
        myClip.SetData(GetData(), 0);
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = myClip;
        aud.Play();
    }

    void OnAudioRead(float[] data)
    {
        int count = 0;
        while (count < data.Length)
        {
            data[count] = Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate);
            position++;
            count++;
        }
    }

    void OnAudioSetPosition(int newPosition)
    {
        position = newPosition;
    }

    float[] GetData()
    {
        float[] data = new float[sampleLength];

        int count = 0;
        while (count < data.Length)
        {
            data[count] = Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate);
            count++;
        }

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

        return data;


    }
}