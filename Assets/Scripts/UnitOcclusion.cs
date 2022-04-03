/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, May 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

public struct UnitOcclusionValues
{
    public bool enabled;
    public OcclusionValue occlusion;
    public string parentName;
    public int getSiblingIndex;
    public int childCount;
    public long tileId;
}

public class UnitOcclusion : MonoBehaviour
{
    private static UnitOcclusion _init;
    public static UnitOcclusion init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public Camera cm;
    private List<UnitOcclusionValues> result;
    private ParallelInstance<List<UnitOcclusionValues>> parallelInstance;
    private Coroutine sync;

    private void Awake()
    {
        init = this;
    }

    public void Initialize()
    {
        parallelInstance = new ParallelInstance<List<UnitOcclusionValues>>(Calculate,(
            List<UnitOcclusionValues> _result) => 
            {
                result =_result;
            }
        );

        //generatedTiles = Convert(TileList.init.generatedUnits.Values.ToList<TileInfo>());
        sync = StartCoroutine(Sync());
    }

    private void OnDestroy()
    {
        if (sync != null)
        {
            StopCoroutine(sync);
        }
    }

    private List<UnitOcclusionValues> Convert(List<TileInfo> generatedTiles)
    {
        List<UnitOcclusionValues> list = new List<UnitOcclusionValues>();

        foreach(TileInfo tile in generatedTiles)
        {
            UnitInfo unitInfo = tile as UnitInfo;
            if(unitInfo == null) continue;
            UnitOcclusionValues unitValues = new UnitOcclusionValues()
            {
                occlusion = new OcclusionValue(cm.WorldToScreenPoint(tile.transform.position),
                new Vector2Int(cm.pixelWidth, cm.pixelHeight), TileOcclusion.init.overflow),
                getSiblingIndex = tile.transform.GetSiblingIndex(),
                childCount = tile.transform.parent.childCount,
                parentName = tile.transform.parent.name,
                tileId = tile.tileId,
                enabled = unitInfo.unitEffect.imageImage.enabled,
            };


            list.Add(unitValues);
        }

        return list;
    }    

    //seperate thread+
    private void Calculate(System.Action<List<UnitOcclusionValues>> result, List<UnitOcclusionValues> list)
    {
        List<UnitOcclusionValues> newList = new List<UnitOcclusionValues>();

        for(int i = 0;i < list.Count;i++)
        {
            UnitOcclusionValues newValue = list[i];
            if (Tools.IsWithinCameraView(list[i].occlusion))
            {
                if (list[i].getSiblingIndex == list[i].childCount - 1)
                {
                    newValue.enabled = true;
                }
                else if (list[i].parentName != "Grid")
                {        
                    newValue.enabled = false;
                }
            }
            else 
            {
                newValue.enabled = false;
            }


            if(newValue.enabled != list[i].enabled)
            {
                newList.Add(newValue);
            }
        }

        result(newList);
    }
    //seperate thread-

    private IEnumerator Sync()
    {

        while (true)
        {
            //CDebug.Log(nameof(UnitOcclusion), "transform.GetSiblingIndex()=" + unitValues.getSiblingIndex + 
            //"transform.parent.childCount=" + unitValues.childCount + "unitValues.enabled=" + unitValues.enabled, LogType.Warning);
            
            Task task = parallelInstance.Start(Convert(TileList.init.generatedUnits.Values.ToList<TileInfo>()));
            while(!task.IsCompleted)
            {
                yield return null;
            }  

            foreach (UnitOcclusionValues unitValues in result)
            {
                TileInfo tileInfo;

                TileList.init.generatedUnits.TryGetValue(unitValues.tileId, out tileInfo);

                if(tileInfo == null) continue;

                UnitInfo unitInfo = tileInfo as UnitInfo;

                unitInfo.unitEffect.imageImage.enabled = unitValues.enabled;
                unitInfo.unitEffect.shadeImage.enabled = unitValues.enabled;
            }


            
            yield return null;
        }
        
    }
}
