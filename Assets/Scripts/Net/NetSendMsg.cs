using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetSendMsg : NetMessage
{
    public string ChatMsg;
    public int CurrentTeam;
    NativeArray<byte> chaine;

    public NetSendMsg(string msg)
    {
        Code = OpCode.CHAT_MESSAGE;
        ChatMsg = msg;
    }

    public NetSendMsg(DataStreamReader reader)
    {
        Code = OpCode.CHAT_MESSAGE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(CurrentTeam);
        writer.WriteBytes(chaine);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        CurrentTeam = reader.ReadInt();
        FixedString128Bytes a = reader.ReadFixedString128();
        ChatMsg = a.ToString();
        Debug.Log("Msg : " + a);
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_CHAT_MESSAGE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_CHAT_MESSAGE?.Invoke(this, cnn);
    }
}
