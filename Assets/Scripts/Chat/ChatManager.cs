using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{

    public int MaxMsg = 25;

    [SerializeField] 
    private List<Message> messages = new List<Message>();
    public string Username;
    public GameObject ChatBg;
    public GameObject OpenC;
    public GameObject CloseC;
    bool isopen =false;
    public ChessBoard board;

    public GameObject ChatPanel;
    public GameObject textobject;
    public InputField Chatbox;

    // Start is called before the first frame update
    void Start()
    {
        ChatBg.SetActive(false);
        CloseC.SetActive(false);
        OpenC.SetActive(true);
        if (SaveUsername2.manager != null)
        {
            if(SaveUsername2.manager.Username.Length!=0)
            {
                Username = SaveUsername2.manager.Username;
            }else
            {
                Username = SAveUsername.manager.Username;
            }
        }
        else
        {
            Username = SAveUsername.manager.Username;

        }
        
        
        
    }

    void Update()
    {
        if(Chatbox.text!="")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendDaMsg();
            }
        }
        if (isopen)
        {
            Chatbox.ActivateInputField();
        }
    }

    public void SendMsg(string text)
    {
        
        if(messages.Count>= MaxMsg)
        {
            Destroy(messages[0].textobj.gameObject);
            messages.Remove(messages[0]);
        }
        Message newmsg = new Message();
        newmsg.text = text;

        GameObject Newtext = Instantiate(textobject, ChatPanel.transform);

        newmsg.textobj = Newtext.GetComponent<Text>();
        newmsg.textobj.text = newmsg.text;
        messages.Add(newmsg);


    }
    public void SendDaMsg()
    {
        if(Chatbox.text.Length==0)
        {
            return;
        }
        SendMsg(Username + " : " + Chatbox.text);
        Chatbox.text = "";
        NetSendMsg msg = new NetSendMsg(Username + " : " + Chatbox.text);
        msg.CurrentTeam = board.currentTeam;
        msg.ChatMsg = Chatbox.text;
        Client.Instance.SendToServer(msg);
    }
     


    public void OpenChat()
    {
        OpenC.SetActive(false);
        CloseC.SetActive(true);
        ChatBg.SetActive(true);
        Chatbox.ActivateInputField();
        isopen = true;
    }
    public void CloseChat()
    {
        OpenC.SetActive(true);
        CloseC.SetActive(false);
        ChatBg.SetActive(false);
        isopen=false;
    }


    private void RegisterEvents()
    {
        NetUtility.S_ASKTIE += OnMsgReceivedServer;


        
        NetUtility.C_ASKTIE += OnMsgReceivedClient;
    }




    private void UnregisterEvents()
    {
        NetUtility.S_ASKTIE -= OnMsgReceivedServer;


        NetUtility.C_ASKTIE -= OnMsgReceivedClient;
    }
    public void OnMsgReceivedServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }
    public void OnMsgReceivedClient(NetMessage msg)
    {
        NetSendMsg mm = msg as NetSendMsg;
        if (mm.CurrentTeam!=board.currentTeam)
        {
            SendMsg(mm.ChatMsg);
        }
    }

}

[Serializable]
public class Message
{
    public string text;
    public Text textobj;
}
