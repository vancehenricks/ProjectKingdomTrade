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

    public GenericObjectHolder loadingScreen;
    public GameObject mainMenu;
    public GameObject console;
    public GameObject devInfo;

    private Text text;
    private Image image;
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

    IEnumerator OnLoading()
    {
        loadingOperation = SceneManager.LoadSceneAsync("sandbox", LoadSceneMode.Single);
        loadingOperation.allowSceneActivation = false;
        text = loadingScreen.GetComponent<Text>((int)Loading.Text);
        image = loadingScreen.GetComponent<Image>((int)Loading.Progress);
        loadingScreen.gameObject.SetActive(true);
        consoleData.Save(console);
        devInfoData.Save(devInfo);

        while (loadingOperation.progress < 0.7f)
        {
            float progress = loadingOperation.progress;

            text.text = (int)(progress * 100) + "%";
            image.fillAmount = progress;
            yield return new WaitForFixedUpdate();
        }

        text.text = "100%";
        image.fillAmount = 1;
        yield return new WaitForSeconds(1f);
        loadingOperation.allowSceneActivation = true;
    }
}
