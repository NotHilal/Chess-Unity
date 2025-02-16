using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetAskPat : NetMessage
{
    public int teamId;
    public NetAskPat()  //Creating the "box"
    {
        Code = OpCode.ASKTIE;
    }
    public NetAskPat(DataStreamReader reader) // Receiving the "box"
    {
        Code = OpCode.ASKTIE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamId);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamId = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_ASKTIE?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_ASKTIE?.Invoke(this, cnn);
    }
}


