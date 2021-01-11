using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudibilityData : MonoBehaviour
{
    public bool[] wasHeard = new bool[100]; //100 is a random number, i doubt it'll go that long...
    public float[] dB_SPLData = new float[100];

    public int trialNum;
    public int fileFrequency;
    public int dB_SPLThreshold;
    public int numReversals;
      

    public void SetWasHeard(int trialNumber, bool wasItHeard)
    {
        trialNum = trialNumber;
        int trialIndex = trialNumber - 1;
        wasHeard[trialIndex] = wasItHeard;
    }

    public void SetNumReversals(int totalNumReversals)
    {
        numReversals = totalNumReversals;
    }

    public void SetdB_SPLData(int trialNumber, float dB_SPL)
    {
        int trialIndex = trialNumber - 1;
        dB_SPLData[trialIndex] = dB_SPL;
    }

    public bool wasThereReversal()
    {
        int trialIndex = trialNum - 1;

        if (trialIndex > 3)
        {
            if (wasHeard[trialIndex] && !wasHeard[trialIndex - 1] && !wasHeard[trialIndex - 2]) //2-up 1-down
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }
}