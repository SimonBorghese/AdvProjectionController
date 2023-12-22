using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviour
{
    TcpClient client = null;
    NetworkStream clientStream = null;
    IPAddress serverAddr = null;

    const int MAGIC_LEN = 6; // furry\0
    const int TYPE_LEN = 1; // PacketType
    const int LEN_LEN = 2; // Unsigned Short
    const int END_LEN = 5; // yiff\0

    const string MAGIC_STRING = "furry\0";
    const string END_STRING = "yiff\0";

    enum PacketType
    {
        init = 0x01,
        conf = 0x02,
        cmd = 0x03,
        res = 0x04
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    byte[] string2Byte(string s)
    {
        byte[] outBytes = new byte[s.Length];
        int currentByte = 0;
        foreach (char c in s)
        {
            outBytes[currentByte++] = (byte)c;
        }

        return outBytes;
    }

    byte[] packetCreator(PacketType packetType, string data)
    {
        byte[] packet = new byte[MAGIC_LEN + TYPE_LEN + LEN_LEN + END_LEN + data.Length];
        int currentByte = 0;

        // Add magic string
        foreach (byte b in string2Byte(MAGIC_STRING))
        {
            packet[currentByte++] = b;
        }

        packet[currentByte++] = (byte)packetType;

        short dataLen = (short) (data.Length + 1);

        packet[currentByte++] = (byte)(dataLen & 0xFF);
        packet[currentByte++] = (byte) ((dataLen >> 8) & 0xFF);

        foreach (byte b in string2Byte(data))
        {
            packet[currentByte++] = b;
        }

        foreach (byte b in string2Byte(END_STRING))
        {
            packet[currentByte++] = b;
        }
        return packet;
    }

    public void SendDebug()
    {
        serverAddr = IPAddress.Parse("127.0.0.1");

        client = new TcpClient("127.0.0.1", 42069);
        clientStream = client.GetStream();
        byte[] message = packetCreator(PacketType.init, "UwU You Sussy Baka\0");
        clientStream.Write(message);

        clientStream.Close();

        client.Close();
    }
}
