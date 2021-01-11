using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
public class Login : MonoBehaviour
{
    public string myURL;
    public string User_ID;
    public string User_Password;
    public GameObject UserID;
    public GameObject Password;
    public void login()
    {
        User_ID = UserID.GetComponent<Text>().text;
        User_Password = Password.GetComponent<Text>().text;
        myURL = "http://3.15.42.81/test.php?funcname=login&username=";
        myURL += User_ID + "&password=" + User_Password;
        StartCoroutine(sendRequest("loginCheck", myURL));
        //username: user
        //password: user
        //insertTrial_info(100, 2, 10, 11, 5, true, 10, false); // example of an insert function : for testing always use session id 100
    }

    //--------------------------------------Function to insert trial info into database

    public void insertTrial_info(int SessionNumber, string userID, string deviceID, string task, string ear, int trialNum, string startTime, string endTime, int stimulusID, bool userResponse, float dBSPL, float calibrationdBSPL, bool wasItReversal)
    {
     myURL = "http://3.15.42.81/index.php?funcname=insertTrial_info&SessionNumber=" + SessionNumber + "&trialNum=" + trialNum + "&startTime=" + startTime + "&endTime=" + endTime + "&stimulusID=" + stimulusID + "&userResponse=" + userResponse + "&dBSPL=" + dBSPL + "&wasItReversal=" + wasItReversal+"&userID="+ userID+ "&deviceID=" + deviceID + "&task=" + task + "&ear="+ ear + "&calibrationdBSPL="+ calibrationdBSPL;
     StartCoroutine(sendRequest("insertTrial_info", myURL));
 }
 
    //-------------------
    IEnumerator sendRequest(string type, string myURL)
    {
        UnityWebRequest www = UnityWebRequest.Get(myURL);
        yield return www.SendWebRequest();
        if (www.downloadHandler.text == "1" && type == "loginCheck")
        {
            SceneManager.LoadScene(1);
        }
        else if (www.downloadHandler.text == "1" && type == "insertTrial_info")
        {
            Debug.Log("Successfully inserted");
        }
        else
        {
            Debug.Log("Failed " + type);
        }
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

}

// https://www.youtube.com/watch?v=nVz3GBw1kDg
//https://drive.google.com/file/d/1izYPov3QMzwWpUBsT9CD25vLKfIIv6Qk/view