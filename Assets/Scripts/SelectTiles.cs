/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */


using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct SelectTilesValue
{
    public Vector2Int tileLocation;
    public string type;
}

public class SelectTiles : MonoBehaviour
{
    public float zLevelFlag;
    //public float zLevelLine;
    public SyncIcon baseSelect;
    public Dictionary<SelectTilesValue, List<SyncIcon>> flags;
    //public List<SyncIcon> exclude;

    public enum Obj
    {
        Image = 0, Value = 0
    }

    public void Initialize() //declare here if wish to also be called by the child class
    {
        flags = new Dictionary<SelectTilesValue, List<SyncIcon>>();
    }

    public SyncIcon InitializeFlag(TileInfo hostTile, SyncIcon bFlag, bool syncColor)
    {
        GameObject flag = Instantiate(bFlag.gameObject); //improvement: instead of creating a new one lets reuse existing
        flag.name = bFlag.name + "_" + hostTile.tileId;
        SyncIcon syncIcon = flag.GetComponent<SyncIcon>();

        Image image = syncIcon.genericObjectHolder.images[(int)Obj.Image];
        TextMeshProUGUI value = syncIcon.genericObjectHolder.texts[(int)Obj.Value];

        if (syncColor)
        {
            image.color = hostTile.playerInfo.color;
        }
        value.text = (Count(bFlag) +1) + "";

        flag.transform.SetParent(bFlag.transform.parent);
        flag.transform.SetAsFirstSibling();

        return syncIcon;
    }

    public SyncIcon DrawFlag(TileInfo hostTile, TileInfo waypoint, SyncIcon bFlag, bool syncColor = true)
    {
        return DrawFlag(hostTile, waypoint, bFlag, syncColor, false);
    }

    public SyncIcon DrawAndSyncFlag(TileInfo hostTile, TileInfo waypoint, SyncIcon bFlag, bool syncColor = true)
    {
        return DrawFlag(hostTile, waypoint, bFlag, syncColor, true);
    }

    public SyncIcon DrawFlag(TileInfo hostTile, TileInfo waypoint, SyncIcon bFlag, bool syncColor = true, bool continousSync = false)
    {
        SelectTilesValue selectTilesValue = new SelectTilesValue();
        selectTilesValue.type = bFlag.type;
        selectTilesValue.tileLocation = waypoint.tileLocation;

        //this is expensive we could improve this by reploading number of flags based on maxHits then only create new one if lacking

        SyncIcon syncIcon = InitializeFlag(hostTile, bFlag, syncColor);
        syncIcon.Initialize(hostTile, waypoint, 0, 0, zLevelFlag); 

        syncIcon.gameObject.SetActive(true);
        syncIcon.continousSync = continousSync;
        syncIcon.Sync(true);
        //syncIcon.SetActive(true);

        if (flags.ContainsKey(selectTilesValue))
        {
            flags[selectTilesValue].Add(syncIcon);
        }
        else
        {
            List<SyncIcon> subLayer = new List<SyncIcon>();
            subLayer.Add(syncIcon);
            flags.Add(selectTilesValue, subLayer);
        }

        CDebug.Log(this, "flags added=" + flags.Count);

        return syncIcon;
    }

    public int Count(SyncIcon baseIcon)
    {
        List<SelectTilesValue> flagKeys = flags.Keys.ToList<SelectTilesValue>();

        int count = 0;

        foreach (SelectTilesValue key in flagKeys)
        {
            if (key.type == baseIcon.type)
            {
                count++;
            }
        }

        return count;
    }

    public void RemoveFlag(TileInfo tileInfo, SyncIcon baseIcon, int index = 0)
    {
        CDebug.Log(this, "flags removed=" + flags.Count);

        SelectTilesValue selectTilesValue = new SelectTilesValue();
        selectTilesValue.tileLocation = tileInfo.tileLocation;
        selectTilesValue.type = baseIcon.type;

        if (flags.ContainsKey(selectTilesValue))
        {
            if (flags[selectTilesValue].Count > 0)
            {
               flags[selectTilesValue][index].Destroy();
               flags[selectTilesValue].RemoveAt(index);
            }

            if (flags[selectTilesValue].Count == 0)
            {
                flags.Remove(selectTilesValue);
            }
        }
    }

    public void RemoveTypeFlag(SyncIcon baseIcon)
    {
        CDebug.Log(this, "flags removed=" + flags.Count);
        List<SelectTilesValue> flagKeys = flags.Keys.ToList<SelectTilesValue>();

        foreach (SelectTilesValue key in flagKeys)
        {
            if (key.type == baseIcon.type)
            {
                //if (baseIcon.type == "Direction")
                //{
                    //CDebug.Log(this, "flags removed=" + flags[key].Count, LogType.Warning);
                //}

                for (int i=0; i < flags[key].Count;i++)
                {
                    flags[key][i].Destroy();
                }

                flags[key].Clear();
                flags.Remove(key);
            }
        }
    }

    public void RemoveAllFlag()
    {
        CDebug.Log(this, "flags removed=" + flags.Count);
        foreach (var subLayer in flags)
        {
            for (int i=0;i < subLayer.Value.Count;i++)
            {
                if(subLayer.Value[i] == null) continue;
                subLayer.Value[i].Destroy();
            }
            subLayer.Value.Clear();
        }
        flags.Clear();
    }

    public virtual void  SetAllSyncIcon(bool visible)
    {
        foreach (List<SyncIcon> subLayer in flags.Values)
        {
            foreach (SyncIcon syncIcon in subLayer)
            {
                syncIcon.Sync(visible);
                syncIcon.SetActive(visible);
            }
        }
    }

}
