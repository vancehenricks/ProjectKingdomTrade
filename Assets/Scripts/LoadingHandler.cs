/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingHandler : MonoBehaviour
{
    public GameObject loadingScreen;
    public Text label;
    public Image progessBar;

    private static LoadingHandler _init;
    public static LoadingHandler init
    {
        get
        {
            return _init;
        }
        private set
        {
            _init = value;
        }
    }

    private void Awake()
    {
        init = this;
    }

    public void SetActive(bool visible)
    {
        loadingScreen.SetActive(visible);
        Set(0);
    }

    public void Set(float _progress)
    {
        label.text = (int)(_progress * 100) + "%";
        progessBar.fillAmount = _progress;
    }
}
