/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, May 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public struct TileOcclusionValues
{
    public bool enabled;
    public Vector2Int tileLocation;
    public Vector3 worldPos;
    public OcclusionValue occlusion;
}

public class TileOcclusion : MonoBehaviour
{
    private static TileOcclusion _init;
    public static TileOcclusion init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public Camera cm;
    public int overflow;
    
    private ParallelInstance<List<TileOcclusionValues>> parallelInstance;
    private Vector3 previousPos;
    private List<TileOcclusionValues> generatedTiles;
    private List<TileOcclusionValues> resultValueList;

    private Coroutine scan;

    private void Awake()
    {
        init = this;
    }

    public void Initialize()
    {
        parallelInstance = new ParallelInstance<List<TileOcclusionValues>>(Calculate,(
            List<TileOcclusionValues> result, List<TileOcclusionValues> updated) => 
            {
                generatedTiles = updated;
                resultValueList = result;
            }
        );

        generatedTiles = Convert(TileList.init.generatedTiles.Values.ToList<TileInfo>());
        scan = StartCoroutine(Scan());
    }

    private void OnDestroy()
    {
        if (scan != null)
        {
            StopCoroutine(scan);
        }
    }

    private List<TileOcclusionValues> Convert(List<TileInfo> generatedTiles)
    {
        List<TileOcclusionValues> tileValueList = new List<TileOcclusionValues>();

        foreach(TileInfo tile in generatedTiles)
        {
            TileOcclusionValues tileValue = new TileOcclusionValues();

            tileValue.tileLocation = tile.tileLocation;
            tileValue.enabled = tile.tileEffect.imageImage.enabled;
            tileValue.worldPos = tile.transform.position;

            tileValueList.Add(tileValue);
        }

        return tileValueList;
    }


    private IEnumerator Scan()
    {

        while(true)
        {
            if (previousPos != cm.transform.position)
            {
                for (int i = 0; i < generatedTiles.Count; i++)
                {
                    TileOcclusionValues tileOcclusionValues = generatedTiles[i];

                    tileOcclusionValues.occlusion = new OcclusionValue(cm.WorldToScreenPoint(generatedTiles[i].worldPos),
                     new Vector2Int(cm.pixelWidth, cm.pixelHeight),overflow);

                    generatedTiles[i] = tileOcclusionValues;
                }

                Task task = parallelInstance.Start(generatedTiles);
                task.Wait();
                
                //while(!task.IsCompleted)
                //{
                //    yield return null;
                //}  

                foreach (TileOcclusionValues tileValue in resultValueList)
                {
                    TileInfo tileInfo;
                    TileList.init.generatedTiles.TryGetValue(tileValue.tileLocation, out tileInfo);

                    if(tileInfo == null) {
                        Initialize();
                        yield break;
                    }

                    tileInfo.tileEffect.imageImage.enabled = tileValue.enabled;

                    if (tileInfo.tileEffect.borderImage != null)
                    {
                        tileInfo.tileEffect.borderImage.enabled = tileValue.enabled;
                    }
                    
                    if (tileInfo.tileEffect.shadeImage != null)
                    {
                        tileInfo.tileEffect.shadeImage.enabled = tileValue.enabled;
                    }
                }

                previousPos = cm.transform.position;
            }

            yield return null;
        }
    }

    //seperate thread+
    private void Calculate(System.Action<List<TileOcclusionValues>, List<TileOcclusionValues>> result, List<TileOcclusionValues> tileValueList)
    {
        List<TileOcclusionValues> newTileValueList = new List<TileOcclusionValues>();

        for(int i = 0; i < tileValueList.Count;i++)
        { 
            TileOcclusionValues newValue = tileValueList[i];
            newValue.enabled = Tools.IsWithinCameraView(tileValueList[i].occlusion);

            if (newValue.enabled != tileValueList[i].enabled)
            {
                tileValueList[i] = newValue;
                newTileValueList.Add(newValue);
            }
        }

        result(newTileValueList, tileValueList);
    }
    //seperate thread-


}
