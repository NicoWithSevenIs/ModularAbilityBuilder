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
        public void SetSprite(Sprite sprite, bool direction_facing)
        {
            if (direction_facing)
                front = sprite;
            else back = sprite;
        }
    }

    #endregion Constants

    #region Attributes
    private Dictionary<uint, SpriteHolder> sprite_pool = new();
    private readonly uint max_tries = 3;
    #endregion Attributes

    public async Task DownloadSprite(uint id, bool direction_facing = true)
    {
        string url = SPRITE_URL;

        if (!direction_facing)
            url += "back/";

        url += id.ToString() + ".png";

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        await request.SendWebRequest();

        var response = (DownloadHandlerTexture)request.downloadHandler;

        Sprite s = null;
        int current_tries = 0;

        while (s == null && current_tries < max_tries)
        {
            try
            {
                Texture2D tex = response.texture;
                tex.filterMode = FilterMode.Point;
                s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
                sprite_pool[id].SetSprite(s, direction_facing);
            }
            catch
            {
                current_tries++; 
                await Task.Delay(1000);
                Debug.Log($"Retrying. Attempt: {current_tries + 1}");
            }
        }

        if (s == null)
            Debug.Log("Aborted");
    }


    public async Task<Sprite> GetSprite(uint id, bool direction_facing = true)
    {
        SpriteHolder sprite_holder;
        bool contains = sprite_pool.TryGetValue(id, out sprite_holder);

        if (!contains)
        {
            // sprite_holder = sprite_pool[id] ??= new SpriteHolder();
            sprite_holder = new SpriteHolder();
            sprite_pool[id] = sprite_holder;
        }


        Sprite sprite = sprite_holder.GetSprite(direction_facing);
        if (!sprite)
        {
            await DownloadSprite(id, direction_facing);
            sprite = sprite_holder.GetSprite(direction_facing);
        }
            
        return sprite;
    }

}
