using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/*
 * 
 *#define MAX_STRING_LENGTH 255
#define MAX_MANAGERS 50
#define MAX_EXTENDED_LENGTH 65535 
 */


public class NetworkController : MonoBehaviour
{
    public TMP_InputField IP;
    public TMP_InputField Port;
    public TMP_Text ErrorText;
    public TMP_Text ShowName;
    public TMP_Dropdown ManagerDropdown;
    public TMP_Dropdown ActionDropdown;
    

    public GameObject[] ConnectItems;
    public GameObject[] ControllerItems;

    TcpClient client = null;
    NetworkStream clientStream = null;
    IPAddress serverAddr = null;

    const int MAGIC_LEN = 6; // furry\0
    const int TYPE_LEN = 1; // PacketType
    const int LEN_LEN = 2; // Unsigned Short
    const int END_LEN = 5; // yiff\0

    const string MAGIC_STRING = "furry\0";
    const string END_STRING = "yiff\0";

    const int MAX_STRING_LENGTH = 255;
    const int MAX_MANAGERS = 10;
    const int MAX_EXTENDED_LENGTH = 65535;

    const int OffsetToData = 6 + 1 + 2;

    enum PacketType
    {
        init = 0x01,
        conf = 0x02,
        cmd = 0x03,
        res = 0x04
    }

    enum Commands
    {
        Close = 0x01,
        Send_Action = 0x02,
        Get_Managers = 0x03,
        Get_Manager_Actions = 0x04,
        Upload_Cue_Backup = 0x05,
        Get_Cue_Backup = 0x06
    };

    struct Packet
    {
        public PacketType type;
        public short DataLength;
        public string Data;
    }

    private void Start()
    {
        foreach (GameObject obj in ConnectItems)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in ControllerItems)
        {
            obj.SetActive(false);
        }
    }

    private Packet ReadPacket()
    {
        Packet packet = new Packet();

        byte[] PacketData = new byte[MAX_EXTENDED_LENGTH + MAX_STRING_LENGTH];
        if (!clientStream.ReadAsync(PacketData, 0, MAX_EXTENDED_LENGTH + MAX_STRING_LENGTH).Wait(1000))
        {
            Debug.Log("Failed to read packet");
            return packet;
        }

        string magic_string = "";
        PacketType packetType = PacketType.init;
        short PacketLength = 0;
        string StageData = "";
        string end_string = "";

        int currnetByte = 0;

        for (int c = 0; c < MAGIC_LEN; c++)
        {
            magic_string += (char)PacketData[currnetByte++];
        }
        packetType = (PacketType)PacketData[currnetByte++];

        PacketLength = BitConverter.ToInt16(PacketData, currnetByte);
        currnetByte += 2;

        for (int c = 0; c < PacketLength; c++)
        {
            StageData += (char)PacketData[currnetByte++];
        }

        for (int c = 0; c < MAGIC_LEN; c++)
        {
            end_string += (char)PacketData[currnetByte++];
        }


        packet.type = packetType;
        packet.DataLength = PacketLength;
        packet.Data = StageData;


        return packet;
    }

    public void Close()
    {
        byte[] message = packetCreator(PacketType.cmd, "" + (char)Commands.Close);
        clientStream.Write(message, 0, message.Length);

        clientStream.Close();
        client.Close();

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void NextCue()
    {
        String ManagerName = ManagerDropdown.options[ManagerDropdown.value].text;
        String ManagerAction = ActionDropdown.options[ActionDropdown.value].text;

        char[] ManagerChars = new char[255];
        ManagerName.CopyTo(0, ManagerChars, 0, ManagerName.Length);

        char[] ActionChars = new char[255];
        ManagerAction.CopyTo(0, ActionChars, 0, ManagerAction.Length);

        String DataPacket = (char)Commands.Send_Action + "";
        foreach (char c in ManagerChars)
        {
            DataPacket += c;
        }
        foreach (char c in ActionChars)
        {
            DataPacket += c;
        }


        byte[] message = packetCreator(PacketType.cmd, DataPacket);
        clientStream.Write(message, 0, message.Length);
    }
    public void PrevCue()
    {
        byte[] message = packetCreator(PacketType.cmd, "" + (char)Commands.Get_Manager_Actions + "Cameras\0");
        if (!clientStream.WriteAsync(message, 0, message.Length).Wait(1000))
        {
            Debug.Log("Failed to send data!");
            return;
        }

        Packet managers = ReadPacket();
        byte NumManagers = (byte) managers.Data[0];

        string[] ManagerLists = new string[NumManagers];
        for (int m = 0; m < NumManagers; m++)
        {
            for (int c = 0; c < MAX_STRING_LENGTH; c++)
            {
                ManagerLists[m] += managers.Data[(m * (MAX_STRING_LENGTH+1)) + c];
            }
            Debug.Log("Action: " + ManagerLists[m]);
        }
    }

    public void GetManagers()
    {
        byte[] message = packetCreator(PacketType.cmd, "" + (char)Commands.Get_Managers);
        if (!clientStream.WriteAsync(message, 0, message.Length).Wait(1000))
        {
            Debug.Log("Failed to send data!");
            return;
        }

        Packet managers = ReadPacket();
        byte NumManagers = (byte)managers.Data[0];

        string[] ManagerLists = new string[NumManagers];
        List<TMP_Dropdown.OptionData> ManagersOptions = new List<TMP_Dropdown.OptionData> ();
        for (int m = 0; m < NumManagers; m++)
        {
            for (int c = 0; c < MAX_STRING_LENGTH; c++)
            {
                ManagerLists[m] += managers.Data[((m * MAX_STRING_LENGTH) + c) + 1];
            }
            ManagersOptions.Add(new TMP_Dropdown.OptionData(ManagerLists[m]));
        }
        ManagerDropdown.ClearOptions();
        ManagerDropdown.AddOptions(ManagersOptions);
    }

    public void GetActions()
    {
        byte[] message = packetCreator(PacketType.cmd, "" + (char)Commands.Get_Manager_Actions + ManagerDropdown.options[ManagerDropdown.value].text + "\0");
        if (!clientStream.WriteAsync(message, 0, message.Length).Wait(1000))
        {
            Debug.Log("Failed to send data!");
            return;
        }

        Packet managers = ReadPacket();
        byte NumActions = (byte)managers.Data[0];

        string[] ManagerLists = new string[NumActions];
        List<TMP_Dropdown.OptionData> ActionOptions = new List<TMP_Dropdown.OptionData>();
        for (int m = 0; m < NumActions; m++)
        {
            for (int c = 0; c < MAX_STRING_LENGTH; c++)
            {
                ManagerLists[m] += managers.Data[((m * MAX_STRING_LENGTH) + c)+1];
            }
            ActionOptions.Add(new TMP_Dropdown.OptionData(ManagerLists[m]));
        }
        ActionDropdown.ClearOptions();
        ActionDropdown.AddOptions(ActionOptions);
    }

    public async void ConnectToServer()
    {
        try
        {
            serverAddr = IPAddress.Parse(IP.text);
        }
        catch (Exception e)
        {
            ErrorText.text = "ERROR: INVALID IP: " + e.Message;
        }

        if (serverAddr == null)
        {
            ErrorText.text = "ERROR: INVALID IP";
            return;
        }

        try
        {
            client = new TcpClient();
            if (client.ConnectAsync(IP.text, int.Parse(Port.text)).Wait(1000))
            {
                clientStream = client.GetStream();
            }
            else
            {
                Debug.Log("Failed to connect to server!");
            }
        }
        catch (Exception e)
        {
            ErrorText.text = "ERROR: UNABLE TO CONNECT TO SERVER: " + e.Message;
            return;
        }

        if (clientStream == null)
        {
            ErrorText.text = "ERROR: UNABLE TO CONNECT TO SERVER";
            return;
        }
        byte[] message = packetCreator(PacketType.init, "\0");

        if (message.Length < 0)
        {
            ErrorText.text = "ERROR: UNABLE TO CONSTRUCT PACKAGE";
            return;
        }

        clientStream.Write(message);

        foreach (GameObject obj in ConnectItems)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in ControllerItems)
        {
            obj.SetActive(true);
        }

        byte[] SceneName = new byte[255];
        clientStream.Read(SceneName, 0, SceneName.Length);

        string magic_string = "";
        PacketType packetType = PacketType.init;
        short PacketLength = 0;
        string StageData = "";
        string end_string = "";

        int currnetByte = 0;

        for (int c = 0; c < MAGIC_LEN; c++)
        {
            magic_string += (char) SceneName[currnetByte++];
        }
        Debug.Log("Got Magic String: " + magic_string);

        packetType = (PacketType)SceneName[currnetByte++];

        Debug.Log("Got Packet Type: " + packetType);


        PacketLength = BitConverter.ToInt16(SceneName, currnetByte);
        currnetByte += 2;

        Debug.Log("Got Packet Length: " + PacketLength);

        for (int c = 0; c < PacketLength; c++)
        {
            StageData += (char)SceneName[currnetByte++];
        }

        Debug.Log("Got Stage Name: " + StageData);

        ShowName.text = "Show Name: " + StageData;

        for (int c = 0; c < MAGIC_LEN; c++)
        {
            end_string += (char)SceneName[currnetByte++];
        }

        Debug.Log("Got End String: " +  end_string);



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

        short dataLen = (short) (data.Length);

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
        byte[] message = packetCreator(PacketType.init, "UwU You Sussy Baka");
        clientStream.Write(message);

        clientStream.Close();

        client.Close();
    }
}
