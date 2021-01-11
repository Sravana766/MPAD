using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EarSelection : MonoBehaviour
{
    public Button left;
    public Button right;
    public bool leftEar = false;
    public bool rightEar = false;
    // Start is called before the first frame update
    void Start()
    {
        InitializeButtons();
    }
    public void InitializeButtons()
    {
        left = GameObject.Find("Left").GetComponent<Button>();
        left.onClick.AddListener(WhenClickedLeft);
        right = GameObject.Find("Right").GetComponent<Button>();
        right.onClick.AddListener(WhenClickedRight);
    }

    void WhenClickedLeft()
    {
        leftEar = true;
        Debug.Log("Set Left true");
        SceneManager.LoadScene("AudibilityThresholdTest");
    }

    void WhenClickedRight()
    {
        rightEar = true;
        Debug.Log("Set Right true");
        SceneManager.LoadScene("AudibilityThresholdTest");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
