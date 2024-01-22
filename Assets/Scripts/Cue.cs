using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cue : ScriptableObject
{
    public float CueNumber;
    public Manager manager;
    public Effect effect;
    public string action;

    public Cue(float cueNumber, Manager manager, Effect effect, string action)
    {
        CueNumber = cueNumber;
        this.manager = manager;
        this.effect = effect;
        this.action = action;
    }
}
