using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Random = System.Random;

public class SpeechAndNoiseScript : MonoBehaviour
{
    public AudioSource targetSource;
    public AudioSource maskerSource;
    public AudioClip[] adClips;
    public Text text;
    public Button button0;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;
    public Button button8;
    public Button button9;

    AudioClip a;
    AudioClip b;

    List<int> maskerBandNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    int number;
    List<int> sounds = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 8, 9 };
    Random rand = new Random();
    List<int> targetBandNumbers = new List<int>();
    List<int> targetSounds = new List<int>();
    List<int> maskerSounds = new List<int>();
    List<int> chosenTargetFreq = new List<int>();
    List<int> chosenMaskerFreq = new List<int>();
    List<int> index = new List<int>();
    List<string> targetFileNames = new List<string>();
    List<string> maskerFileNames = new List<string>();
    bool notSame = true;
    bool isEqual;
    string fileName;
    int currentIndex = 0;
    List<string> testFileNames1 = new List<string>() { "masker_1_1_80_0_0_male_eng", "masker_1_2_80_0_0_male_eng", "masker_1_3_80_0_0_male_eng", "masker_1_4_80_0_0_male_eng", "masker_1_5_80_0_0_male_eng" };
    List<string> testFileNames2 = new List<string>() { "target_1_1_80_0_0_male_eng", "target_1_2_80_0_0_male_eng", "target_1_3_80_0_0_male_eng", "target_1_4_80_0_0_male_eng", "target_1_5_80_0_0_male_eng" };
    List<int> userResponse = new List<int>();
    string buttonText;
    int buttonNumber;
    int fileNumber = 0;

    public void PlayAllSounds()
    {
        targetSource = GameObject.Find("Control").GetComponent<AudioSource>();
        maskerSource = GameObject.Find("Control").GetComponent<AudioSource>();
        StartCoroutine(PlayTargetAndMaskerSound());
    }


    public IEnumerator PlayTargetAndMaskerSound()
    {
        for (int i = 0; i < 5; i++)
        {

            a = Resources.Load(testFileNames1[i]) as AudioClip;
            b = Resources.Load(testFileNames2[i]) as AudioClip;
            targetSource.clip = a;
            maskerSource.clip = b;
            maskerSource.PlayOneShot(b);
            targetSource.PlayOneShot(a);

            yield return new WaitForSeconds(1f);

        }
        changeTransperency(1f);

        Debug.Log("test");

    }

    public void createFileNames()
    {
        for (int i = 0; i < 8; i++)
        {
            number = rand.Next(0, 14);
            if (!index.Contains(number))
            {
                targetBandNumbers.Add(maskerBandNumbers[number]);
            }
            else
            {
                i--;
                continue;
            }
            index.Add(number);

        }
        for (int i = 0; i < 8; i++)
        {
            maskerBandNumbers.Remove(index[i]);
        }
        index.Clear();

        while (notSame)
        {
            for (int i = 0; i < 5; i++)
            {
                number = rand.Next(0, 8);
                if (!index.Contains(number))
                {
                    targetSounds.Add(sounds[number]);
                }
                else
                {
                    i--;
                    continue;
                }
                index.Add(number);
            }
            index.Clear();
            for (int i = 0; i < 5; i++)
            {
                number = rand.Next(0, 8);
                if (!index.Contains(number))
                {
                    maskerSounds.Add(sounds[number]);
                }
                else
                {
                    i--;
                    continue;
                }
                index.Add(number);
            }
            isEqual = Enumerable.SequenceEqual(targetSounds, maskerSounds);
            if (!isEqual)
            {
                notSame = false;
            }
        }
        index.Clear();
        for (int i = 0; i < 5; i++)
        {
            number = rand.Next(0, 8);
            if (!index.Contains(number))
            {
                chosenTargetFreq.Add(targetBandNumbers[number]);
            }
            else
            {
                i--;
                continue;
            }
            index.Add(number);
        }
        index.Clear();
        for (int i = 0; i < 5; i++)
        {
            number = rand.Next(0, 7);
            if (!index.Contains(number))
            {
                chosenMaskerFreq.Add(maskerBandNumbers[number]);
            }
            else
            {
                i--;
                continue;
            }
            index.Add(number);
        }
        for (int i = 0; i < 5; i++)
        {
            fileName = "target_" + targetSounds[i] + "_" + chosenTargetFreq[i] + "_80_0_0_male_eng";
            targetFileNames.Add(fileName);
        }
        for (int i = 0; i < 5; i++)
        {
            fileName = "masker_" + maskerSounds[i] + "_" + chosenMaskerFreq[i] + "_80_0_0_male_eng";
            maskerFileNames.Add(fileName);
        }
    }
    // Start is called before the first frame update


    public void InitializeButtons()
    {
        button0 = GameObject.Find("0").GetComponent<Button>();
        button0.onClick.AddListener(WhenClicked0);
        button1 = GameObject.Find("1").GetComponent<Button>();
        button1.onClick.AddListener(WhenClicked1);
        button2 = GameObject.Find("2").GetComponent<Button>();
        button2.onClick.AddListener(WhenClicked2);
        button3 = GameObject.Find("3").GetComponent<Button>();
        button3.onClick.AddListener(WhenClicked3);
        button4 = GameObject.Find("4").GetComponent<Button>();
        button4.onClick.AddListener(WhenClicked4);
        button5 = GameObject.Find("5").GetComponent<Button>();
        button5.onClick.AddListener(WhenClicked5);
        button6 = GameObject.Find("6").GetComponent<Button>();
        button6.onClick.AddListener(WhenClicked6);
        button8 = GameObject.Find("8").GetComponent<Button>();
        button8.onClick.AddListener(WhenClicked8);
        button9 = GameObject.Find("9").GetComponent<Button>();
        button9.onClick.AddListener(WhenClicked9);
    }

    public void WhenClicked0()
    {
        buttonText = button0.GetComponentInChildren<Text>().text;
        buttonNumber = Int32.Parse(buttonText);
        userResponse.Add(buttonNumber);
        text = GameObject.Find("user").GetComponent<Text>();
        text.GetComponent<UnityEngine.UI.Text>().text = "You Pressed: " + buttonText;
        checkCurrentIndexAnswer();

    }
    public void WhenClicked1()
    {
        buttonText = button1.GetComponentInChildren<Text>().text;
        buttonNumber = Int32.Parse(buttonText);
        userResponse.Add(buttonNumber);
        text = GameObject.Find("user").GetComponent<Text>();
        text.GetComponent<UnityEngine.UI.Text>().text = "You Pressed: " + buttonText;
        checkCurrentIndexAnswer();
    }
    public void WhenClicked2()
    {
        buttonText = button2.GetComponentInChildren<Text>().text;
        buttonNumber = Int32.Parse(buttonText);
        userResponse.Add(buttonNumber);
        text = GameObject.Find("user").GetComponent<Text>();
        text.GetComponent<UnityEngine.UI.Text>().text = "You Pressed: " + buttonText;
        checkCurrentIndexAnswer();
    }
    public void WhenClicked3()
    {
        buttonText = button3.GetComponentInChildren<Text>().text;
        buttonNumber = Int32.Parse(buttonText);
        userResponse.Add(buttonNumber);
        text = GameObject.Find("user").GetComponent<Text>();
        text.GetComponent<UnityEngine.UI.Text>().text = "You Pressed: " + buttonText;
        checkCurrentIndexAnswer();
    }
    public void WhenClicked4()
    {
        buttonText = button4.GetComponentInChildren<Text>().text;
        buttonNumber = Int32.Parse(buttonText);
        userResponse.Add(buttonNumber);
        text = GameObject.Find("user").GetComponent<Text>();
        text.GetComponent<UnityEngine.UI.Text>().text = "You Pressed: " + buttonText;
        checkCurrentIndexAnswer();
    }
    public void WhenClicked5()
    {
        buttonText = button5.GetComponentInChildren<Text>().text;
        buttonNumber = Int32.Parse(buttonText);
        userResponse.Add(buttonNumber);
        text = GameObject.Find("user").GetComponent<Text>();
        text.GetComponent<UnityEngine.UI.Text>().text = "You Pressed: " + buttonText;
        checkCurrentIndexAnswer();
    }
    public void WhenClicked6()
    {
        buttonText = button6.GetComponentInChildren<Text>().text;
        buttonNumber = Int32.Parse(buttonText);
        userResponse.Add(buttonNumber);
        text = GameObject.Find("user").GetComponent<Text>();
        text.GetComponent<UnityEngine.UI.Text>().text = "You Pressed: " + buttonText;
        checkCurrentIndexAnswer();
    }
    public void WhenClicked8()
    {
        buttonText = button9.GetComponentInChildren<Text>().text;
        buttonNumber = Int32.Parse(buttonText);
        userResponse.Add(buttonNumber);
        text = GameObject.Find("user").GetComponent<Text>();
        text.GetComponent<UnityEngine.UI.Text>().text = "You Pressed: " + buttonText;
        checkCurrentIndexAnswer();
    }
    public void WhenClicked9()
    {
        buttonText = button9.GetComponentInChildren<Text>().text;
        buttonNumber = Int32.Parse(buttonText);
        userResponse.Add(buttonNumber);
        text = GameObject.Find("user").GetComponent<Text>();
        text.GetComponent<UnityEngine.UI.Text>().text = "You Pressed: " + buttonText;
        checkCurrentIndexAnswer();
    }

    void Start()
    {
        createFileNames();
        PlayAllSounds();
        InitializeButtons();

    }
    private IEnumerator callInitializaButtonsAgain()
    {
        yield return new WaitForSeconds(1f);
        InitializeButtons();
    }

    private IEnumerator DelaySounds()
    {
        yield return new WaitForSeconds(5f);
    }

    public void checkCurrentIndexAnswer()
    {
        if (currentIndex != 4)
        {
            if (userResponse[currentIndex] == targetSounds[currentIndex])
            {
                currentIndex++;
                callInitializaButtonsAgain();
                Debug.Log(currentIndex);
            }
            else
            {
                Debug.Log("Wrong Answer");
                Debug.Log("Game Over");
                Application.Quit();
            }
        }
        else
        {
            if (userResponse[currentIndex] == targetSounds[currentIndex])
            {
                Debug.Log("You answered everything correctly!");
                Application.Quit();
            }
            else
            {
                Debug.Log("Wrong Answer");
                Debug.Log("Game Over");
                Application.Quit();
            }
        }
    }

    public void changeTransperency(float transperency)
    {
        for (int i = 0; i < 10; i++)
        {
            string buttonName = i.ToString();

            if (i != 7)
            {
                GameObject.Find(buttonName).GetComponent<CanvasGroup>().alpha = transperency;
            }
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
}
