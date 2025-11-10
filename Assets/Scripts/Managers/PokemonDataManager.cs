using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonDataManager : M_Singleton<PokemonDataManager>
{
    private Dictionary<uint, PokemonData> pkm_data = new();
    private Dictionary<string, uint> pkm_lookup = new();

    public bool TryPushData(PokemonData data)
    {
        if (pkm_data.ContainsKey(data.id))
            return false;

        pkm_data[data.id] = data;
        pkm_lookup[data.name] = data.id;

        Debug.Log(JsonUtility.ToJson(data));
        return true;
    }

    #region Public Getters
    
    //pass this as read only collection
    public Dictionary<uint, PokemonData> PkmData { get => pkm_data;  }

    #endregion Public Getters

}
