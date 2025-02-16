using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetUpgrade : NetMessage
{
    public int type;
    public int TeamId;
    public int destinationX;
    public int destinationY;
    public NetUpgrade()  //Creating the "box"
    {
        Code = OpCode.UPGRADE;
    }
    public NetUpgrade(DataStreamReader reader) // Receiving the "box"
    {
        Code = OpCode.UPGRADE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(type);
        writer.WriteInt(TeamId);
        writer.WriteInt(destinationX);
        writer.WriteInt(destinationY);

    }
    public override void Deserialize(DataStreamReader reader)
    {
        type = reader.ReadInt();
        TeamId = reader.ReadInt();
        destinationX = reader.ReadInt();
        destinationY = reader.ReadInt();

    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_UPGRADE?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_UPGRADE?.Invoke(this, cnn);
    }
}

