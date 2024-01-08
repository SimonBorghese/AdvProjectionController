using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public NetManager netManager;

    // Connection Screen
    public TMP_InputField IPField;
    public TMP_InputField PortField;
    public GameObject ConnectionPanel;

    // Interface Screen
    public GameObject ActionPanel;
    public TMP_Text ShowTitle;

    public void StartConnection()
    {
        string ShowName = netManager.ConnectToServer(IPField.text, int.Parse(PortField.text));

        if (ShowName.StartsWith("Error:"))
        {
            Debug.Log("Failed to connect to server!");
            return;
        }

        ConnectionPanel.SetActive(false);
        ActionPanel.SetActive(true);

        ShowTitle.text = ShowName;
    }
    // Start is called before the first frame update
    void Start()
    {
        ActionPanel.SetActive(false);
        ConnectionPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
