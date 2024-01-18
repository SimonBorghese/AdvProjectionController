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
    public TMP_Dropdown EffectActionList;

    // Manager Action Map
    // Dictionary:
    // [Manager : List of Effects : [Effect Name, List of Effect Actions]]
    public Dictionary<string, Dictionary<string,List<string>>> ManagerActions;

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
        ManagerActions = new Dictionary<string, Dictionary<string, List<string>>>();
        foreach (string manager in Managers)
        {
            ListsManager.Add(manager);
            string[] actions = netManager.GetManagerActions(manager);
            Dictionary<string, List<string>> ActionList = new Dictionary<string, List<string>>();
            foreach (string action in actions)
            {
                List<string> EffectActions = new List<string>();
                Debug.Log("For Action " + action + " Got Actions: ");
                foreach (string action2 in netManager.GetEffectActions(manager, action))
                {
                    Debug.Log(action2);
                    EffectActions.Add(action2);
                }
                ActionList.Add(action, EffectActions);
            }
            ManagerActions.Add(manager, new Dictionary<string, List<string>>(ActionList));
        }
        ManagerList.AddOptions(ListsManager);
    }

    public void SendAction()
    {
        if (netManager != null)
        {
            string Manager = ManagerList.options[ManagerList.value].text;
            string Action = ActionList.options[ActionList.value].text;

            string EffectAction = "";
            if (EffectActionList.options.Count > 0)
            {
                EffectAction = EffectActionList.options[EffectActionList.value].text;
            }

            string Message = "";
            foreach (char a in Action)
            {
                if (a == '\0')
                {
                    break;
                }
                Message += a;
            }
            Message += '>';
            foreach (char a in EffectAction)
            {
                Message += a;
            }
            Message += '\0';

            Debug.Log("Message: " + Message);
            netManager.SendAction(Manager, Message);
        }
    }

    public void OnManagerChanged(int change)
    {
        string CurrentManager = ManagerList.options[ManagerList.value].text;
        if (!string.IsNullOrEmpty(CurrentManager))
        {
            ActionList.ClearOptions();
            List<string> Effects = new List<string>();
            foreach (var Effect in ManagerActions[CurrentManager].Keys)
            {
                Effects.Add(Effect);
            }
            ActionList.AddOptions(Effects);
        }
    }

    public void OnEffectChanged(int change)
    {
        string CurrentManager = ManagerList.options[ManagerList.value].text;
        string CurrentEffect = ActionList.options[ActionList.value].text;
        if (!string.IsNullOrEmpty(CurrentManager))
        {
            EffectActionList.ClearOptions();
            EffectActionList.AddOptions(ManagerActions[CurrentManager][CurrentEffect]);
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
