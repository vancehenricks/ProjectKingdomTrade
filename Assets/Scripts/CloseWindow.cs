/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CloseWindow : MonoBehaviour
{
    public List<GameObject> windowList;

    public void DoClose()
    {
        foreach (GameObject window in windowList)
        {
            window.SetActive(false);
        }
    }

    public void DoDestroy()
    {
        foreach (GameObject window in windowList)
        {
            Destroy(window);
        }
    }
}
