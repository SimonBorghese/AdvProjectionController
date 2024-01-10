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

    // Action Screen
    public TMP_Dropdown ManagerList;
    public TMP_Dropdown ActionList;

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

        // Setup the Manager List
        string[] Managers = netManager.GetManagers();
        List<string> ListsManager = new List<string>();
        foreach (string manager in Managers)
        {
            ListsManager.Add(manager);
        }
        ManagerList.AddOptions(ListsManager);
    }

    public void OnManagerChanged(int change)
    {
        string CurrentManager = ManagerList.options[ManagerList.value].text;
        if (!string.IsNullOrEmpty(CurrentManager))
        {
            ActionList.ClearOptions();
            string[] actions = netManager.GetManagerActions(CurrentManager);
            List<string> ListsManager = new List<string>();
            foreach (string manager in actions)
            {
                ListsManager.Add(manager);
            }
            ActionList.AddOptions(ListsManager);
        }
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
