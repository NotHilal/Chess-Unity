using System.Collections;
using System;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    MAKE_MOVE = 4,
    REMATCH = 5,
    UPGRADE=6,
    FF=7,
    TIMER=8,
    REPET=9,
    ASKTIE=10,
    CHAT_MESSAGE=11,
}
public static class NetUtility 
{
    public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null;
        var opCode =(OpCode)stream.ReadByte();
        switch(opCode)
        {
            case OpCode.KEEP_ALIVE: msg = new NetKeepAlive(stream); break;
            case OpCode.WELCOME: msg = new NetWelcome(stream); break;
            case OpCode.START_GAME: msg = new NetStartGame(stream); break;
            case OpCode.MAKE_MOVE: msg = new NetMakeMove(stream);break;
            case OpCode.REMATCH: msg = new NetRematch(stream); break;
            case OpCode.UPGRADE: msg = new NetUpgrade(stream); break;
            case OpCode.FF: msg = new NetFF(stream); break;
            case OpCode.TIMER: msg = new NetTimer(stream); break;
            case OpCode.REPET: msg = new NetRepet(stream); break;
            case OpCode.ASKTIE: msg = new NetAskPat(stream); break;
            case OpCode.CHAT_MESSAGE: msg = new NetSendMsg(stream); break;
            default:
                Debug.LogError("Message recieved had no OpCode");
                break;
        }
        if(server != null)
        { 
            msg.ReceivedOnServer(cnn);
        }
        else
        {
            msg.ReceivedOnClient();
        }
    }

    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_MAKE_MOVE;
    public static Action<NetMessage> C_REMATCH;
    public static Action<NetMessage> C_UPGRADE;
    public static Action<NetMessage> C_EGALITE;
    public static Action<NetMessage> C_FF;
    public static Action<NetMessage> C_TIMER;
    public static Action<NetMessage> C_REPET;
    public static Action<NetMessage> C_ASKTIE;
    public static Action<NetMessage> C_CHAT_MESSAGE;

    public static Action<NetMessage,NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_MAKE_MOVE;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;
    public static Action<NetMessage, NetworkConnection> S_UPGRADE; 
    public static Action<NetMessage, NetworkConnection> S_EGALITE;
    public static Action<NetMessage, NetworkConnection> S_FF;
    public static Action<NetMessage, NetworkConnection> S_TIMER;
    public static Action<NetMessage, NetworkConnection> S_REPET;
    public static Action<NetMessage, NetworkConnection> S_ASKTIE;
    public static Action<NetMessage, NetworkConnection> S_CHAT_MESSAGE;
}
