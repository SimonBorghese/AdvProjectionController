using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CueController : MonoBehaviour
{

    // The Show Manager for sending actions
    public ShowManager ShowManager;

    // The Cue List for getting cue
    public CueList CueList;

    public List<float> CueNumbers;

    public int CurrentCue;

    public TMP_Text CurrentCueText;
    // Start is called before the first frame update
    void Start()
    {
        ShowManager = FindObjectOfType<ShowManager>();

        CueList = FindObjectOfType<CueList>();

        CueNumbers = new List<float>();
    }


    public void ReloadCues()
    {
        if (ShowManager != null && CueList != null && ShowManager.ConnectedToServer()) 
        {
            CueNumbers.Clear();
            foreach (var cue in CueList.cues)
            {
                CueNumbers.Add(cue.Key);
            }
        }
    }

    public void NextCue()
    {
        SwitchCue();
        CurrentCue++;
        if (CurrentCue == CueNumbers.Count)
        {
            CurrentCue = 0;
        }
    }

    public void PrevCue()
    {
        SwitchCue();
        CurrentCue--;
        if (CurrentCue <= 0)
        {
            CurrentCue = CueNumbers.Count - 1;
        }
    }

    public void SwitchCue()
    {
        CueString CueMan = CueList.cues[CueNumbers[CurrentCue]];
        ShowManager.SendAction(CueMan.Manager, CueMan.Effect, CueMan.Action);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
