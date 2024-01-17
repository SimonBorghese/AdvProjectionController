using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Manager Action Map
    public Dictionary<string, List<string>> ManagerActions;

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
        ManagerActions = new Dictionary<string, List<string>>();
        foreach (string manager in Managers)
        {
            ListsManager.Add(manager);
            string[] actions = netManager.GetManagerActions(manager);
            List<string> ActionList = new List<string>();
            foreach (string action in actions)
            {
                ActionList.Add(action);
            }
            ManagerActions.Add(manager, new List<string>(ActionList));
        }
        ManagerList.AddOptions(ListsManager);
    }

    public void SendAction()
    {
        if (netManager != null)
        {
            string Manager = ManagerList.options[ManagerList.value].text;
            string Action = ActionList.options[ActionList.value].text;
            netManager.SendAction(Manager, Action);
        }
    }

    public void OnManagerChanged(int change)
    {
        string CurrentManager = ManagerList.options[ManagerList.value].text;
        if (!string.IsNullOrEmpty(CurrentManager))
        {
            ActionList.ClearOptions();
            ActionList.AddOptions(ManagerActions[CurrentManager]);
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
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
