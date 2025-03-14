﻿/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
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
    public Image progressBar;

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
        progressBar.fillAmount = 0f;
        label.text = "0%";
        loadingScreen.SetActive(visible);
    }

    public void Set(float _progress, string message = "")
    {
        StartCoroutine(LoadingAnimation(_progress, progressBar.fillAmount, message));
    }

    private IEnumerator LoadingAnimation(float target, float current, string message)
    {
        LanguageHandler.init.Text(ref message);

        for (float i = current;i <= target;i += 0.01f)
        {
            int percentage = Mathf.RoundToInt(i * 100);
            label.text = message + " " + percentage + "%";

            if (percentage >= 98)
            {
                label.text = message + " 100%";
            }

            progressBar.fillAmount = i;

            yield return null;
        }
    }
}
