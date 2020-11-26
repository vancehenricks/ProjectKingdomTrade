/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[SerializeField]
public class WindowData
{
    public bool visible;
    public Vector3 position;
    public bool ignorePos;

    public WindowData(GameObject obj, bool _ignorePos = false)
    {
        ignorePos = _ignorePos;
        Save(obj);
    }

    public void Save(GameObject obj)
    {
        visible = obj.activeSelf;

        if (ignorePos) return;
        position = obj.transform.position;
    }

    public void Load(GameObject obj)
    {
        obj.SetActive(visible);

        if (ignorePos) return;
        obj.transform.position = position;
    }
}


public class TransitionHandler : MonoBehaviour
{
    public static TransitionHandler _init;
    private static bool onFirstLoad = true;

    public static TransitionHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public static WindowData mainMenuData;
    public static WindowData consoleData;
    public static WindowData devInfoData;

    public GameObject mainMenu;
    public GameObject console;
    public GameObject devInfo;

    private AsyncOperation loadingOperation;

    private enum Loading
    {
        Text = 0,
        Progress
    }

    private void Awake()
    {
        if (onFirstLoad)
        {
            mainMenuData = new WindowData(mainMenu, true);
            consoleData = new WindowData(console);
            devInfoData = new WindowData(devInfo, true);
        }

        init = this;
        onFirstLoad = false;
        mainMenuData.Load(mainMenu);
        consoleData.Load(console);
        devInfoData.Load(devInfo);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void Transition()
    {
        StartCoroutine(OnLoading());
    }

    public void TransitionWithoutMenu()
    {
        mainMenuData.visible = false;
        Transition();
    }

    private IEnumerator OnLoading()
    {
        loadingOperation = SceneManager.LoadSceneAsync("sandbox", LoadSceneMode.Single);
        loadingOperation.allowSceneActivation = false;
        LoadingHandler.init.SetActive(true);
        consoleData.Save(console);
        devInfoData.Save(devInfo);

        while (loadingOperation.progress < 0.7f)
        {
            LoadingHandler.init.Set(loadingOperation.progress);
            yield return new WaitForFixedUpdate();
        }

        LoadingHandler.init.Set(1f);
        yield return new WaitForSeconds(1f);
        loadingOperation.allowSceneActivation = true;
    }
}
