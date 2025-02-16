using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    #region Singlteon implementation
    public static Client Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }
    #endregion

    public NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;

    public Action connectionDropped;


    #region Methods
    public void Init( string ip, ushort port)
    {

        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(ip,port);

        connection = driver.Connect(endpoint);
        Debug.Log("Attempting to connect to the Server : " + endpoint.Address);
        isActive = true;

       RegisterToEvent();
    }

    public void ShutDown()
    {
        if (isActive)
        {
            UnregisterToEvent();
            driver.Dispose();
            isActive = false;
            connection =default(NetworkConnection);
        }
    }

    public void OnDestroy()
    {
        ShutDown();
    }

    public void Update()
    {
        if (!isActive)
        {
            return;
        }

        driver.ScheduleUpdate().Complete();

        CheckAlive();
        UpdateMessagePump();
    }
    private void CheckAlive()
    {
        if(!connection.IsCreated && isActive)
        {
            Debug.Log("Something went wrong.. Lost connection to the server");
            connectionDropped?.Invoke();
            ShutDown();
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver,out stream))!=NetworkEvent.Type.Empty)
        {
            if(cmd==NetworkEvent.Type.Connect)
            {
                //var s = new NetWelcome();
                //s.AssignedTeam = 0;
                SendToServer(new NetWelcome());
                Debug.Log("We're connected youhouuu");
            }
            else if(cmd==NetworkEvent.Type.Data)
            {
                NetUtility.OnData(stream, default(NetworkConnection));
            }
            else if(cmd==NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                connection =default(NetworkConnection);
                connectionDropped?.Invoke();
                ShutDown();
            }
        }



    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection,out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }
    #endregion

    #region EventParsing

    private void RegisterToEvent()
    {
         NetUtility.C_KEEP_ALIVE +=OnKeepAlive;
    }

    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE-= OnKeepAlive;
    }

    private void OnKeepAlive(NetMessage nm)
    {
        //Send it back, to both side alive
        SendToServer(nm);
    }
    #endregion
}
