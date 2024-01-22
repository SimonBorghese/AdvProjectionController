using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : ScriptableObject
{
    public string ManagerName;

    public Dictionary<string, Effect> Effects;

    public Manager()
    {
        ManagerName = string.Empty;
        Effects = new Dictionary<string, Effect>();
    }

    public Manager(string managerName, Dictionary<string, Effect> effects)
    {
        ManagerName = managerName;
        Effects = effects;
    }
}
