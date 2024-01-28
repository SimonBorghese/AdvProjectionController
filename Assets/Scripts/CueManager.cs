using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CueManager : MonoBehaviour
{
    public ShowManager ShowManager;
    public TMP_InputField CueNumerInput;
    public TMP_Dropdown ManagerDropdown;
    public TMP_Dropdown EffectDropdown;
    public TMP_Dropdown ActionDropdown;

    public CueList CueList;
    // Start is called before the first frame update
    private void Start()
    {
    }
    void Awake()
    {
        // Find the show manager
        ShowManager = FindObjectOfType<ShowManager>();

        if (ShowManager == null)
        {
            Debug.Log("Failed to get Show Manager!");
        }

        CueList = FindObjectOfType<CueList>();
        if (CueList == null)
        {
            Debug.Log("Failed to get Cue List!");
        }
    }

    public void FromCueString(float CueNum, CueString BaseCue)
    {
        CueNumerInput.text = CueNum.ToString();

        //ManagerDropdown.captionText.text = BaseCue.Manager;
        
       
        UpdateManager();
        int index = 0;
        ManagerDropdown.value = 0;
        foreach (var Manager in ManagerDropdown.options)
        {
            Debug.Log("Manager: " + Manager.text + " " + BaseCue.Manager + " " + BaseCue.Manager.Equals(Manager.text.Substring(0, BaseCue.Manager.Length)));
            if (BaseCue.Manager.Equals(Manager.text.Substring(0, BaseCue.Manager.Length)))
            {
                ManagerDropdown.value = index;
                break;
            }
            index++;
        }
        //ManagerDropdown.value = ManagerDropdown.options;
        OnManagerChanged(ManagerDropdown.value);

        EffectDropdown.value = EffectDropdown.options.FindIndex(item => BaseCue.Effect.Equals(item.text.Substring(0, BaseCue.Effect.Length)));
        OnEffectChanged(ManagerDropdown.value);

        ActionDropdown.value = ActionDropdown.options.FindIndex(item => BaseCue.Action.Equals(item.text.Substring(0, BaseCue.Action.Length)));
        
        //EffectDropdown.captionText.text = BaseCue.Effect;
        //ActionDropdown.captionText.text = BaseCue.Action;
    }


    public void OnManagerChanged(int change)
    {
        string CurrentManager = ManagerDropdown.options[ManagerDropdown.value].text;
        if (!string.IsNullOrEmpty(CurrentManager))
        {
            EffectDropdown.ClearOptions();
            List<string> Effects = ShowManager.GetManagerEffects(CurrentManager);
            EffectDropdown.AddOptions(Effects);
        }
    }

    public void OnEffectChanged(int change)
    {
        string CurrentManager = ManagerDropdown.options[ManagerDropdown.value].text;
        string CurrentEffect = EffectDropdown.options[EffectDropdown.value].text;
        if (!string.IsNullOrEmpty(CurrentManager))
        {
            ActionDropdown.ClearOptions();
            List<string> Effects = ShowManager.GetEffectActions(CurrentManager, CurrentEffect);
            ActionDropdown.AddOptions(Effects);
        }
    }

    public void AddCue()
    {
        CueList.SetCue(GetCueNumber(), new CueString(GetManager(), GetEffect(), GetAction()));
    }

    public string GetManager()
    {
        if (ManagerDropdown.options.Count <= 0)
        {
            return "";
        }
        return ManagerDropdown.options[ManagerDropdown.value].text;
    }

    public string GetEffect()
    {
        if (EffectDropdown.options.Count <= 0)
        {
            return "";
        }
        return EffectDropdown.options[EffectDropdown.value].text;
    }

    public string GetAction()
    {
        if (ActionDropdown.options.Count <= 0)
        {
            return "";
        }
        return ActionDropdown.options[ActionDropdown.value].text;
    }

    public float GetCueNumber()
    {
        return float.Parse(CueNumerInput.text);
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateManager();
    }
    public void UpdateManager()
    {
        if (ManagerDropdown.options.Count <= 0 && ShowManager.ConnectedToServer())
        {
            // Assign the show manager's managers to the dropdown
            ManagerDropdown.ClearOptions();
            List<string> Managers = ShowManager.GetManagers();
            ManagerDropdown.AddOptions(Managers);
        }
    }
}
