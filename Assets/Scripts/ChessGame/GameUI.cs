using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CameraAngle
{
    menu =0,
    whiteTeam =1, 
    BlackTeam=2,
    EndGame = 3
}

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { set; get; }

    public Server server;
    public Client client;

    [SerializeField] private Animator menuAnimator;
    [SerializeField] private TMP_InputField adressInput;
    [SerializeField] private GameObject[] cameraAngles;
    
    public Action<bool> SetLocalGame;


    private void Awake()
    {
        Instance = this;
        RegisterEvents();
    }
    

    #region Camera

    public void ChangeCamera(CameraAngle index)
    {
        for (int i = 0; i < cameraAngles.Length; i++)
        {
            cameraAngles[i].SetActive(false);
        }
        cameraAngles[(int)index].SetActive(true);
    }

    #endregion

    #region Buttons

    public void OnLocalGameButton()
    {
        
        menuAnimator.SetTrigger("InGameMenu");
        SetLocalGame?.Invoke(true);
        server.Init(8008);
        client.Init("127.0.0.1", 8008);
    }
    public void OnOnlineGameButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }
    public void OnOnlineHostButton()
    {
        SetLocalGame?.Invoke(false);
        server.Init(8008);
        client.Init("127.0.0.1", 8008);
        menuAnimator.SetTrigger("HostMenu");

    }
    public void OnOnlineConnectButton()
    {
        SetLocalGame?.Invoke(false);
        client.Init(adressInput.text, 8008);

    }
    public void OnOnlineBackButton()
    {
        menuAnimator.SetTrigger("StartMenu");
    }
    public void OnOnlineLeaderboard()
    {
        menuAnimator.SetTrigger("LeaderBoardMenu");
    }
    public void OnHostBackButton()
    {
        server.ShutDown();
        client.ShutDown();
        menuAnimator.SetTrigger("OnlineMenu");
    }

    public void OnLeaveFromGame()
    {
        ChangeCamera(CameraAngle.menu);
        menuAnimator.SetTrigger("StartMenu");
    }

    #endregion

    #region RegisterEvents

    private void RegisterEvents()
    {
        NetUtility.C_START_GAME += OnStartGameClient;
    }

    private void UnregisterEvents()
    {
        NetUtility.C_START_GAME -= OnStartGameClient;
    }

    private void OnStartGameClient(NetMessage obj)
    {
        menuAnimator.SetTrigger("InGameMenu");
    }

    #endregion
}

