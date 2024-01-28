using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CueList : MonoBehaviour
{
    // All of the cues
    [SerializeField]
    public Dictionary<float, CueString> cues;

    // List of active CueManager items
    public List<CueManager> CurrentMenu;

    // Scenes show manager
    public ShowManager ShowManager;

    // Cue Action
    public CueManager TemplateCue;

    // The Gameobject containing the CueManager
    public GameObject CueTextTemplate;
    // The Base Location to initalize item
    public Transform BaseItemLocation;
    // The height offset for new items
    public float HeightOffset;

    private float counter = 1.0f;
    public void TestButton()
    {
        SetCue(counter, new CueString("Owo", "Uwu", "Owo"));
        counter += 1.0f;
    }

    public void AddCue()
    {
        SetCue(TemplateCue.GetCueNumber(), new CueString(TemplateCue.GetManager(), TemplateCue.GetEffect(), TemplateCue.GetAction()));
    }

    public void SetCue(float CueNumber, CueString cue)
    {
        cues.Add(CueNumber, cue);
        UpdateList();
        //GameObject NewText = Instantiate(CueTextTemplate,)
    }

    public void UpdateList()
    {
        foreach (var CueItem in CurrentMenu)
        {
            Destroy(CueItem.gameObject);
        }
        CurrentMenu.Clear();

        foreach (var BaseCue in cues)
        {
            if (CurrentMenu.Count == 0) {
                GameObject CueItem = Instantiate(CueTextTemplate, BaseItemLocation);

                CueManager CueManager = CueItem.GetComponent<CueManager>();
                if (CueManager != null)
                {
                    CueManager.UpdateManager();
                    CueManager.FromCueString(BaseCue.Key, BaseCue.Value);
                    CurrentMenu.Add(CueManager);
                }
            }
            else
            {
                GameObject CueItem = Instantiate(CueTextTemplate, BaseItemLocation);
                CueItem.transform.position = CurrentMenu.ToArray()[CurrentMenu.Count - 1].transform.position - new Vector3(0.0f, HeightOffset, 0.0f);

                CueManager CueManager = CueItem.GetComponent<CueManager>();
                if (CueManager != null)
                {
                    CueManager.UpdateManager();
                    CueManager.FromCueString(BaseCue.Key, BaseCue.Value);
                    CurrentMenu.Add(CueManager);
                }
            }
        }
    }

    public void RemoveCue(float OldCue)
    {
        UpdateList();
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

    public void SaveToFile()
    {
        var StringCSV = new StringBuilder();

        foreach (var cue in cues)
        {
            string Manager = cue.Value.Manager.Substring(0, cue.Value.Manager.IndexOf('\0'));
            string Effect = cue.Value.Effect.Substring(0, cue.Value.Effect.IndexOf('\0'));
            string Action = "";
            if (!string.IsNullOrEmpty(cue.Value.Action) && cue.Value.Action.IndexOf('\0') >= 0)
            {
                Action = cue.Value.Action.Substring(0, cue.Value.Action.IndexOf('\0'));
            }
            var newLine = string.Format("{0},{1},{2},{3}", cue.Key.ToString(), Manager, Effect, Action);
            StringCSV.AppendLine(newLine);
        }

        File.WriteAllText(Application.persistentDataPath + "/" + ShowManager.ShowName + ".csv", StringCSV.ToString());
    }

    public void ReadFromFile()
    {
        string ShowData = File.ReadAllText(Application.persistentDataPath + "/" + ShowManager.ShowName + ".csv");

        string[] SpecificCues = ShowData.Split('\n');
        foreach (var SpecificCue in SpecificCues)
        {
            Debug.Log("CUE+ " + SpecificCue);
            string[] CueElements = SpecificCue.Split(',');
            if (CueElements.Length <= 2) { continue;  }
            string Manager = CueElements[1];
            string Effect = CueElements[2];
            string Action = CueElements[3];
            CueElements[1].Trim();
            SetCue(float.Parse(CueElements[0]), new CueString(Manager, Effect, Action));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        cues = new Dictionary<float, CueString>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
