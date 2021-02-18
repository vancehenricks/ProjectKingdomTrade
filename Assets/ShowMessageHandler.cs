/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMessageHandler : MonoBehaviour
{
    public static ShowMessageHandler init;

    public ShowMessage confirmWindow;
    public ShowMessage infoWindow;

    private void Awake()
    {
        init = this;
    }
}
