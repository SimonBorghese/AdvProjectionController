using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CueLoader : MonoBehaviour
{
    public TMPro.TMP_Dropdown Showlist;
    // Start is called before the first frame update
    void Start()
    {
        var Shows = Directory.EnumerateFiles(".", ".json");

        List<string> ShowList = new List<string>();
        foreach (string s in Shows)
        {
            ShowList.Add(s);
        }

        Showlist.AddOptions(ShowList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
