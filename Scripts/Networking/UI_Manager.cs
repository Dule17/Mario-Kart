using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;
    public GameObject loginMenu;
    public TMP_InputField UsernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField ipAdressInput;
    public TextMeshProUGUI txtMessage;
    public string ButtonType;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance exits!");
            Destroy(this);
        }

    }

    public void SetButtonRegister()
    {
        ButtonType = "Register";
    }
    public void SetButtonLogin()
    {
        ButtonType = "Login";
    }
    public void connectToServer()
    {
        /*  loginMenu.SetActive(false);
          UsernameInput.interactable = false;
          passwordInput.interactable = false;*/
        if (UsernameInput.text != "" && passwordInput.text != "") Client.instance.connectToServer();
        else txtMessage.text = "Username or password empty";
    }

    public void onChange()
    {
        Client.instance.ip = ipAdressInput.text;
        Client.instance.SetTCPandUDP();
    }
}
