/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonTemplate : MonoBehaviour
{
    private void Start()
    {
        TileConfig tileConfig = new TileConfig();
        Tools.WriteTileConfig(tileConfig, "template.json");
    }

}
