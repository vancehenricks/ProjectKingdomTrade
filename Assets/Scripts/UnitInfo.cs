﻿/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UnitInfo : TileInfo
{
    public string unitType;
    public Sprite sprite;
    public UnitEffect unitEffect;
    public float travelSpeed;
    public List<TileInfo> waypoints;

    public override void Initialize()
    {
        Image shade = tileCaller.shade.GetComponent<Image>();
        Image image = tileCaller.image.GetComponent<Image>();
        waypoints = new List<TileInfo>();

        image.sprite = sprite;
        shade.color = new Color(this.color.r, this.color.g, this.color.b, shade.color.a);
        shade.sprite = sprite;

        base.Initialize();
    }

    public override void End()
    {
        waypoints.Clear();
        base.End();
    }
}
