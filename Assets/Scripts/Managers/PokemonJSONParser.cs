using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


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
        PokemonData pkm = new();

        foreach (var split in l) {
            string s = split.Replace("\t", "");

            ConfigureLock(s, ref writing, ref pkm);

            if (!writing)
                continue;

            string key, value;
            SplitKV(s, out key, out value);
            ParseKV(ref pkm, key, value);
        }
        
    }

    #region helpers
    private void ConfigureLock(string s, ref bool writing, ref PokemonData pkm)
    {
        if (!writing)
        {
            var start_Case = Regex.Match(s, ": {");
            if (start_Case.Success)
            {
                writing = true;
                pkm = new PokemonData();
            }
        }

        if (writing && s == "},")
        {
            writing = false;
            PokemonDataManager.Instance.TryPushData(pkm);
        }
            
    }

    private void SplitKV(string s, out string key, out string value)
    {
        int first_colon_index = s.IndexOf(':');
        key = s.Substring(0, first_colon_index);
        value = s.Substring(first_colon_index + 1);
    }


   
    public void ParseKV(ref PokemonData pkm, string key, string value)
    {
       
        if (value[value.Length-1] == ',')
            value = value.Substring(0, value.Length - 1);

        if (value[0] == ' ')
            value = value.Substring(1, value.Length - 1);


        switch (key)
        {
            case "num": pkm.id = uint.Parse(value); break;
            case "name": pkm.name = ParseString(value); break;
            case "types": pkm.types = ParseStringList(value).ToArray(); break;
            case "baseStats": pkm.baseStats = ParseStats(value); break;
            case "evos": pkm.evolutions = ParseStringList(value).ToArray(); break;
        }
    }

    private string ParseString(string s) => s.Substring(1, s.Length - 2);

    private List<string> ParseStringList(string s)
    {
        List<string> l_str = new();

        if (s[0] != '[' || s[s.Length - 1] != ']')
            throw new System.Exception($"{s} is not a string list.");

        s = s.Substring(1, s.Length - 2);

        char[] tokens = { ',' };
        l_str = new(s.Split(tokens));
     
        for(int i =0; i < l_str.Count; i++)
        {
            if (l_str[i][0] == ' ')
                l_str[i] = l_str[i].Substring(1);
            l_str[i] = l_str[i].Substring(1, l_str[i].Length - 2);
        }
            
        return l_str;
    }

    //ermm
    private Stat ParseStats(string s)
    {
        if (s[0] != '{' || s[s.Length - 1] != '}')
            throw new System.Exception($"{s} is not a string list.");

        s = s.Substring(1, s.Length - 2);

        char[] tokens = { ',' };
        char[] p_token = { ':' };

        List<string> l_str = new(s.Split(tokens));
        List<uint> s_list = new();
        
        for(int i =0; i < l_str.Count; i++)
        {
            if (l_str[i][0] == ' ')
                l_str[i] = l_str[i].Substring(1);
            string[] partitions = l_str[i].Split(p_token);
            s_list.Add(uint.Parse(partitions[1]));
        }

        return new Stat(s_list.ToArray());
    }

    #endregion helpers
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

        if (Application.isPlaying && GUILayout.Button("Read"))
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
