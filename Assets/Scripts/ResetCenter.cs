/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenter : MonoBehaviour
{
    private static ResetCenter _init;
    public static ResetCenter init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public Camera cm;

    public KeyCode key;

    public Vector3 originalPos;
    public List<TileInfo> selectPos;
    public int currentIndex;

    private void Awake()
    {
        init = this;
    }
    public void Initialize()
    {
        originalPos = cm.transform.position;
        MultiSelect.init.onSelectedChange += OnSelectedChange;
        cm.transform.position = originalPos;
    }

    private void OnSelectedChange(List<TileInfo> tileInfos)
    {
        if(tileInfos.Count > 0)
        {
            selectPos = new List<TileInfo>(tileInfos);
            currentIndex = 0;
        }
    }

    private void Update()
    {

        if (InputOverride.init.GetKeyUp(key))
        {
            DoAction();
        }
    }

    public void DoAction()
    {
        if(selectPos.Count == 0)
        {
            cm.transform.position = originalPos;
        }
        else if(selectPos[currentIndex].selected)
        {
            Vector2 pos = selectPos[currentIndex].transform.position;
            cm.transform.position = new Vector3(pos.x, pos.y, cm.transform.position.z);

            for(int i = currentIndex;i < selectPos.Count;i++)
            {
                Vector2 nextPos = selectPos[i].transform.position;

                if (pos != nextPos)
                {
                    currentIndex = i;
                    break;
                }
                else
                {
                    currentIndex = 0;
                }
            }
        }
    }
}
