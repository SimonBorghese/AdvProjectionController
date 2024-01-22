using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowManager : MonoBehaviour
{
    public NetManager RenderServer;

    public Dictionary<string, Manager> SceneManagers;
    public string ConnectToRenderer(string Ip, int port)
    {
        string ShowName = RenderServer.ConnectToServer(Ip, port);

        if (ShowName.StartsWith("Error:"))
        {
            Debug.Log("Failed to connect to server!");
            return ShowName;
        }

        // Setup the Manager List
        string[] Managers = RenderServer.GetManagers();
        SceneManagers = new Dictionary<string, Manager>();

        foreach (var Manager in Managers)
        {
            SceneManagers[Manager] = new Manager(Manager, new Dictionary<string, Effect>());
        }

        foreach (var manager in SceneManagers)
        {
            string[] actions = RenderServer.GetManagerActions(manager.Key);

            foreach (string action in actions)
            {
                Effect effect = new Effect(action, null);

                effect.EffectActions = new List<string>(RenderServer.GetEffectActions(manager.Key, action));

                SceneManagers[manager.Key].Effects[action] = effect;
            }
        }

        return ShowName;
    }
    
    public List<string> GetManagers()
    {
        List<string> Managers = new List<string>();
        foreach (var manager in SceneManagers)
        {
            Managers.Add(manager.Key);
        }
        return Managers;
    }

    public List<string> GetManagerEffects(string Manager)
    {
        List<string> Effects = new List<string>();

        foreach (var effect in SceneManagers[Manager].Effects)
        {
            Effects.Add(effect.Key);
        }
        return Effects;
    }

    public List<string> GetEffectActions(string Manager, string Effect)
    {
        List<string> Actions = new List<string>();

        foreach (var action in SceneManagers[Manager].Effects[Effect].EffectActions)
        {
            Actions.Add(action);
        }
        return Actions;
    }

    public void SendAction(string Manager, string Effect, string Action)
    {
        string Message = "";
        foreach (char a in Effect)
        {
            if (a == '\0')
            {
                break;
            }
            Message += a;
        }
        Message += '>';
        foreach (char a in Action)
        {
            Message += a;
        }
        Message += '\0';

        Debug.Log("Message: " + Message);
        RenderServer.SendAction(Manager, Message);
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
