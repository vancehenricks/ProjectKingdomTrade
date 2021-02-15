/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PreLoaderHandler : MonoBehaviour
{
    public static PreLoaderHandler init;
    public static bool isDone;

    public List<Texture2D> textures;
    public List<GameObject> gameObjects;

    private void Start()
    {
        init = this;

        if (!isDone)
        {
            isDone = true;
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

        foreach (Texture2D texture in textures)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Sprites/" + texture.name + ".png");

            if (!File.Exists(path)) continue;

            texture.LoadImage(File.ReadAllBytes(path));

            Debug.Log($"Loaded {texture.name}");
            count++;
            progress = count / total;

            LoadingHandler.init.Set(progress);
            yield return new WaitForSeconds(0.1f);
        }

        LoadingHandler.init.Set(1f);
        yield return new WaitForSeconds(1f);

        LoadingHandler.init.SetActive(false);

    }

    public void LoadTextures()
    {
        StopAllCoroutines();
        StartCoroutine(LoadTexturesCoroutine());
    }
}
