/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsShow : MonoBehaviour
{

    public Text fps;
    public Text ms;

    private void Update()
    {
        string _ms = string.Format("{0:0.##}", Time.deltaTime * 1000.0f);
        string _fps = string.Format("{0:0}", 1.0f / Time.deltaTime);
        ms.text = _ms;
        fps.text = _fps;
    }
}
