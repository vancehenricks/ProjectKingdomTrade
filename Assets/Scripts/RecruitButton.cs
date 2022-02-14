/* Copyright 2022 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2022
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecruitButton : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    //public Image border;
    public Image progress;
    public TextMeshProUGUI batch;
    public TileInfo baseInfo;
    public List<TileInfo> towns;
    public RecruitUnitWindowHandler recruitUnitWindowHandler;
    private int _batch;
    //private readonly object recruitLock = new object();

    public void Initialize(TileInfo _baseInfo, List<TileInfo> _towns)
    {
        name += "_" + _baseInfo.name;
        baseInfo = _baseInfo;
        towns = _towns;
        image.sprite = baseInfo.sprite;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {

        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            recruitUnitWindowHandler.Remove(new RecruitInfo(this));
            CDebug.Log(this, "Right Clicked", LogType.Warning);
            //Debug.Log("Right Clicked");
        }

        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            recruitUnitWindowHandler.Add(new RecruitInfo(this));
            CDebug.Log(this, "Left Clicked", LogType.Warning);
        }
    }

    public void SetProgress(float timeLeft, float originalTime)
    {
        progress.fillAmount = timeLeft/originalTime;
    }  
}
