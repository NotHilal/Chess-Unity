using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetRepet : NetMessage
{
    public int teamId;
    public int type;
    public NetRepet()  //Creating the "box"
    {
        Code = OpCode.REPET;
    }
    public NetRepet(DataStreamReader reader) // Receiving the "box"
    {
        Code = OpCode.REPET;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamId);
        writer.WriteInt(type);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamId = reader.ReadInt();
        type = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_REPET?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_REPET?.Invoke(this, cnn);
    }
}

