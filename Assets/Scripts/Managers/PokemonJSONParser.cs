using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


public struct PokemonData
{
    public uint id;
    public string name;
}

public class PokemonJSONParser : MonoBehaviour
{
    #region constants
    public static readonly string JSON_URL = "https://raw.githubusercontent.com/smogon/pokemon-showdown/master/data/pokedex.ts";
    #endregion

    #region attributes 
    [SerializeField] internal bool download_lock = true;
    #endregion

    public async Task DownloadText() { 
        UnityWebRequest web_request = new UnityWebRequest(JSON_URL, "GET");
        web_request.downloadHandler = new DownloadHandlerBuffer();
        await web_request.SendWebRequest();

        File.WriteAllText($"{Application.dataPath}/text.txt", web_request.downloadHandler.text);
    }

    public void ReadAllText()
    {
        string text;
        List<string> l;
        char[] tokens = {'\r', '\n'};

        text = File.ReadAllText($"{Application.dataPath}/text.txt");
        l = new(text.Split(tokens));

        bool writing = false;
        var pkm = new PokemonData();

        foreach (var split in l) {
            string s = split.Replace("\t", "");
            ConfigureLock(s, ref writing);

            if (!writing)
                continue;

            string key, value;
            SplitKV(s, out key, out value);
            ParseKV(ref pkm, key, value);
        }
    }

    #region helper
    private void ConfigureLock(string s, ref bool writing)
    {
        if (!writing)
        {
            var start_Case = Regex.Match(s, ": {");
            if (start_Case.Success)
                writing = true;
        }

        if (writing && s == "},")
            writing = false;
    }

    private void SplitKV(string s, out string key, out string value)
    {
        int first_colon_index = s.IndexOf(':');
        key = s.Substring(0, first_colon_index);
        value = s.Substring(first_colon_index + 1);
    }


    public void ParseKV(ref PokemonData pkm, string key, string value)
    {
        switch (key)
        {
            case "num": pkm.id = uint.Parse(value); break;
            case "name": pkm.name = value; break;
        }
    }

    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(PokemonJSONParser))]
public class TextDownloaderEditor : Editor
{
    public override async void OnInspectorGUI()
    {
        var script = (PokemonJSONParser)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Download") && !script.download_lock)
        {
            await script.DownloadText();
            script.download_lock = true;
        }

        if (GUILayout.Button("Read"))
        {
            script.ReadAllText();
        }
    }
}

#endif

/*
//For Class Mon
private static readonly Dictionary<string, Action<Mon, string>> lookup = new() {
    {"num", (m,v) => m.id = uint.Parse(v)},
    {"name", (m,v) => m.name = v },
};

public void ParseKV(Mon mon, string key, string value)
{
    if (lookup.TryGetValue(key, out var action))
        action(mon, value);
}
*/
