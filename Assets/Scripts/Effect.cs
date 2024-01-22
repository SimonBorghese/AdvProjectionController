using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : ScriptableObject
{
    public string EffectName;

    public List<string> EffectActions;

    public Effect()
    {
        EffectName = "UNNamed-Effect";
        EffectActions = new List<string>();
    }

    public Effect(string effectName, List<string> effectActions)
    {
        EffectName = effectName;
        EffectActions = effectActions;
    }
}
