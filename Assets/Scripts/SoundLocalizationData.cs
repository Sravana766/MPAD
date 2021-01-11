using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLocalizationData : MonoBehaviour
{
    public bool[] correct = new bool[100]; //100 is a random number, i doubt it'll go that long...
    public bool[] wasLeft = new bool[100];

    public int trialNum;
    public int fileFrequency;
    public int dB_SPLThreshold;
    public int numReversals;


    public void SetWasCorrect(int trialNumber, bool wasItCorrect)
    {
        trialNum = trialNumber;
        int trialIndex = trialNumber - 1;
        correct[trialIndex] = wasItCorrect;
    }

    public void SetWasLeft(int trialNumber, bool wasItLeft)
    {
        trialNum = trialNumber;
        int trialIndex = trialNumber - 1;
        wasLeft[trialIndex] = wasItLeft;
    }

}
