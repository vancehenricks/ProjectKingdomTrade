/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoilageGenerator : MonoBehaviour
{
    private void Start()
    {
        MapGenerator.init.Add(Generate, 20f);
    }
    public void Generate()
    {
        Debug.Log("FoilageGenerator");
    }
}
