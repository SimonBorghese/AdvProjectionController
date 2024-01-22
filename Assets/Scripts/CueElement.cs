using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CueElement : MonoBehaviour
{
    public TMP_Text CueNumber;
    public TMP_Text CueManager;
    public TMP_Text CueEffect;
    public TMP_Text CueAction;

    public float cue;

    public CueList CueList;

    public void DeleteCue()
    {
        CueList.RemoveCue(cue);
    }

    public void SetCueText(float Cue, string Manager, string Effect, string Action)
    {
        cue = Cue;
        CueNumber.text = "Cue #" + Cue;
        CueManager.text = Manager;
        CueEffect.text = Effect;
        CueAction.text = Action;
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
