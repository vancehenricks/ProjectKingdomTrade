/* Copyright 2022 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2022
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public TextMeshProUGUI unit;
    public UnitInfo baseInfo;
    public List<TileInfo> towns;
    public Color normalColor;
    public Color pressedColor;
    public float fade;
    public RecruitUnitWindowHandler recruitUnitWindowHandler;
    //private readonly object recruitLock = new object();
    private Button button;
    
    private void Start()
    {
        //Tick.init.tickUpdate += TickUpdate;
        button = GetComponent<Button>();
    }

    public void Initialize(TileInfo _baseInfo, List<TileInfo> _towns)
    {
        name += "_" + _baseInfo.name;
        baseInfo = _baseInfo as UnitInfo;
        towns = _towns;
        image.sprite = baseInfo.sprite;
        unit.text = _baseInfo.unit + string.Empty;
        batch.text = Count() + string.Empty;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            recruitUnitWindowHandler.Remove(new RecruitInfo(this));
            StartCoroutine(Transition());
            //CDebug.Log(this, "Right Clicked", LogType.Warning);
            //Debug.Log("Right Clicked");
        }
        else if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            recruitUnitWindowHandler.Add(new RecruitInfo(this));
            StartCoroutine(Transition());
            //CDebug.Log(this, "Left Clicked", LogType.Warning);
        }
    }

    private IEnumerator Transition()
    {
        image.color = pressedColor;
        yield return new WaitForSeconds(fade);
        image.color = normalColor;

    }

    private void Update()
    {

        int count = Count();
        batch.text = string.Empty;

        if(count > 0)
        {
            batch.text = Count() + string.Empty;
        }
        UpdateProgress();
        /*foreach(var town in towns)
        {
            count += town.recruitInfos[baseInfo.subType].Count;
        }*/
    }

    private void OnDestory()
    {
        //Tick.init.tickUpdate -= TickUpdate;
    }

    public float diff;

    private void UpdateProgress()
    {
        float timeLeft = float.MaxValue;

        foreach(var town in towns)
        {
            if(town.recruitInfos.ContainsKey(baseInfo.subType))
            {
                Queue<RecruitInfo> queue = town.recruitInfos[baseInfo.subType];
                RecruitInfo recruitInfo = queue.Where((r) => r.timeLeft <= timeLeft).FirstOrDefault();

                if(recruitInfo != null)
                {
                    timeLeft = recruitInfo.timeLeft;
                }
            }
        }

        diff = 1-(timeLeft/baseInfo.spawnTime);
        progress.fillAmount = diff;
    }

    private int Count()
    {
        int count = 0;

        foreach(var town in towns)
        {
            if(town.recruitInfos.ContainsKey(baseInfo.subType))
            {
                count += town.recruitInfos[baseInfo.subType].Count;
            }
        }

        return count;
    }
}
