/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */


using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

///<summary>
///Handle Textures for tiles.<br/>
///</summary>
public class TextureHandler : MonoBehaviour
{
    private static TextureHandler _init;

    private Dictionary<string, Sprite> sprites;

    public static TextureHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    [SerializeField]
    private List<Texture2D> _textures;
    private Dictionary<string, Texture2D> textures;

    private void Awake()
    {
        sprites = new Dictionary<string, Sprite>();
        init = this;
    }

    private Sprite GenerateSprite(string name)
    {
        if (name == "") return null;
        if (sprites.ContainsKey(name)) return sprites[name];


        string template = "empty";

        if (template.Contains("-icon"))
        {
            template = "empty-icon";
        }

        string path = Path.Combine(Application.streamingAssetsPath, "Sprites", name);

        Texture2D texture = Instantiate(textures[template]);

        texture.name = name;
        texture.LoadImage(File.ReadAllBytes(path));

        if (!textures.ContainsKey(name))
        {
            textures.Add(name, texture);
        }

        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        sprite.name = texture.name;
        sprites.Add(sprite.name, sprite);

        CDebug.Log(this, $"Loaded={sprite.name}");

        return sprite;
    }

    ///<summary>
    ///Load sprites in Sprites folder.<br/>
    ///</summary>
    public void Load()
    {
        textures = new Dictionary<string, Texture2D>();
        foreach (Texture2D texture in _textures)
        {
            textures.Add(texture.name, texture);
        }

        foreach (Texture2D texture in textures.Values)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Sprites", texture.name + ".png");
            string alternative = Path.Combine(Application.streamingAssetsPath, "Sprites", "mod_" + texture.name + ".png");

            if (File.Exists(alternative))
            {
                path = alternative;
            }
            else if (!File.Exists(path))
            {
                continue;
            }

            texture.LoadImage(File.ReadAllBytes(path));

            CDebug.Log(this, $"Loaded={texture.name}");
        }

    }

    ///<summary>
    ///Retrieve sprite into cache
    ///name Sprite file name e.g. tile_1.png or tile_1_9.png
    ///Returns Sprite.<br/>
    ///</summary>
    public Sprite GetSprite(string name)
    {
        if (name.Contains("_0.png"))
        {
            string normalized = name.Remove(name.Length - 7); // this might be wrong

            for (int i = 0; i < 8; i++)
            {
                GenerateSprite(normalized + "_" + i + ".png");
            }

            return sprites[name];
        }
        else
        {
            return GenerateSprite(name);
        }
    }

    ///<summary>
    ///Generate a cutout of a Sprite.
    ///_sprite Sprite to generate an outline.
    ///Returns white cutout of a type Sprite.<br/>
    ///</summary>
    public Sprite GetCutOut(Sprite _sprite)
    {
        if (sprites.ContainsKey(_sprite.name + ".outline"))
        {
            return sprites[_sprite.name + ".outline"];
        }

        Color32[] finalize = _sprite.texture.GetPixels32();

        for (int i = 0;i < finalize.Length;i++)
        {
            if (finalize[i].a != 255)
            {
                finalize[i] = Color.white;
            }
        }

        Sprite sprite = Sprite.Create(_sprite.texture, new Rect(0.0f, 0.0f, _sprite.texture.width, _sprite.texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        sprite.name = _sprite.texture.name + ".outline";
        sprites.Add(sprite.name, sprite);

        return sprite;
    }
}
