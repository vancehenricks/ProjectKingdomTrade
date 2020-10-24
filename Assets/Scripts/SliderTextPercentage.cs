/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderTextPercentage : MonoBehaviour
{

    public Text text;
    public Slider slider;

    private void Start()
    {

        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        ValueChangeCheck();
    }

    public void ValueChangeCheck()
    {
        float percentage = slider.value * 100f;
        text.text = (int)percentage + "%";
    }
}
