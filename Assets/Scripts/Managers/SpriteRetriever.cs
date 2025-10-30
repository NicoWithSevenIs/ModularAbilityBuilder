using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Threading.Tasks;


public class SpriteRetriever : Singleton<SpriteRetriever>
{
    #region Constants
    public static readonly bool FRONT = true;
    public static readonly bool BACK = false;

    public static readonly string SPRITE_URL = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/";

    private class SpriteHolder
    {
        public Sprite front = null;
        public Sprite back = null;

        public Sprite GetSprite(bool direction_facing) => direction_facing ? front : back;
    }

    #endregion Constants

    #region Attributes
    private Dictionary<uint, SpriteHolder> sprite_pool = new();
    #endregion Attributes

    public async Task DownloadSprite(uint id, bool direction_facing = true)
    {
        string url = SPRITE_URL;

        if (!direction_facing)
            url += "back/";

        url += id.ToString() + ".png";

       
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        //await request.SendWebRequest();

        var response = (DownloadHandlerTexture)request.downloadHandler;
        if (string.IsNullOrEmpty(response.error))
        {
            Texture2D tex = response.texture;
            tex.filterMode = FilterMode.Point;
            var s= Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }
    }


    public async Task<Sprite> GetSprite(uint id, bool direction_facing = true)
    {
        SpriteHolder sprite_holder;
        bool contains = sprite_pool.TryGetValue(id, out sprite_holder);

        if (!contains)
            sprite_holder = sprite_pool[id] ??= new SpriteHolder();

        Sprite sprite = sprite_holder.GetSprite(direction_facing);
        if (!sprite)
            await DownloadSprite(id, direction_facing);
        
        return sprite;
    }

}
