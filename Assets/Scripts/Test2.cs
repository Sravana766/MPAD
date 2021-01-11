using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    AudioSource myAudio;
    public int position = 0;
    public int samplerate = 44100;
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
        Debug.Log(data.Length);
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
