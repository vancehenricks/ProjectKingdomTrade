/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PreLoaderHandler : MonoBehaviour
{
    public static PreLoaderHandler init;

    private static bool isDone;

    public List<Texture2D> _textures;
    public Dictionary<string, Texture2D> textures;
    public List<GameObject> gameObjects;

    private void Start()
    {
        ExecuteCommands command = Initialize;
        PreLoadPipeline.Add(command, 0);
    }

    private void Initialize()
    {
        init = this;

        if (!isDone)
        {
            isDone = true;

            textures = new Dictionary<string, Texture2D>();
            foreach (Texture2D texture in _textures)
            {
                textures.Add(texture.name, texture);
            }

            LoadTextures();
        }

        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(true);
        }

    }

    private IEnumerator LoadTexturesCoroutine()
    {

        LoadingHandler.init.SetActive(true);

        float progress = 0f;
        float count = 0;
        float total = textures.Count;

        foreach (Texture2D texture in textures.Values)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Sprites/" + texture.name + ".png");

            if (!File.Exists(path)) continue;

            texture.LoadImage(File.ReadAllBytes(path));

            Debug.Log($"Loaded {texture.name}");
            count++;
            progress = count / total;

            LoadingHandler.init.Set(progress, "Loading Textures...");
            yield return new WaitForSeconds(0.1f);
        }

        LoadingHandler.init.Set(1f, "Loading Textures...");
        yield return new WaitForSeconds(1f);

        LoadingHandler.init.SetActive(false);

    }

    public Sprite GetTexture(string name)
    {
        string template = "empty.png";

        if (template.Contains("-icon"))
        {
            template = "empty-icon";
        }

        string path = Path.Combine(Application.streamingAssetsPath, "Sprites/");

        Texture2D texture = Instantiate(textures[template]);

        texture.name = name;
        texture.LoadImage(File.ReadAllBytes(path + name));
        textures.Add(texture.name, texture);

        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public void LoadTextures()
    {
        StopAllCoroutines();
        StartCoroutine(LoadTexturesCoroutine());
    }
}
