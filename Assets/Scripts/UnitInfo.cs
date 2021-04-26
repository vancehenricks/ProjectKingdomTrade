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
    private UnitEffect _unitEffect;

    public UnitEffect unitEffect
    {
        get 
        {
            if (_unitEffect == null)
            {
                _unitEffect = (UnitEffect)tileEffect;
            }

            return _unitEffect;
        }
        private set
        {
            _unitEffect = value;
        }
    }

    public float travelSpeed;
    public List<TileInfo> waypoints;
    public List<TileInfo> targets;
    public TileInfo currentTarget;
    public List<TileInfo> targetted;
    public UnitInfo merge;
    public bool isEngaged;
    public int attackDistance;
    public float killChance;
    public float deathChance;
    public List<Walkable> nonWalkable;

    public override void Initialize()
    {
        waypoints = new List<TileInfo>();
        targets = new List<TileInfo>();
        targetted = new List<TileInfo>();

        SetSprite(sprite);

        base.Initialize();
    }

    public new void OnDestroy()
    {
       if (targetted != null) targetted.Clear();
       if (targets != null) targets.Clear();
       if (waypoints != null) waypoints.Clear();

        base.OnDestroy();
    }

    protected override Sprite SetSprite(Sprite sp)
    {
        if (sp != null && playerInfo != null)
        {
            Image shade = unitEffect.shadeImage;
            Image image = unitEffect.imageImage;

            image.sprite = sp;
            shade.color = new Color(playerInfo.color.r, playerInfo.color.g, playerInfo.color.b, shade.color.a);
            shade.sprite = sp;
        }

        return sp;
    }
}
