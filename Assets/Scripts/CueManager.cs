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
    void Start()
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
        return ManagerDropdown.options[ManagerDropdown.value].text;
    }

    public string GetEffect()
    {
        return EffectDropdown.options[EffectDropdown.value].text;
    }

    public string GetAction()
    {
        return ActionDropdown.options[ActionDropdown.value].text;
    }

    public float GetCueNumber()
    {
        return float.Parse(CueNumerInput.text);
    }

    // Update is called once per frame
    void Update()
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
