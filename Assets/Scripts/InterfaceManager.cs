using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterfaceManager : MonoBehaviour
{
    public NetManager netManager;
    public ShowManager showManager;

    public TMP_Text ErrorText;

    // Connection Screen
    public TMP_InputField IPField;
    public TMP_InputField PortField;
    public GameObject ConnectionPanel;

    // Interface Screen
    public GameObject ActionPanel;
    public TMP_Text ShowTitle;

    // Action Screen
    public GameObject ActionMenu;
    public GameObject CueMenu;

    // Action Screen
    public TMP_Dropdown ManagerList;
    public TMP_Dropdown ActionList;
    public TMP_Dropdown EffectActionList;


    public void StartConnection()
    {

        string ShowName = showManager.ConnectToRenderer(IPField.text, int.Parse(PortField.text));

        if (ShowName.StartsWith("Error:"))
        {
            ErrorText.text = ShowName;
            return;
        }

        ConnectionPanel.SetActive(false);
        ActionPanel.SetActive(true);

        ShowTitle.text = ShowName;

        List<string> Managers = showManager.GetManagers();
        ManagerList.AddOptions(Managers);
    }

    public void SendAction()
    {
        if (netManager != null && showManager != null)
        {
            string Manager = ManagerList.options[ManagerList.value].text;
            string Action = ActionList.options[ActionList.value].text;

            string EffectAction = "";
            if (EffectActionList.options.Count > 0)
            {
                EffectAction = EffectActionList.options[EffectActionList.value].text;
            }

            showManager.SendAction(Manager, Action, EffectAction);
        }
    }

    public void OnManagerChanged(int change)
    {
        string CurrentManager = ManagerList.options[ManagerList.value].text;
        if (!string.IsNullOrEmpty(CurrentManager))
        {
            ActionList.ClearOptions();
            List<string> Effects = new List<string>();
            foreach (var Effect in showManager.GetManagerEffects(CurrentManager))
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
            List<string> Effects = new List<string>();
            foreach (var Effect in showManager.GetEffectActions(CurrentManager, CurrentEffect))
            {
                Effects.Add(Effect);
            }
            EffectActionList.AddOptions(Effects);
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowActionMenu()
    {
        CueMenu.SetActive(false);
        ActionMenu.SetActive(true);
    }

    public void ShowCueMenu()
    {
        CueMenu.SetActive(true);
        ActionMenu.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        ActionPanel.SetActive(false);
        ConnectionPanel.SetActive(true);
        ActionMenu.SetActive(true);
        CueMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
