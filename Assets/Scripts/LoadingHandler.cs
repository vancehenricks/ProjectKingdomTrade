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

    public void Set(float _progress)
    {
        StopAllCoroutines();
        StartCoroutine(LoadingAnimation(_progress, progressBar.fillAmount));
    }

    private IEnumerator LoadingAnimation(float target, float current)
    {
        for (float i = current;i <= target;i += 0.01f)
        {
            int percentage = (int)(i * 100);
            label.text = percentage + "%";

            if (percentage >= 98)
            {
                label.text = "100%";
            }

            progressBar.fillAmount = i;

            yield return null;
        }
    }
}
