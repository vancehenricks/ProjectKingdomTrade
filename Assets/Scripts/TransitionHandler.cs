/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[SerializeField]
public class WindowData
{
    public bool visible;
    public Vector3 position;

    public WindowData(GameObject obj)
    {
        Save(obj);
    }

    public void Save(GameObject obj)
    {
        visible = obj.activeSelf;
        position = obj.transform.position;
    }

    public void Load(GameObject obj)
    {
        obj.SetActive(visible);
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

    private void Awake()
    {
        if (onFirstLoad)
        {
            mainMenuData = new WindowData(mainMenu);
            consoleData = new WindowData(console);
            devInfoData = new WindowData(devInfo);
        }

        init = this;
        onFirstLoad = false;
        mainMenuData.Load(mainMenu);
        consoleData.Load(console);
        devInfoData.Load(devInfo);
    }

    public void Transition()
    {
        SceneManager.LoadScene("sandbox", LoadSceneMode.Single);
        consoleData.Save(console);
        devInfoData.Save(devInfo);
    }

    public void TransitionWithoutMenu()
    {
        mainMenuData.visible = false;
        Transition();
    }
}
