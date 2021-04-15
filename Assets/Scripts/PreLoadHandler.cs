/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreLoadHandler : MonoBehaviour
{
    public static PreLoadHandler init;
    public List<GameObject> gameObjects;

    private void Awake()
    {
        init = this;
    }

    private void Start()
    {
        LoadFiles();
        ActivateGameObjects();
    }

    public void LoadFiles()
    {
        StopAllCoroutines();
        StartCoroutine(LoadFilesCoroutine());
    }

    private IEnumerator LoadFilesCoroutine()
    {
        LoadingHandler.init.SetActive(true);
        TextureHandler.init.Load();
        LoadingHandler.init.Set(0.3f, "Loading Textures...");
        yield return new WaitForSeconds(2f);

        TileConfigHandler.init.Load();
        SettingsHandler.init.Load();
        LoadingHandler.init.Set(0.8f, "Loading Configs...");
        yield return new WaitForSeconds(2f);
        LanguageHandler.init.Load();
        TextHandler.init.Load();
        LoadingHandler.init.Set(1f, "Loading Texts...");
        yield return new WaitForSeconds(2f);
        LoadingHandler.init.SetActive(false);
    }

    private void ActivateGameObjects()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(true);
        }
    }
}
