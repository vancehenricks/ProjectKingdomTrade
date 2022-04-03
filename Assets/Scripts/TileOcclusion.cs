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
    //public Vector3 worldPos;
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
    private List<TileOcclusionValues> result;

    private Coroutine sync;

    private void Awake()
    {
        init = this;
    }

    public void Initialize()
    {
        parallelInstance = new ParallelInstance<List<TileOcclusionValues>>(Calculate,(
            List<TileOcclusionValues> _result) => 
            {
                result = _result;
            }
        );

        sync = StartCoroutine(Sync());
    }

    private void OnDestroy()
    {
        if (sync != null)
        {
            StopCoroutine(sync);
        }
    }

    private List<TileOcclusionValues> Convert(List<TileInfo> generatedTiles)
    {
        List<TileOcclusionValues> tileValueList = new List<TileOcclusionValues>();

        foreach(TileInfo tile in generatedTiles)
        {
            TileOcclusionValues tileValue = new TileOcclusionValues()
            {
                tileLocation = tile.tileLocation,
                enabled = tile.tileEffect.imageImage.enabled,
                occlusion = new OcclusionValue(cm.WorldToScreenPoint(tile.transform.position),
                new Vector2Int(cm.pixelWidth, cm.pixelHeight),overflow),
            };

            tileValueList.Add(tileValue);
        }

        return tileValueList;
    }


    private IEnumerator Sync()
    {

        while(true)
        {
            if (previousPos != cm.transform.position)
            {
                Task task = parallelInstance.Start(Convert(TileList.init.generatedTiles.Values.ToList<TileInfo>()));
                task.Wait();

                foreach (TileOcclusionValues tileValue in result)
                {
                    TileInfo tileInfo;
                    TileList.init.generatedTiles.TryGetValue(tileValue.tileLocation, out tileInfo);

                    if(tileInfo == null) yield return null;

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
    private void Calculate(System.Action<List<TileOcclusionValues>> result, List<TileOcclusionValues> list)
    {
        List<TileOcclusionValues> newTileValueList = new List<TileOcclusionValues>();

        for(int i = 0; i < list.Count;i++)
        { 
            TileOcclusionValues newValue = list[i];
            newValue.enabled = Tools.IsWithinCameraView(list[i].occlusion);

            if (newValue.enabled != list[i].enabled)
            {
                list[i] = newValue;
                newTileValueList.Add(newValue);
            }
        }

        result(newTileValueList);
    }
    //seperate thread-


}
