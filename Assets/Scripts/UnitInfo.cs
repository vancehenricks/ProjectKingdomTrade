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
    public List<string> unitSpawnable; 
    public float spawnTime;     
    public TileInfo standingTile      
    {
        get
        {
            return standingTiles.Count > 0 ? standingTiles[0] : null;
        }
        set
        {
            if (standingTiles.Count > 0)
            {
                standingTiles[0] = value;
            }
            else
            {
                standingTiles.Add(value);    
            }
        }
    }

    public new void OnDestroy()
    {
       if (targetted != null) targetted.Clear();
       if (targets != null) targets.Clear();
       if (waypoints != null) waypoints.Clear();

        base.OnDestroy();
    }

    protected override void SetSprite(Sprite sp)
    {
        if (sp != null)
        {
            unitEffect.imageImage.sprite = sp;

            if (unitEffect.shadeImage != null)
            {
                unitEffect.shadeImage.sprite = TextureHandler.init.GetOutline(sp);
            }
            //shade.sprite = sp;
        }

        if (playerInfo != null)
        {
            unitEffect.shadeImage.color = new Color(playerInfo.color.r, playerInfo.color.g, playerInfo.color.b, unitEffect.shadeImage.color.a);
        }

        _sprite = sp;
    }
}
