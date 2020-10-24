/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, Febuary 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitWayPoint : MonoBehaviour {

    public UnitInfo unitInfo;
    //public LineRenderer lineRenderer;
    public PathFinding pathFinding;
    public CombatHandler combatHandler;
    public GameObject moveFlag;
    public GameObject attackFlag;
    public float zLevelFlag;
    public float zLevelLine;

    //public List<TileInfo> doneWayPoints;
    public Dictionary<string, GameObject> flags;

    //public int index;
    // public bool cleaned;

    private enum obj
    {
        Image = 0 , Value
    }

    private void Start()
    {
        pathFinding.wayPointReached += WayPointReached;
        //pathFinding.newWayPointAssigned += NewWayPointAssigned;
        pathFinding.wayPointCountChange += WayPointCountChange;
        pathFinding.firstWayPointChange += FirstWayPointChange;
        combatHandler.targetCountChange += TargetCountChange;
        combatHandler.firstTargetChange += firstTargetChange;
        flags = new Dictionary<string, GameObject>();
        unitInfo.onEnd += OnEnd;
    }

    private void OnEnd()
    {
        RemoveAllFlag();
    }

    private void WayPointReached(TileInfo tileInfo)
    {
        if (tileInfo != null && flags.Count > 0 && unitInfo.targets.Count == 0)
        {
            RemoveFlag(tileInfo);
        }
    }

    private void FirstWayPointChange(TileInfo tileInfo)
    {
        RemoveAllFlag();
        DrawFlag(tileInfo, moveFlag);
    }

    private void WayPointCountChange(TileInfo tileInfo)
    {
        if (unitInfo.targets.Count > 0 || unitInfo.waypoints.Count == 1) return;

        if (tileInfo == null)
        {
            RemoveAllFlag();
            return;
        }

        DrawFlag(tileInfo, moveFlag);
    }

    private void firstTargetChange(TileInfo tileInfo)
    {
        RemoveAllFlag();
        DrawAndSyncFlag(tileInfo, attackFlag);
    }

    private void TargetCountChange(TileInfo tileInfo)
    {
        if (unitInfo.targets.Count == 1) return;

        if (tileInfo == null)
        {
            RemoveAllFlag();
            return;
        }

        DrawAndSyncFlag(tileInfo, attackFlag);
    }

    private GameObject InitializeFlag(GameObject bFlag)
    {
        GameObject flag = Instantiate(bFlag);
        GenericObjectHolder objectHolder = flag.GetComponent<GenericObjectHolder>();

        Image image = objectHolder.GetComponent<Image>((int)obj.Image);
        Text value = objectHolder.GetComponent<Text>((int)obj.Value);

        image.color = unitInfo.color;
        value.text = (flags.Count+1) + "";

        flag.transform.SetParent(bFlag.transform.parent);
        flag.transform.SetAsFirstSibling();

        return flag;
    }

    private void DrawFlag(TileInfo waypoint, GameObject bFlag)
    {
        int salt = (int)Random.Range(0f, 1000f);

        if (waypoint != null && flags.ContainsKey(waypoint.tileLocation+"," + salt)) return;

        GameObject flag = InitializeFlag(bFlag);

        Vector3 pos = waypoint.transform.position;
        flag.transform.position = new Vector3(pos.x, pos.y, zLevelFlag);
        flag.SetActive(true);

        string tileLocation = waypoint.tileLocation + "," + salt;
        flags.Add(tileLocation, flag);
    }

    private void DrawAndSyncFlag(TileInfo waypoint, GameObject bFlag)
    {
        if (flags.ContainsKey(waypoint.tileId + "," + unitInfo.tileId)) return;

        GameObject flag = InitializeFlag(bFlag);

        SyncIcon syncIcon = flag.GetComponent<SyncIcon>();
        syncIcon.Initialize(waypoint,0,0,zLevelFlag);
        flag.SetActive(true);

        flags.Add(waypoint.tileId + "," + unitInfo.tileId, flag);
    }

    private void RemoveFlag(TileInfo tile)
    {
        Debug.Log("110 Destroy");

        List<string> keys = new List<string>(flags.Keys);

        string keyFormat = tile.tileLocation + ",";

        if (tile.targets.Count > 0)
        {
            keyFormat = tile.tileId + "," + unitInfo.tileId;
        }

        for (int i = 0;i < flags.Count;i++)
        {
            if (flags[keys[i]] != null && keys[i].Contains(keyFormat))
            {

                Destroy(flags[keys[i]]);
                flags.Remove(keys[i]);
                break;
            }
        }
    }

    private void RemoveAllFlag()
    {
        Debug.Log("flags.Count=" + flags.Count);
        foreach (var flag in flags.Values)
        {
            Destroy(flag);
        }

        flags.Clear();
    }
}
