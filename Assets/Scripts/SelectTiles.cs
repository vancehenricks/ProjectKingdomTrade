/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectTiles : MonoBehaviour
{
    public OpenRightClick openRightClick;
    public float zLevelFlag;
    public float zLevelLine;
    public GameObject baseSelect;
    public Dictionary<string, GameObject> flags;
    public List<GameObject> exclude;

    public enum Obj
    {
        Image = 0, Value = 0
    }

    protected void Start()
    {
        MultiSelect.init.onSelectedChange += OnSelectedChange;
        Initialize();
    }

    public void Initialize() //declare here if wish to also be called by the child class
    {
        flags = new Dictionary<string, GameObject>();
    }

    protected virtual void OnDestroy()
    {
        RemoveAllFlag();
    }

    private void OnSelectedChange(List<TileInfo> tileInfos)
    {
        if (openRightClick.include.Count > 0) return;

        if (tileInfos.Count != 0)
        {
            SetAllVisibleFlags(false);
            RemoveAllFlag();
        }

        foreach (TileInfo tile in tileInfos)
        {
            SetVisibleFlags(tile, true);
            Select(tile);
        }
    }

    public void Select(TileInfo tile)
    {
        if (tile.tileType == "Edge") return;

        if (tile.tileType == "Unit")
        {
            DrawAndSyncFlag(tile, tile, baseSelect, false);
            UnitInfo unitInfo = (UnitInfo)tile;
            unitInfo.unitEffect.ResetDisplay(unitInfo.standingTile);
            tile.transform.SetAsLastSibling();
            //issue with image is being treated as one instance;

        }
        else
        {
            DrawFlag(tile, tile, baseSelect, false);
            //tile.transform.SetAsLastSibling();
        }
    }

    public GameObject InitializeFlag(TileInfo hostTile, GameObject bFlag, bool syncColor)
    {
        GameObject flag = Instantiate(bFlag);
        GenericObjectHolder objectHolder = flag.GetComponent<GenericObjectHolder>();

        Image image = objectHolder.images[(int)Obj.Image];
        TextMeshProUGUI value = objectHolder.texts[(int)Obj.Value];

        if (syncColor)
        {
            image.color = hostTile.playerInfo.color;
        }
        value.text = (flags.Count + 1) + "";

        flag.transform.SetParent(bFlag.transform.parent);
        flag.transform.SetAsFirstSibling();

        return flag;
    }

    public void DrawFlag(TileInfo hostTile, TileInfo waypoint, GameObject bFlag, bool syncColor = true)
    {
        DrawFlag(hostTile, waypoint, bFlag, syncColor, true, false);
    }

    public void DrawAndSyncFlag(TileInfo hostTile, TileInfo waypoint, GameObject bFlag, bool syncColor = true)
    {
        DrawFlag(hostTile, waypoint, bFlag, syncColor, false, true);
    }

    public void DrawFlag(TileInfo hostTile, TileInfo waypoint, GameObject bFlag, bool syncColor = true, bool repeatable = false, bool autoSync = false)
    {
        int salt = (int)Random.Range(0f, 1000f);

        if (repeatable && flags.ContainsKey(waypoint.tileLocation + "," + salt)) return;
        if (!repeatable && flags.ContainsKey(waypoint.tileId + "," + hostTile.tileId)) return;

        GameObject flag = InitializeFlag(hostTile, bFlag, syncColor);

        SyncIcon syncIcon = flag.GetComponent<SyncIcon>();
        syncIcon.Initialize(waypoint, 0, 0, zLevelFlag);
        string id = waypoint.tileId + "," + hostTile.tileId;

        if (repeatable)
        {
            id = waypoint.tileLocation + "," + salt;
        }

        flag.SetActive(true);

        syncIcon.Sync(hostTile.selected && autoSync);
        syncIcon.SetActive(hostTile.selected);

        flags.Add(id, flag);
        CDebug.Log(this, "flags added=" + flags.Count);
    }

    public void RemoveFlag(TileInfo hostTile, TileInfo tile)
    {
        CDebug.Log(this, "flags removed=" + flags.Count);

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
        CDebug.Log(this, "flags removed=" + flags.Count);
        foreach (var flag in flags.Values)
        {
            Destroy(flag);
        }

        flags.Clear();
    }

    public void SetAllVisibleFlags(bool visible)
    {
        foreach (var flag in flags.Values)
        {
            if (flag == null) continue;

            SyncIcon syncIcon = flag.GetComponent<SyncIcon>();
            if (syncIcon != null)
            {
                SetVisibleFlags(syncIcon._tile, visible);
            }
        }
    }

    public void SetVisibleFlags(TileInfo hostTile, bool visible)
    {
        Dictionary<string, GameObject>.ValueCollection Values = null;

        hostTile.selected = visible;

        if (hostTile.tileType == "Unit")
        {
            UnitInfo unitHost = hostTile as UnitInfo;
            if (unitHost != null && unitHost.unitEffect.unitWayPoint.flags != null)
            {
                Values = unitHost.unitEffect.unitWayPoint.flags.Values;
            }
        }
        else if (hostTile.tileEffect.UnitWayPoint != null && hostTile.tileEffect.UnitWayPoint.flags != null)
        {
            Values = hostTile.tileEffect.UnitWayPoint.flags.Values;
        }

        if (Values == null) return;

        foreach (GameObject gameObj in Values)
        {
            if (exclude.Contains(gameObj)) continue;

            SyncIcon syncIcon = gameObj.GetComponent<SyncIcon>();
            syncIcon.SetActive(visible);
            syncIcon.Sync(visible);
        }
    }

}
