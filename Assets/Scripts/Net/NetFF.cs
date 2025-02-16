using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetFF : NetMessage
{
    public int TeamId;

    public NetFF()  //Creating the "box"
    {
        Code = OpCode.FF;
    }
    public NetFF(DataStreamReader reader) // Receiving the "box"
    {
        Code = OpCode.FF;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(TeamId);


    }
    public override void Deserialize(DataStreamReader reader)
    {
        TeamId = reader.ReadInt();

    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_FF?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_FF?.Invoke(this, cnn);
    }
}

