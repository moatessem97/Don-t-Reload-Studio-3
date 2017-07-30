using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/*
THE FOLLOWING SCRIPT WAS WRITTEN BY DAVID ROSEN. IT IS FREE TO USE WITH ACKNOWLEDGEMENT OF THE CREATOR.
IT IS INTENDED FOR USE WITH AWARD SPACE (https://www.awardspace.net/), PHP and MySQL
*/

public class SQLLogin : MonoBehaviour
{
    //THE FOLLOWING ARE REFERENCES TO THE UI INPUT FIELDS THAT THE PLAYER TYPES INTO
    public Text usernameText, passwordText, regNameText, regUsernameText, regPassText, regConfPassText, regEmailText;
    public Text messageText; //THIS IS THE TEXT TO BE DISPLAYED ON SCREEN TO THE PLAYER

    public GameObject canvas;

    //THE FOLLOWING VARIBALES ARE POPULATED BY THE TEXT THE PLAYER ENTERS INTO THE CORRESPONDING INPUT FIELDS
    private string username, password, regName, regUsername, regPass, regConfPass, regEmail;

    //FUNCTION TO BE CALLED VIA THE UI BUTTON
    public void LogIn()
    {
        messageText.text = ""; //CLEAR ANY DISPLAYED MESSAGES TO THE PLAYER

        username = usernameText.text;    //POPULATE THE PRIVATE username VARIABLE WITH THE TEXT THE PLAYER ENTERED INTO THE usernameText INPUT FIELDo
        password = passwordText.text;    //POPULATE THE PRIVATE password VARIABLE WITH THE TEXT THE PLAYER ENTERED INTO THE passwordText INPUT FIELD

        if (username == "" || password == "") //IF THE PLAYER HASN'T ENTERED THE REQUIRED INFORMATION...TELL THEM TO
            messageText.text = "Please complete all fields.";
        else    //IF ALL INFORMATION IS ENTERED, BUILD A WWWForm AND SEND IT TO THE SERVER
        {
            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);
            WWW w = new WWW("http://dontreload.000webhostapp.com/login.php", form);    //REPLACE ?????????? WITH YOUR AWARD SPACE DOMAIN
            StartCoroutine(LogIn(w));
        }
    }
    //FUNCTION TO BE CALLED VIA THE UI BUTTON
    public void Register()
    {
        messageText.text = ""; //CLEAR ANY DISPLAYED MESSAGES TO THE PLAYER

        regName = regNameText.text;    //POPULATE THE PRIVATE regName VARIABLE WITH THE TEXT THE PLAYER ENTERED INTO THE regNameText INPUT FIELD
        regUsername = regUsernameText.text;    //POPULATE THE PRIVATE regUsername VARIABLE WITH THE TEXT THE PLAYER ENTERED INTO THE regUsernameText INPUT FIELD
        regPass = regPassText.text;    //POPULATE THE PRIVATE USERNAME regPass WITH THE TEXT THE PLAYER ENTERED INTO THE pregPassText INPUT FIELD
        regConfPass = regConfPassText.text;    //POPULATE THE PRIVATE regConfPass VARIABLE WITH THE TEXT THE PLAYER ENTERED INTO THE regConfPassText INPUT FIELD
        regEmail = regEmailText.text;    //POPULATE THE PRIVATE regEmail VARIABLE WITH THE TEXT THE PLAYER ENTERED INTO THE regEmailText INPUT FIELD

        if (regName == "" || regUsername == "" || regPass == "" || regConfPass == "" || regEmail == "") //IF THE PLAYER HASN'T ENTERED THE REQUIRED INFORMATION...TELL THEM TO
            messageText.text = "Please complete all fields.";
        else    //IF ALL INFORMATION IS ENTERED......
        {
            if (regPass == regConfPass)    //......AND THE PASSWORDS MATCH, BUILD A WWWForm AND SEND IT TO THE SERVER
            {
                WWWForm form = new WWWForm();
                form.AddField("username", regUsername);
                form.AddField("name", regName);
                form.AddField("email", regEmail);
                form.AddField("password", regPass);
                WWW w = new WWW("http://dontreload.000webhostapp.com/register.php", form);    //REPLACE ?????????? WITH YOUR AWARD SPACE DOMAIN
                StartCoroutine(Register(w));
            }
            else    //......AND THE PASSWORDS DON'T MATCH, TELL THE PLAYER
                messageText.text = "Your passwords do not match.";
        }
    }

    //WE USE COROUTINES TO SEND INFORMATION TO THE SERVER, SO THAT WE CAN WAIT FOR A RESPONSE
    private IEnumerator LogIn(WWW _w)
    {
        yield return _w;    //WAIT FOR A RESPONSE FROM THE SERVER

        if (_w.error == null)    //IF THE SERVER DOESN'T SEND BACK AN ERROR
        {
            if (_w.text == " Log In Successful. ")    //THE PHP SCRIPT SUPPLIED WILL SEND THIS MESSAGE BACK IF THE LOGIN WAS SUCCESSFUL
            {
                // WHAT HAPPENS WHEN THE PLAYER LOGS IN
                Debug.Log("shizlogged");
                canvas.GetComponent<PhotonConnect>().ConnectToRoom();
            }
            else
                messageText.text = _w.text;    //THE PHP SCRIPT SUPPLIED WILL TELL THE PLAYER IF THEIR PASSWORD IS INCORRECT, OR IF THEIR USERNAME DOESN'T EXIST
        }
        else
            messageText.text = "ERROR: " + _w.error;    //IF THERE IS AN ERROR (SUCH AS THE SERVER BEING DOWN) THE PHP SCRIPT SUPPLIED WILL TELL THE PLAYER
    }
    private IEnumerator Register(WWW _w)
    {
        yield return _w;    //WAIT FOR A RESPONSE FROM THE SERVER

        if (_w.error == null)    //IF THE SERVER DOESN"T SEND BACK AN ERROR
            messageText.text = _w.text;        //THE PHP SCRIPT SUPPLIED WILL SEND A MESSAGE BACK TO THE PLAYER SAYING REGISTRATION WAS COMPLETED
        else
            messageText.text = "ERROR: " + _w.error;    //IF THERE IS AN ERROR (SUCH AS THE SERVER BEING DOWN) THE PHP SCRIPT SUPPLIED WILL TELL THE PLAYER
    }
}