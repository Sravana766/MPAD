using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PTADisclaimerManager : MonoBehaviour
{
    private Button Ready;
    private Text Information;

    // Start is called before the first frame update
    void Start()
    {
        Ready = GameObject.Find("Ready").GetComponent<Button>();
        Information = GameObject.Find("Information").GetComponent<Text>();

        Ready.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * (250f / 1000));
        Ready.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * (450f / 600));
        Ready.GetComponent<RectTransform>().position = new Vector3(Screen.width * (3f / 6), Screen.height * 300f / 1000, 0);

        Information.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * (600f / 1000));
        Information.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * (500f / 600));
        Information.GetComponent<RectTransform>().position = new Vector3(Screen.width * (3f / 6), Screen.height * 600f / 1000, 0);

    }

    public void StartTrial()
    {
        SceneManager.LoadScene("PTAEarSelection");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
