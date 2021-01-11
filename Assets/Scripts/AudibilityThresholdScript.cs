using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;


public class AudibilityThresholdScript : MonoBehaviour
{
    //Add in Maximum Comfortable Level MCL
    //Make a calibration file or something, and in that file will be 2 numbers
    //Singal RMS of 1 at 1 kHz, what dB_SPL is it?
    //Read in number into the app
    //.txt file with calibration #

    /* Step 1: Play sound.
     * Step 2: If they hear it, they can press the button hear. If not, press Did not hear.
     * Step 3: If heard, 10 dB_SPL down. If not, 5 dB_SPL up.
     * Step 4: Check for a reversal. Boolean true if 2 up, 1 down.
     * Step 5: Record the dB_SPL that was the threshold.
     * Step 5: Have them repeat 6 more times crossing threshold 
     * Step 6: Calculate median and that's the threshold.
     * 
     * 
     * So for each trial:
     * 1. Generate Sound
     * 2. Play Sound
     * 3. Wait for User Response
     * 4. 10 dB_SPL down or 5 dB_SPL up
     * 5. 1 sec pause, then repeat
     * */

    // Start is called before the first frame update
    private AudioSource audioSource;
    private AudioPlayerUpdated audioPlayer;


    //Store these variables in database
    private float prevdb_SPL;
    private float currentdB_SPL;
    public int trialNum;
    public int totalNumOfReversals;
    private AudibilityData dataObject = new AudibilityData();

    //These are teporary
    private bool reversal;
    private bool wasPrevTrialHeard;
    private bool wasPrevPrevTrialHeard;
    private bool continueTrial;
    private float trialTime;
    private float trialStartTime;
    private float trialEndTime;
    private bool lockbutton;
    private Button Heard;
    private Button Abort;
    private Text dB_Text;
    private Text reversals_Text;
    private Text instructions;
    private bool waitToStart;
    private float volume;
    private float left;
    private float right;
    private float sampleOffset;
    private float samplePitch;
    private EarSelection selectedEar = new EarSelection();

    //Time storage variables
    private string trialStartTimeString;
    private string trialEndTimeString;


    //For Calibration/Sound Manipulation
    private float interPulseInterval;
    private float initialDelay;
    private float dBStepSizeUp;
    private float dBStepSizeDown;

    //For RMS Manipulation
    private float caldBSPL;
    private int numSamples;
    private float[] samples;
    private float sum;
    private float rmsValue;
    private float adjustedDBValue;
    private float[] calibratedSamples;
    private float MCL;
    private float[] originalSamples;

    //Data transmission
    public Login loginScript;

    void Start()
    {
        
        interPulseInterval = 0f;
        initialDelay = 5f;
        dBStepSizeUp = 5;
        dBStepSizeDown = 10f;
        waitToStart = true; //Want to wait 10 sec before first trial
        
        left = -90f;
        right = 90f;

        lockbutton = true;
        Heard = GameObject.Find("Heard").GetComponent<Button>();
        dB_Text = GameObject.Find("dB_Text").GetComponent<Text>();
        reversals_Text = GameObject.Find("reversals_Text").GetComponent<Text>();
        instructions = GameObject.Find("Instructions").GetComponent<Text>();
        Abort = GameObject.Find("Abort").GetComponent<Button>();

        //Set UI
        SetUI();

        audioSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        originalSamples = new float[audioSource.clip.samples];
        audioSource.clip.GetData(originalSamples, 0);

        audioPlayer = new AudioPlayerUpdated(audioSource,-90f);

        //For localization to ear
        if (selectedEar.leftEar)
        {
            Debug.Log("Left Selected");
            audioPlayer.SetOffsetAngle(left); //left ear selected
        }

        else 
        {
            Debug.Log("Right Selected");
            audioPlayer.SetOffsetAngle(right); //right ear selected
        }



        // Being read from the sound meter.
        caldBSPL = 103; 
        currentdB_SPL = 55;
        //Want to start 70, 80, 90 dBSPL clinically for hearing loss
        //but we geared towards mild-to-moderate hearing loss
        //start out at 70 dBSPL!
        //go all the way down to 10, 5 dBSPL
        //quantization noise -- too close to 16 bit floor
        //peak clipping -- exceeded amplitude 1

        //play initially 65 & bring more more down
        //rms of 1 is roughly 80 dBSPL
        //normalizing to 0.06, so even in peakiest file, max amplitude is smaller than 1
        //for 0.06: 24 dB softer 20log10(1/0.06) --> 25 dB apart
        
        ChangeCalibratedVolume(currentdB_SPL); //To RMS normalize
        continueTrial = true;


        StartNewTrial();

        //for data insertion
        loginScript = GameObject.Find("LoginScript").GetComponent<Login>();


    }

    private void SetUI() {
        Heard.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * (250f / 1000));
        Heard.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * (300f / 600));
        Heard.GetComponent<RectTransform>().position = new Vector3(Screen.width * (3f / 6), Screen.height * 400f / 1000, 0);


        Abort.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * (100f / 1000));
        Abort.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * (200f / 600));
        Abort.GetComponent<RectTransform>().position = new Vector3(Screen.width * (4f / 6), Screen.height * 900f / 1000, 0);

        dB_Text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * (250f / 1000));
        dB_Text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * (300f / 600));
        dB_Text.GetComponent<RectTransform>().position = new Vector3(Screen.width * (3f / 6), Screen.height * 100f / 1000, 0);

        reversals_Text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * (250f / 1000));
        reversals_Text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * (300f / 600));
        reversals_Text.GetComponent<RectTransform>().position = new Vector3(Screen.width * (3f / 6), Screen.height * 50f / 1000, 0);

        instructions.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * (250f / 1000));
        instructions.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * (550f / 600));
        instructions.GetComponent<RectTransform>().position = new Vector3(Screen.width * (3f / 6), Screen.height * 666f / 1000, 0);

        trialNum = 0; //1st trial is Trial 1, trialNum 2 corresponds to Trial 2; index for arrays are - 1
        totalNumOfReversals = 0;
    }

    void StartNewTrial()
    {

        if (continueTrial)
        {
            lockbutton = true;
            trialNum++;
            //audioSource.Play();

            StartCoroutine(PlayThreeTrials());

            trialStartTime = Time.time;
            trialStartTimeString = System.DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd hh:mm:ss");

            trialTime = ((3 * audioSource.clip.length) + 3*interPulseInterval) + 3.0f + (Random.value * 3.0f); //assign random time

            if (trialNum > 3)
            {
                if (dataObject.wasThereReversal())
                {
                    totalNumOfReversals++;
                    dataObject.SetNumReversals(totalNumOfReversals);
                }
            }
        }

        if (totalNumOfReversals > 6)
        {
            continueTrial = false;
        }             //else continueTrial is true
    }

    IEnumerator PlayThreeTrials()
    {
        if (waitToStart)
        {
            yield return new WaitForSeconds(initialDelay);
            waitToStart = false;
            trialStartTime += 5;
        }
        audioPlayer.Play();
        yield return new WaitForSeconds(audioSource.clip.length + interPulseInterval);
        audioPlayer.Play();
        yield return new WaitForSeconds(audioSource.clip.length + interPulseInterval);
        audioPlayer.Play();
        yield return new WaitForSeconds(audioSource.clip.length + interPulseInterval);
        lockbutton = false;
    }

    public void OnHeardB_SPLuttonClick()
    {
        if (!lockbutton && continueTrial)
        {
            trialEndTime = Time.time;
            trialEndTimeString = System.DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd hh:mm:ss");

            //if heard
            prevdb_SPL = currentdB_SPL;
            dataObject.SetWasHeard(trialNum, true);
            currentdB_SPL -= dBStepSizeDown;
            dataObject.SetdB_SPLData(trialNum, currentdB_SPL);

           

            //audioSource.volume = DecibelToLinear(currentdB_SPL);
            //ChangeVolume(-1 * dBStepSizeDown);
            ChangeCalibratedVolume(currentdB_SPL);

            dB_Text.text = "Current dB: " + currentdB_SPL.ToString();
            reversals_Text.text = "Reversals (there is a slight lag): " + totalNumOfReversals.ToString();

            //insert data
            if (selectedEar.leftEar == true)
            {
                loginScript.insertTrial_info(100, "userID", "deviceID", "PTA", "L", trialNum, trialStartTimeString, trialEndTimeString, 1000, true, prevdb_SPL, caldBSPL, dataObject.wasThereReversal());
            }
            else 
            {
                loginScript.insertTrial_info(100, "userID", "deviceID", "PTA", "R", trialNum, trialStartTimeString, trialEndTimeString, 1000, true, prevdb_SPL, caldBSPL, dataObject.wasThereReversal());
            }
            StartNewTrial();
        }
    }

    public void AbortTrial()
    {
        selectedEar.leftEar = false;
        selectedEar.rightEar = false;
        SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        //check time

        
        if (Time.time - trialStartTime > trialTime && continueTrial)
        {
            trialEndTime = Time.time;
            //means sound was not heard
            prevdb_SPL = currentdB_SPL;
            dataObject.SetWasHeard(trialNum, false);
            currentdB_SPL += dBStepSizeUp;
            dataObject.SetdB_SPLData(trialNum, currentdB_SPL);

            

            //audioSource.volume = DecibelToLinear(currentdB_SPL);
            //ChangeVolume(dBStepSizeUp);
            ChangeCalibratedVolume(currentdB_SPL);

            dB_Text.text = "Current dB: " + currentdB_SPL.ToString();
            reversals_Text.text = "Reversals (there is a slight lag): " + totalNumOfReversals.ToString();

            //insert data
            if (selectedEar.leftEar == true)
            {
                loginScript.insertTrial_info(100, "userID", "deviceID", "PTA", "L", trialNum, trialStartTimeString, trialEndTimeString, 1000, true, prevdb_SPL, caldBSPL, dataObject.wasThereReversal());
            }
            else
            {
                loginScript.insertTrial_info(100, "userID", "deviceID", "PTA", "R", trialNum, trialStartTimeString, trialEndTimeString, 1000, true, prevdb_SPL, caldBSPL, dataObject.wasThereReversal());
            }

            StartNewTrial();
        }

        UIHelper.OnBackButtonClickListener("MainMenu");
    }

    public void ChangeCalibratedVolume(float desiredDB)
    {
        MCL = 80;
        if (desiredDB > MCL)         //Maximum Comfortable Level
            desiredDB = MCL; //FLASH USER WARNING; YOUR LEVEL IS TOO LOUD & we have to change to maximum permitted level

        //This line can be hard-coded for each iPad, or it can read in a file later
        //Change calibration to caldBSPL
        //caldBSPL = 30; //Add in calibration 

        numSamples = audioSource.clip.samples;
        samples = new float[numSamples];
        audioSource.clip.GetData(samples, 0);
        //we need to do things with reference to the original clip, not the current one

        //for loop assigning the samples array values from the audio clip

        //string path = "Assets/Resources/test_calibratedsamples.txt";

        //Write some text to the test.txt file
        //StreamWriter writer = new StreamWriter(path, true);
        //writer.WriteLine("1");
        //writer.Close();


        sum = 0;
        for (int i = 0; i < numSamples; i++)
        {
            sum += originalSamples[i] * originalSamples[i];
        }

        rmsValue = Mathf.Sqrt(sum / numSamples);

        //Calculate dB
        //adjustedDBValue = 20 * Mathf.Log10(caldBSPL) - desiredDB;
        //unity_y = unity_y/sqrt(mean(unity_y.^2)) * 10^((refdBSPL-desireddBSPL)/20)
        //dBValue = 20 * Mathf.Log10(rmsValue/SomeRefValue that I don't know?)
        
        //Adjusting based on calibration
        calibratedSamples = new float[numSamples];
        for (int i = 0; i < numSamples; i++) {
            calibratedSamples[i] = originalSamples[i]/(rmsValue)* Mathf.Pow(10, (desiredDB - caldBSPL)/20);  //x2 = x/(rms)*20*log10(CalibratedNum - desireddB)
            //Debug.Log(calibratedSamples[i]);

            //writer.WriteLine(calibratedSamples[i].ToString());

        }

        Debug.Log(caldBSPL);
        Debug.Log(desiredDB);

        //writer.Close();

        //Actually assigning dBSPL correction
        audioSource.clip.SetData(calibratedSamples, 0);

        //NEED TO FIX AUDIOPLAYER AFTER THIS TOO!

        //TO DO
        //Need separate calibration that takes output of headphones & ensures that a flat frequency output is observed when a flat freqeuency is inputted
        //Inverse filtering; need to ensure no frequency shaping


        //ABORT BUTTON needs to record Abort output in database!
        //MCL needs to have a flashing screen!

        //Pre-selection for left ear or right year
        //Automatic or manual, manual you choose frequency & ear
        //Automatic: automated screening L then R
        /*
         * 
         * Add inverse filter to PTA
        by including dB attenuation that is ear and frequency specific
        lookup table
        */

        //Tell mamun database for fetching calibration #'s for DeviceID
        //Need to add permission screen maximum

        //DISABLE HRTF'S IN UNITY
    }

    public float LookUpTable(float frequency, bool isLeftEar) {
        float attenuation;
        attenuation = 0f;

        if (frequency == 250f && isLeftEar)
            return 5f;
        else if (frequency == 250f && !isLeftEar)
            return 0f;

        else if (frequency == 500f && isLeftEar)
            attenuation = 1;
        else if (frequency == 500f && !isLeftEar)
            attenuation = 1;

        else if (frequency == 1000f && isLeftEar)
            attenuation = 1;
        else if (frequency == 1000f && !isLeftEar)
            attenuation = 1;

        else if (frequency == 2000f && isLeftEar)
            attenuation = 1;
        else if (frequency == 2000f && !isLeftEar)
            attenuation = 1;

        else if (frequency == 4000f && isLeftEar)
            attenuation = 1;
        else if (frequency == 4000f && !isLeftEar)
            attenuation = 1;

        else if (frequency == 8000f && isLeftEar)
            attenuation = 1;
        else if (frequency == 8000f && !isLeftEar)
            attenuation = 1;

        else
            attenuation = 0f;

        return attenuation;

    }

}

//Old:
/*
 *     private float LinearToDecibel(float linear)
    {
        float dB_SPL;

        if (linear != 0)
            dB_SPL = 20.0f * Mathf.Log10(linear);
        else
            dB_SPL = -144.0f;

        return dB_SPL;
    }

    private float DecibelToLinear(float dB_SPL)
    {
        float linear = Mathf.Pow(10.0f, dB_SPL / 20.0f);

        return linear;
    }
 * 
 * 
 */

