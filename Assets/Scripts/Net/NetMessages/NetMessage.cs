using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetMessage
{
    public OpCode Code { set; get; }

    public virtual void Serialize(ref DataStreamWriter writer) //put the information inside the sent "box"
    {
        writer.WriteByte((byte)Code);
    }

    public virtual void Deserialize(DataStreamReader reader) //receiving the "box" and unpacking it
    {
    }

    public virtual void ReceivedOnClient()
    {
    }

    public virtual void ReceivedOnServer(NetworkConnection cnn)
    {
    }
}
