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
    public Vector3 pos;
    public Vector3 screenPos;
    public Vector2Int tileLocation;
    public int pixelWidth;
    public int pixelHeight;
    public int overflow;
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
        StartCoroutine(Scan());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private List<TileOcclusionValues> Convert(List<TileInfo> generatedTiles)
    {
        List<TileOcclusionValues> tileValueList = new List<TileOcclusionValues>();

        foreach(TileInfo tile in generatedTiles)
        {
            TileOcclusionValues tileValue = new TileOcclusionValues();

            tileValue.tileLocation = tile.tileLocation;
            tileValue.enabled = tile.tileEffect.imageImage.enabled;
            tileValue.pixelWidth = cm.pixelWidth;
            tileValue.pixelHeight = cm.pixelHeight;
            tileValue.pos = tile.transform.position;
            tileValue.overflow = overflow;

            tileValueList.Add(tileValue);
        }

        return tileValueList;
    }


    private IEnumerator Scan()
    {
        yield return new WaitForSeconds(3f);

        while(true)
        {
            if (previousPos != cm.transform.position)
            {
                for (int i = 0; i < generatedTiles.Count; i++)
                {
                    TileOcclusionValues tileOcclusionValues = generatedTiles[i];
                    tileOcclusionValues.screenPos = cm.WorldToScreenPoint(generatedTiles[i].pos);
                    generatedTiles[i] = tileOcclusionValues;
                }

                parallelInstance.Set(generatedTiles);
                Task task = new Task(parallelInstance.Calculate);

                task.Start();
                task.Wait();

                foreach (TileOcclusionValues tileValue in resultValueList)
                {
                    TileEffect tileEffect = TileList.init.generatedTiles[tileValue.tileLocation].tileEffect;

                    tileEffect.imageImage.enabled = tileValue.enabled;

                    if (tileEffect.shadeImage != null)
                    {
                        tileEffect.borderImage.enabled = tileValue.enabled;
                    }
                    
                    if (tileEffect.shadeImage != null)
                    {
                        tileEffect.shadeImage.enabled = tileValue.enabled;
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
        TileOcclusionValues newValue = new TileOcclusionValues();

        for(int i = 0; i < tileValueList.Count;i++)
        { 

            if (tileValueList[i].screenPos.x >= -tileValueList[i].overflow &&
                tileValueList[i].screenPos.x <= tileValueList[i].pixelWidth+ tileValueList[i].overflow &&
                tileValueList[i].screenPos.y >= -tileValueList[i].overflow &&
                tileValueList[i].screenPos.y <= tileValueList[i].pixelHeight+ tileValueList[i].overflow)
            {
                newValue.enabled = true;
            }
            else
            {
                newValue.enabled = false;
                
            }

            if (newValue.enabled != tileValueList[i].enabled)
            {
                TileOcclusionValues updateValue = tileValueList[i];
                updateValue.enabled = newValue.enabled;
                tileValueList[i] = updateValue;

                newValue.tileLocation = tileValueList[i].tileLocation;
                newTileValueList.Add(newValue);
            }
        }

        result(newTileValueList, tileValueList);
    }
    //seperate thread-


}
