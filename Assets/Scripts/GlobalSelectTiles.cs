using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSelectTiles : SelectTiles
{

    private static GlobalSelectTiles _init;

    public static GlobalSelectTiles init
    {
        get { return _init; }
        private set { _init = value; }
    }


    //public Dictionary<string, List<SyncIcon>> parentTiles;
    public List<TileInfo> previousSelected;

    private void Awake()
    {
        init = this;
        //parentTiles = new Dictionary<string, List<SyncIcon>>();
    }

    private void Start()
    {
        MultiSelect.init.onSelectedChange += OnSelectedChange;
        Initialize();
    }

    private void OnSelectedChange(HashSet<TileInfo> tileInfos)
    {
        if (OpenRightClick.init.include.Count > 0) return;

        if (tileInfos.Count != 0)
        {
            foreach (TileInfo tile in previousSelected)
            {
                tile.selected = false;
                SetUnitSync(tile, false);
            }
            previousSelected.Clear();
            RemoveAllFlag();
        }

        foreach (TileInfo tile in tileInfos)
        {
            tile.selected = true;
            SetUnitSync(tile, true);
            Select(tile);
        }

        previousSelected.AddRange(tileInfos);
    }

    /*public void SetParentTile(TileInfo tile, SyncIcon icon)
    {
        string parentName = tile.transform.parent.name;

        if (parentTiles.ContainsKey(parentName))
        {
            foreach (SyncIcon syncIcon in parentTiles[parentName])
            {
                syncIcon.isTopLayer = false;
                icon.Sync(false);
                icon.SetActive(false);
            }

            parentTiles[parentName].Remove(icon);
            parentTiles[parentName].Add(icon);
            icon.isTopLayer = true;
            //icon.Sync(true);
            //icon.SetActive(true);
        }
        else
        {
            icon.isTopLayer = true;
            //icon.Sync(true);
            //icon.SetActive(true);
            List<SyncIcon> subLayer = new List<SyncIcon>();
            subLayer.Add(icon);
            parentTiles.Add(parentName, subLayer);
        }
    }*/

    private void SetUnitSync(TileInfo tile, bool visible)
    {
        UnitInfo unit = tile as UnitInfo;
        if (unit != null)
        {
            unit.unitEffect.UnitWayPoint.SetAllSyncIcon(visible);
        }
    }

    private void Select(TileInfo tile)
    {
        if (tile.tileType == "Edge") return;

        DrawAndSyncFlag(tile, tile, baseSelect, false);

        if (tile.tileType == "Unit")
        {
            //DrawAndSyncFlag(tile, tile, baseSelect, false);
            UnitInfo unitInfo = (UnitInfo)tile;
            unitInfo.unitEffect.ResetDisplay(unitInfo.standingTile);
            //tile.transform.SetAsLastSibling();
            //issue with image is being treated as one instance;

        }
        /*else
        {
            RemoveFlag(tile, baseSelect);
        }*/
    }

    private void OnDestroy()
    {
        MultiSelect.init.onSelectedChange -= OnSelectedChange;
        RemoveAllFlag();
    }
}
