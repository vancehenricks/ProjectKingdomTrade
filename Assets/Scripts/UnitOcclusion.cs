/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, May 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public struct UnitOcclusionValues
{
    public bool enabled;
    public OcclusionValue occlusion;
    public string parentName;
    public int getSiblingIndex;
    public int childCount;
}

public class UnitOcclusion : MonoBehaviour
{
    public Camera cm;

    private UnitInfo unitInfo;
    private UnitEffect unitEffect;

    public UnitOcclusionValues unitValues;
    private ParallelInstance<UnitOcclusionValues> parallelInstance;

    private Coroutine scan;

    private void Start()
    {
        unitInfo = GetComponent<UnitInfo>();
        unitEffect = unitInfo.unitEffect;

        unitValues.occlusion.overflow = TileOcclusion.init.overflow;
        unitValues.occlusion.screenSize = new Vector2Int(cm.pixelWidth, cm.pixelHeight);

        parallelInstance = new ParallelInstance<UnitOcclusionValues>(Calculate,
            (UnitOcclusionValues _result, UnitOcclusionValues _original) => {
            unitValues = _result;
        });

        scan = StartCoroutine(Scan());
    }

    private void OnDestroy()
    {
        if (scan != null)
        {
            StopCoroutine(scan);
        }
    }

    //seperate thread+
    private void Calculate(System.Action<UnitOcclusionValues,UnitOcclusionValues> result, UnitOcclusionValues unitValues)
    {

        if (Tools.IsWithinCameraView(unitValues.occlusion))
        {
            
            if (unitValues.getSiblingIndex == unitValues.childCount - 1)
            {
                unitValues.enabled = true;
            }
            else if (unitValues.parentName != "Grid")
            {
                unitValues.enabled = false;
            }
        }
        else
        {
            unitValues.enabled = false;
        }

        result(unitValues, unitValues);

    }
    //seperate thread-

    private IEnumerator Scan()
    {

        while (true)
        {
            unitValues.occlusion.screenPos = cm.WorldToScreenPoint(unitInfo.transform.position);
            unitValues.getSiblingIndex = transform.GetSiblingIndex();
            unitValues.childCount = transform.parent.childCount;
            unitValues.parentName = transform.parent.name;

            parallelInstance.Set(unitValues);
            Task task = new Task(parallelInstance.Calculate);
            task.Start();

            WAIT:
            if(!task.IsCompleted)
            {
                yield return null;
                goto WAIT;
            }  

            unitEffect.imageImage.enabled = unitValues.enabled;
            unitEffect.shadeImage.enabled = unitValues.enabled;
            
            yield return null;
        }
        
    }
}
