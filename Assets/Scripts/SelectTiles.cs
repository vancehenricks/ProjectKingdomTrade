/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTiles : MonoBehaviour
{
    public OpenRightClick openRightClick;
    public float zLevelFlag;
    public float zLevelLine;
    public GameObject baseSelect;
    public Dictionary<string, GameObject> flags;

    public enum obj
    {
        Image = 0, Value
    }

    private void Start()
    {
        MultiSelect.onSelectedChange += OnSelectedChange;
        Initialize();
    }

    public void Initialize() //declare here if wish to also be called by the child class
    {
        flags = new Dictionary<string, GameObject>();
    }

    private void OnDestroy()
    {
        RemoveAllFlag();
    }

    private void OnSelectedChange(List<TileInfo> tileInfos)
    {
        if (openRightClick.include.Count > 0) return;

        if (tileInfos.Count != 0)
        {
            RemoveAllFlag();
        }

        foreach (TileInfo tile in tileInfos)
        {
            Select(tile);
        }
    }

    public void Select(TileInfo tile)
    {
        if (tile.tileType == "Edge") return;

        if (tile.tileType == "Unit")
        {
            DrawAndSyncFlag(tile, tile, baseSelect, false);
        }
        else
        {
            DrawFlag(tile, tile, baseSelect, false);
        }
    }

    public GameObject InitializeFlag(TileInfo hostTile, GameObject bFlag, bool syncColor)
    {
        GameObject flag = Instantiate(bFlag);
        GenericObjectHolder objectHolder = flag.GetComponent<GenericObjectHolder>();

        Image image = objectHolder.GetComponent<Image>((int)obj.Image);
        Text value = objectHolder.GetComponent<Text>((int)obj.Value);

        if (syncColor)
        {
            image.color = hostTile.color;
        }
        value.text = (flags.Count + 1) + "";

        flag.transform.SetParent(bFlag.transform.parent);
        flag.transform.SetAsFirstSibling();

        return flag;
    }

    public void DrawFlag(TileInfo hostTile, TileInfo waypoint, GameObject bFlag, bool syncColor = true)
    {
        int salt = (int)Random.Range(0f, 1000f);

        if (waypoint != null && flags.ContainsKey(waypoint.tileLocation + "," + salt)) return;

        GameObject flag = InitializeFlag(hostTile, bFlag, syncColor);

        Vector3 pos = waypoint.transform.position;
        flag.transform.position = new Vector3(pos.x, pos.y, zLevelFlag);
        flag.SetActive(true);

        string tileLocation = waypoint.tileLocation + "," + salt;
        flags.Add(tileLocation, flag);
    }

    public void DrawAndSyncFlag(TileInfo hostTile, TileInfo waypoint, GameObject bFlag, bool syncColor = true)
    {
        if (flags.ContainsKey(waypoint.tileId + "," + hostTile.tileId)) return;

        GameObject flag = InitializeFlag(hostTile, bFlag, syncColor);

        SyncIcon syncIcon = flag.GetComponent<SyncIcon>();
        syncIcon.Initialize(waypoint, 0, 0, zLevelFlag);
        flag.SetActive(true);

        flags.Add(waypoint.tileId + "," + hostTile.tileId, flag);
    }

    public void RemoveFlag(TileInfo hostTile, TileInfo tile)
    {
        Debug.Log("110 Destroy");

        List<string> keys = new List<string>(flags.Keys);

        string keyFormat = tile.tileLocation + ",";

        UnitInfo uInfo = tile as UnitInfo;

        if (uInfo != null && uInfo.targets.Count > 0)
        {
            keyFormat = tile.tileId + "," + hostTile.tileId;
        }

        for (int i = 0; i < flags.Count; i++)
        {
            if (flags[keys[i]] != null && keys[i].Contains(keyFormat))
            {

                Destroy(flags[keys[i]]);
                flags.Remove(keys[i]);
                break;
            }
        }
    }

    public void RemoveAllFlag()
    {
        Debug.Log("flags.Count=" + flags.Count);
        foreach (var flag in flags.Values)
        {
            Destroy(flag);
        }

        flags.Clear();
    }
}
