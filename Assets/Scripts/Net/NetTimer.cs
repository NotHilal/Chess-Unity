using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTimer : NetMessage
{
    public int TeamId;
    public int Timers;
    public int half=0;

    public NetTimer()  //Creating the "box"
    {
        Code = OpCode.TIMER;
    }
    public NetTimer(DataStreamReader reader) // Receiving the "box"
    {
        Code = OpCode.TIMER;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(TeamId);
        writer.WriteInt(Timers);
        writer.WriteInt(half);
        



    }
    public override void Deserialize(DataStreamReader reader)
    {
        TeamId = reader.ReadInt();
        Timers = reader.ReadInt();
        half = reader.ReadInt();

    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_TIMER?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_TIMER?.Invoke(this, cnn);
    }
}

