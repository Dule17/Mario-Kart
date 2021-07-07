using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ChatHandler : MonoBehaviour
{
    public static ChatHandler instance;
    public int MaxMessages=25;
    [SerializeField]
    List<Message> messageList = new List<Message>();
   public   GameObject chatPanel, text;
    public TMP_InputField MessageBox;
    public TMPro.TextMeshProUGUI chatType;
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
    public void displayMessage(int id,string msg,string type)
    {
        if (messageList.Count > MaxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.RemoveAt(0);
        }

        Message m = new Message();
        m.text = msg;
        if (id == Client.instance.myId)
        {
            m.text = "me:"+msg;
        }
        
        GameObject newText = Instantiate(text, chatPanel.transform);
        m.textObject = newText.GetComponent<TextMeshProUGUI>();
        m.textObject.text = m.text;
        if (type != "ALL") m.textObject.color =Color.yellow;
            messageList.Add(m);
    }


    public void changeMessageType()
    {
        string[] msg = MessageBox.text.Split(' ');
        if (msg.Length > 0 && msg[0] == "/private")
        {
            chatType.text = msg[1];
            MessageBox.image.color = Color.yellow;
            MessageBox.text = "";
            return;
        }
        if (msg.Length > 0 && msg[0] == "/public")
        {
            chatType.text = "ALL";
            MessageBox.image.color = Color.white;
            MessageBox.text = "";
            return;
        }
    }
    public void SendMessage()
    {
        if (MessageBox.text != "")
        {
            displayMessage(Client.instance.myId, MessageBox.text, chatType.text);
            if (chatType.text == "ALL") ClientSend.publicChatReceived(Client.instance.myUsername + ":" + MessageBox.text);
            else ClientSend.privateChatReceived(Client.instance.myUsername + ":" + MessageBox.text, chatType.text);
            MessageBox.text = "";
        }
    }
    [System.Serializable]
    public class Message
    {
        public string text;
        public TextMeshProUGUI textObject;
    }
}
