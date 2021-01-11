using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class SoundController : MonoBehaviour
{
    public Button play;
    public AudioSource audioSource;
    AudioClip sync;
    AudioClip gap;
    AudioClip mls;
    AudioClip extra;
    public AudioClip[] adClipsInOrder;
    AudioClip result;
    void Start()
    {

        InitializeButtons();

    }
    public void InitializeButtons()
    {
        play = GameObject.Find("Play").GetComponent<Button>();
        play.onClick.AddListener(playAudio);

    }
    void Update()
    {

    }
    public void playAudio()
    {
        sync = Resources.Load("word1") as AudioClip;
        gap = Resources.Load("word2") as AudioClip;
        mls = Resources.Load("word3") as AudioClip;
        extra = Resources.Load("word4") as AudioClip;
        adClipsInOrder = new AudioClip[4];
        adClipsInOrder[0] = sync;
        adClipsInOrder[1] = gap;
        adClipsInOrder[2] = mls;
        adClipsInOrder[3] = extra;
        //StartCoroutine(playAudioSequentially());
        AudioClipCombine(adClipsInOrder);
        audioSource.clip = result;
        audioSource.Play();
       


        

    }
    IEnumerator playAudioSequentially()
    {
        yield return null;
        for (int i = 0; i < adClipsInOrder.Length; i++)
        {
            audioSource.clip = adClipsInOrder[i];
            audioSource.Play();
            while (audioSource.isPlaying)
            {
                yield return null;
            }

            //5. Go back to #2 and play the next audio in the adClips array
        }
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
}
