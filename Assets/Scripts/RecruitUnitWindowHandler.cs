﻿/* Copyright 2022 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2022
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitUnitWindowHandler : MonoBehaviour
{
    public int[] multiplierArr = {1,5,10};
    public int multiplier;
    public Text multiplierText;
    public Text countText;
    public List<TileInfo> selectedTowns;
    public RecruitButton baseButton;
    public List<RecruitButton> buttons;

    public void Initialize()
    {
        selectedTowns = new List<TileInfo>(MultiSelect.init.selectedTiles);
    
        Cleanup();

        foreach(TileInfo town in selectedTowns)
        {
            UnitInfo townUnit = town as UnitInfo;

            if(townUnit == null) continue;

            foreach(string spawnable in townUnit.unitSpawnable)
            {
                TileInfo spawnableTile = TileConfigHandler.init.Serialize(spawnable);

                if(spawnableTile == null) continue;
                if(IsDuplicate(spawnableTile)) continue;

                RecruitButton button = Instantiate<RecruitButton>(baseButton, baseButton.transform.parent);
                button.name = baseButton.name;
                button.Initialize(spawnableTile, selectedTowns);
                //RecruitUnitHandler.init.onProgressUpdate += button.SetProgress;
                button.gameObject.SetActive(true);
                buttons.Add(button);
            }
        }

        countText.text = "(" + selectedTowns.Count + ")";
    }

    /*public void Unitialize()
    {
        countText.text = originalText;
    }*/

    public void Cleanup()
    {
        for(int i = 0;i < buttons.Count;i++)
        {
            //RecruitUnitHandler.init.onProgressUpdate -= buttons[i].SetProgress;
            Destroy(buttons[i].gameObject);
        }

        buttons.Clear();
    }

    public void Add(RecruitInfo recruitInfo)
    {
        foreach(TileInfo town in recruitInfo.towns)
        {
            if(!town.recruitInfos.ContainsKey(recruitInfo.baseInfo.subType))
            {
                Queue<RecruitInfo> recruits = new Queue<RecruitInfo>();
                town.recruitInfos.Add(recruitInfo.baseInfo.subType, recruits);
            }
            
            for(int i = 0;i < multiplierArr[multiplier];i++)
            {
                town.recruitInfos[recruitInfo.baseInfo.subType].Enqueue(recruitInfo);
            }
        }    
    }

    public void Remove(RecruitInfo recruitInfo)
    {
        foreach(TileInfo town in recruitInfo.towns)
        {
            for(int i = 0;i < multiplierArr[multiplier];i++)
            {
                if(!town.recruitInfos.ContainsKey(recruitInfo.baseInfo.subType)) continue;
                
                if(town.recruitInfos[recruitInfo.baseInfo.subType].Count == 0) break;

                town.recruitInfos[recruitInfo.baseInfo.subType].Dequeue();
            }
        }
    }

    public void OnMultiply()
    {
        if(++multiplier >= multiplierArr.Length)
        {
            multiplier = 0;
        }

        multiplierText.text = "X" + multiplierArr[multiplier];
    }

    public void OnCancelAll()
    {
        foreach(TileInfo town in selectedTowns)
        {
            foreach(var queue in town.recruitInfos.Values)
            {
                queue.Clear();
            }
        }
    }

    private void OnDestory()
    {
        Cleanup();
    }

    private bool IsDuplicate(TileInfo tileInfo)
    {
        foreach(RecruitButton button in buttons)
        {
             if(tileInfo.IsSameType(button.baseInfo)) 
             {
                return true;
             }
        }

        return false;
    }
}
