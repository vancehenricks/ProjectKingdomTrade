/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureHandler : MonoBehaviour
{
    private static TextureHandler _init;

    public static TextureHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public List<Texture2D> _textures;
    public Dictionary<string, Texture2D> textures;

    private void Awake()
    {
        init = this;
    }

    public Sprite GetSprite(string name)
    {
        if (name == "") return null;

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

            Tools.Log(this, $"Loaded {texture.name}");
        }

    }
}
