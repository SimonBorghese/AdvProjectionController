using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.Sprites;
using UnityEngine;

public class NetManager : MonoBehaviour
{

    // Constant Magic String
    const string MAGIC_STRING = "furry\0";
    const string MAGIC_STRING_CHECK = "furry";

    // The Max String Length, Managers, and Cue Length
    const int MAX_STRING_LENGTH = 255;
    const int MAX_MANAGERS = 10;
    const int MAX_EXTENDED_LENGTH = 65535;
    const int MAX_PACKET_SIZE = MAX_EXTENDED_LENGTH + OffsetToData;

    // The size of each packet element
    const int MAGIC_STRING_LEN = 6;
    const int TYPE_LEN = 1;
    const int DATA_LEN_LEN = 2;

    // The offset to the data element of the packet
    const int OffsetToData = MAGIC_STRING_LEN + TYPE_LEN + DATA_LEN_LEN;

    enum PacketType
    {
        Init = 0x01,
        Conf = 0x02,
        Cmd = 0x03,
        Res = 0x04
    }

    enum Commands
    {
        Close = 0x01,
        Send_Action = 0x02,
        Get_Managers = 0x03,
        Get_Manager_Actions = 0x04,
        Upload_Cue_Backup = 0x05,
        Get_Cue_Backup = 0x06,
        Send_RGB = 0x07
    };

    enum EActionTypes
    {
        Action = 0x01,
        RGB_Control = 0x02
    };

    // Send Action Struct
    struct FSendAction
    {
        public string Manager; // MAX_STR_LEN
        public string Action; // MAX_STR_LEN

        public override readonly string ToString()
        {
            string Out = "";
            for (int c = 0; c < MAX_STRING_LENGTH; c++)
            {
                if (c > Manager.Length)
                {
                    Out += '\0';
                }
                else
                {
                    Out += Manager[c];
                }
            }
            for (int c = 0; c < MAX_STRING_LENGTH; c++)
            {
                if (c > Action.Length)
                {
                    Out += '\0';
                }
                else
                {
                    Out += Action[c];
                }
            }

            return Out;
        }
    };

    // Get Managers Result Struct
    struct FGetManagersResult
    {
        uint NumManagers;
        string[] Managers;

        public void FromString(byte[] Data, int offset)
        {
            NumManagers = Data[offset];
            Managers = new string[NumManagers];
            for (int m = 0; m < NumManagers; m++)
            {
                Managers[m] = "";
                for (int c = 0; c < MAX_STRING_LENGTH; c++)
                {
                    // Manager Offset + Current Character + Offset for short
                    Managers[m] += (char)Data[(MAX_STRING_LENGTH * m) + c + 1 + offset];
                }
            }
        }

        public string[] ToStringArray()
        {
            return Managers;
        }
	};

    // Get Manager Actions Input
    struct FGetManagerActions
    {
        char[] Manager;
        public override readonly string ToString()
        {
            string Out = "";
            foreach (char c in Manager)
            {
                Out += c;
            }
            return Out;
        }
    };

    // Get Manager Action Results
    struct FGetManagerActionsResult
    {
        uint NumActions;
        string[] Actions;
        public void FromString(byte[] Data, int offset)
        {
            NumActions = Data[offset];
            Actions = new string[NumActions];
            Debug.Log("Got Num Actions: " + NumActions);
            for (int m = 0; m < NumActions; m++)
            {
                Actions[m] = "";
                for (int c = 0; c < MAX_STRING_LENGTH; c++)
                {
                    // Manager Offset + Current Character + Offset for short
                    Actions[m] += (char) Data[(MAX_STRING_LENGTH * m) + c + 1 + offset];
                }
                Debug.Log("got action: " + Actions[m]);
            }
        }
        public string[] ToStringArray()
        {
            return Actions;
        }
    };

    // Upload Cue Input
    struct FUploadCue
    {
        char[] CueList;
    };

    // Get Cue Result
    struct FGetCueResult
    {
        char[] CueList;
    };

    struct Packet
    {
        public string MagicString;
        public PacketType type;
        public ushort DataLength;
        public string Data;

        public Packet(PacketType pakType, ushort DataLen, string PacketData)
        {
            MagicString = MAGIC_STRING;
            type = pakType;
            DataLength = DataLen;
            Data = PacketData;
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[MAGIC_STRING_LEN + TYPE_LEN + DATA_LEN_LEN + Data.Length];
            int ByteOffset = 0;
            for (int c = 0; c < MAGIC_STRING_LEN; c++)
            {
                bytes[ByteOffset++] = (byte) MagicString[c];
            }
            bytes[ByteOffset++] = (byte)type;
            bytes[ByteOffset++] = (byte)(DataLength & 0XFF);
            bytes[ByteOffset++] = (byte)((DataLength >> 8) & 0xFF);
            for (int d = 0; d < DataLength; d++)
            {
                bytes[ByteOffset++] = (byte)Data[d];
            }
            return bytes;
        }
    }

    // Non-static members, set from the functions, we can only have 1 client
    TcpClient client = null;
    NetworkStream clientStream = null;
    IPAddress serverAddr = null;

    public string ConnectToServer(string IP, int Port)
    {
        try
        {
            serverAddr = IPAddress.Parse(IP);
        }
        catch (Exception e)
        {
            return "Error: Failed to parse IP: " + IP;
        }

        try
        {
            client = new TcpClient();
            if (client.ConnectAsync(IP, Port).Wait(1000))
            {
                clientStream = client.GetStream();
            }
            else
            {
                return "Error: Failed to connect to server for unknown reason!";
            }
        }
        catch (Exception e)
        {
            return "Error: Unable to connect to server: " + e.Message;
        }

        Packet packet = new Packet(PacketType.Init, 0, "\0");

        byte[] PacketData = packet.ToBytes();
        if (!clientStream.WriteAsync(PacketData, 0, PacketData.Length).Wait(1000))
        {
            return "Error: Failed to write data!";
        }

        Packet ResPacket = RecvPacket(clientStream);
        return ResPacket.Data;
    }
    
    public string[] GetManagers()
    {
        Packet ManagerRequest = new Packet(PacketType.Cmd, 1, "" + (char) Commands.Get_Managers);
        SendPacket(clientStream, ManagerRequest);

        Packet Managers = RecvPacket(clientStream);
        FGetManagersResult ManagerList = new FGetManagersResult();
        ManagerList.FromString(Managers.ToBytes(), OffsetToData);
        
        return ManagerList.ToStringArray();
    }

    public string[] GetManagerActions(string Manager)
    {
        Packet ActionRequest = new Packet(PacketType.Cmd, (ushort) (Manager.Length + 1), "" + (char)Commands.Get_Manager_Actions + Manager);
        SendPacket(clientStream, ActionRequest);

        Packet ActionsPacket = RecvPacket(clientStream);
        FGetManagerActionsResult ManagerResults = new FGetManagerActionsResult();
        ManagerResults.FromString(ActionsPacket.ToBytes(), OffsetToData);

        return ManagerResults.ToStringArray();
    }

    public void SendAction(string Manager, string Action)
    {
        FSendAction action = new FSendAction();
        action.Manager = Manager;
        action.Action = Action;

        string ActionData = action.ToString();
        Packet actionPacket = new Packet(PacketType.Cmd, (ushort) ActionData.Length, "" + (char)Commands.Send_Action + ActionData);
        SendPacket(clientStream, actionPacket);
    }

    static void SendPacket(NetworkStream tcpClient, Packet packet)
    {
        byte[] PacketData = packet.ToBytes();
        if (!tcpClient.WriteAsync(PacketData, 0, PacketData.Length).Wait(2000))
        {
            Debug.Log("Failed to send data!");
        }
    }

    static Packet RecvPacket(NetworkStream tcpClient)
    {
        // Initalize our packet and the buffer to read the packet
        var packet = new Packet();

        byte[] RawPacketData = new byte[MAX_PACKET_SIZE];

        if (tcpClient == null || !tcpClient.ReadAsync(RawPacketData, 0, MAX_PACKET_SIZE).Wait(5000))
        {
            Debug.Log("Failed to read packet!");
            return packet;
        }

        // Copy the magic string to the packet
        int PacketOffset = 0;
        for (int i = 0; i < MAGIC_STRING_LEN; i++)
        {
            packet.MagicString += (char) RawPacketData[PacketOffset++];
        }

        if (!packet.MagicString.StartsWith(MAGIC_STRING_CHECK))
        {
            Debug.Log("INVALID MAGIC STRING: " + packet.MagicString);
            return packet;
        }

        // Read the type and data length from the packet
        packet.type = (PacketType)RawPacketData[PacketOffset++];

        packet.DataLength = BitConverter.ToUInt16(RawPacketData, PacketOffset);

        // Move the offset up by 2 as that's the length of the short
        PacketOffset += DATA_LEN_LEN;

        // Read all of the data and put it into a string
        for (int i = 0; i < packet.DataLength; i++)
        {
            packet.Data += (char) RawPacketData[PacketOffset++];
        }

        // All of the data has been read successfully, return
        return packet;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
