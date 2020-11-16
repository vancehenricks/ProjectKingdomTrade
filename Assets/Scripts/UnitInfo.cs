/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
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
    public Sprite sprite;
    public UnitEffect unitEffect;
    public float travelSpeed;
    public List<TileInfo> waypoints;
    public List<TileInfo> targets;
    public TileInfo currentTarget;
    public bool isEngaged;
    public int attackDistance;
    public float killChance;
    public float deathChance;

    public override void Initialize()
    {
        Image shade = tileCaller.shade.GetComponent<Image>();
        Image image = tileCaller.image.GetComponent<Image>();
        waypoints = new List<TileInfo>();

        targets = new List<TileInfo>();
        image.sprite = sprite;
        shade.color = new Color(this.color.r, this.color.g, this.color.b, shade.color.a);
        shade.sprite = sprite;

        base.Initialize();
    }

    public override void End()
    {
        targets.Clear();
        waypoints.Clear();
        base.End();
    }
}
