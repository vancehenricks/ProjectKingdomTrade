/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureHandler : MonoBehaviour
{
    private static TextureHandler _init;

    public Dictionary<string, Sprite> sprites;

    public static TextureHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public List<Texture2D> _textures;
    public Dictionary<string, Texture2D> textures;

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

        return sprite;
    }

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

            if (!File.Exists(path)) continue;

            texture.LoadImage(File.ReadAllBytes(path));

            CDebug.Log(this, $"Loaded={texture.name}");
        }

    }

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
}
