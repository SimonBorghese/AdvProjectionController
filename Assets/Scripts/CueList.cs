using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CueList : MonoBehaviour
{
    [SerializeField]
    public Dictionary<float, CueString> cues;

    public ShowManager ShowManager;

    public void SetCue(float CueNumber, CueString cue)
    {
        cues.Add(CueNumber, cue);
    }

    public void RemoveCue(float OldCue)
    {
        cues.Remove(OldCue);
    }

    public void SwitchManager(string Manager, TMP_Dropdown EffectDropdown)
    {
        EffectDropdown.ClearOptions();
        
        List<string> Effects = ShowManager.GetManagerEffects(Manager);
        EffectDropdown.AddOptions(Effects);
    }

    public void SwitchEffect(string Manager, string Effect, TMP_Dropdown ActionDropdown)
    {
        ActionDropdown.ClearOptions();

        List<string> Effects = ShowManager.GetEffectActions(Manager, Effect);
        ActionDropdown.AddOptions(Effects);
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
