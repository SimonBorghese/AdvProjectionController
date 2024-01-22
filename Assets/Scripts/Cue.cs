using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueString
{
    public string Manager;
    public string Effect;
    public string Action;

    public CueString(string manager, string effect, string action)
    {
        Manager = manager;
        Effect = effect;
        Action = action;
    }
}

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
