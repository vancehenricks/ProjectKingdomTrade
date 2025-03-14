﻿/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TileEffect : MonoBehaviour
{

    public TileInfo tileInfo;
    public GameObject border;
    public GameObject image;
    public GameObject shade;
    public Sprite borderClaimed;
    public Sprite borderUnClaimed;
    public Sprite borderConflict;
    public Sprite freezingTile;
    public Sprite autumnTile;
    public Sprite summerTile;
    public Sprite springTile;
    public float freezingTemp;
    public float autumnTemp;
    public float summerTemp;
    public UnitWayPoint UnitWayPoint;
    public UnitDisplay unitDisplay;
    public RecruitUnitHandler recruitUnitHandler;

    //public TileCollider tileCollider;
    //public int maxDaysBeforeRevert;

    //private int daysResultWinter;
    //private int daysResult;

    //private int startedDay;

    private Image _borderImage;
    public Image borderImage
    {
        get
        {
            if (_borderImage == null && border != null)
            {
                _borderImage = border.GetComponent<Image>();
            }

            return _borderImage;
        }
        private set
        {
            _borderImage = value;
        }
    }

    private Image _imageImage;
    public Image imageImage
    {
        get
        {
            if (_imageImage == null && image != null)
            {
                _imageImage = image.GetComponent<Image>();
            }

            return _imageImage;
        }
        private set
        {
            _imageImage = value;
        }
    }

    private Image _shadeImage;
    public Image shadeImage
    {
        get
        {
            if (_shadeImage == null && shade != null)
            {
                _shadeImage = shade.GetComponent<Image>();
            }

            return _shadeImage;
        }
        private set
        {
            _shadeImage = value;
        }
    }

    public virtual void Initialize()
    {
        tileInfo.tileCollider.onEnter += OnEnter;
        tileInfo.tileCollider.onExit += OnExit;
        tileInfo.tileCollider.Initialize();
    }

    protected virtual void OnDestroy()
    {
        tileInfo.tileCollider.onEnter -= OnEnter;
        tileInfo.tileCollider.onExit -= OnExit;
    }

    protected virtual void OnEnter(List<BaseInfo> tiles)
    {

    }

    protected virtual void OnExit(List<BaseInfo> tiles)
    {

    }

    public void UpdateTileEffect(CloudAction cloud, bool isExit)
    {
        if (isExit)
        {
            tileInfo.travelTime += tileInfo.dragAffectedByCloud;

            if (cloud.speedModifier < 0f)
            {
                cloud.speedModifier -= tileInfo.cloudDrag;
            }
            else
            {
                cloud.speedModifier += tileInfo.cloudDrag;
            }
        }
        else
        {
            if (tileInfo.tileType == "Edge") return;

            //daysResult = Tick.init.realDays-startedDay;
            if (springTile.name != null && tileInfo.localTemp <= summerTemp && ClimateControl.init.isSpring)
            {
                tileInfo.sprite = springTile;
                //image.sprite = Temperature.temperature;
            }

            if (summerTile.name != null && tileInfo.localTemp >= summerTemp && ClimateControl.init.isSummer)
            {
                tileInfo.sprite = summerTile;
            }

            if (autumnTile.name != null && tileInfo.localTemp <= autumnTemp && ClimateControl.init.isAutumn)
            {
                tileInfo.sprite = autumnTile;
            }

            if (freezingTile.name != null && tileInfo.localTemp <= freezingTemp && ClimateControl.init.isWinter)
            {
                tileInfo.sprite = freezingTile;
            }

            tileInfo.localTemp = Temperature.init.temperature;
            tileInfo.travelTime -= tileInfo.dragAffectedByCloud;

            if (cloud.speedModifier > 0f)
            {
                cloud.speedModifier -= tileInfo.cloudDrag;
            }
            else
            {
                cloud.speedModifier += tileInfo.cloudDrag;
            }
        }
    }

    public void UpdateTerritory(TileInfo tile)
    {
        if (tileInfo.tileType == "Edge") return;

        tileInfo.claimants.Add(tile.playerInfo);
        tile.playerInfo.claims.Add(tileInfo);

        if (border == null) return;

        //CDebug.Log(this, "source tileInfo=" + tile.tileId + "target tileInfo=" + tileInfo.tileId);

        borderImage.color = tileInfo.playerInfo.color;
        if (tileInfo.claimants.Count == 1)
        {
            borderImage.sprite = borderClaimed;
        }
        else if (tileInfo.claimants.Count > 1)
        {
            borderImage.sprite = borderConflict;
        }
        else
        {
            borderImage.sprite = borderUnClaimed;
        }

        if (tileInfo.tileType == "Town") return;

        borderImage.color /= tileInfo.claimants.Count;
        borderImage.color = new Color(borderImage.color.r, borderImage.color.g, borderImage.color.b);
    }
}
