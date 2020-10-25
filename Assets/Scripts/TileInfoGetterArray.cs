/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfoGetterArray : MonoBehaviour
{
    public int maxHits;
    //public bool holdList;

    public BoxCollider2D boxCollider2D;

    public List<TileInfo> tileInfos;
    public List<TileInfo> baseTiles;

    //private int overflowCount;

    private void OnTriggerEnter2D(Collider2D col)
    {

        /*if (overflowCount != 0)
        {
            overflowCount--;
            return;
        }*/

        TileInfo temp = col.GetComponent<TileInfo>();

        if (temp == null) return;

        foreach (TileInfo tile in baseTiles)
        {
            if (tile.tileType == temp.tileType && tileInfos.Count < (maxHits - 1))
            {
                tileInfos.Add(temp);
                break;
            }
            else if (tile.tileType == temp.tileType && tileInfos.Count == (maxHits - 1))
            {
                Debug.Log((maxHits - 1) + " == " + tileInfos.Count);
                tileInfos.Add(temp);
                //overflowCount = tileInfos.Count;
                boxCollider2D.enabled = false;
                break;
            }
        }
    }

    /*private void OnTriggerExit2D(Collider2D col)
    {
        if (holdList) return;

        Debug.Log("REMOVING");

        if (overflowCount != 0)
        {
            overflowCount--;
            return;
        }

        if (overflowCount == 0 && !boxCollider2D.enabled)
        {
            overflowCount = tileInfos.Count;
            boxCollider2D.enabled = true;
            return;
        }

        TileInfo temp = col.GetComponent<TileInfo>();

        if (temp == null) return;

        foreach (TileInfo tile in baseTiles)
        {
            if (tile.tileType == temp.tileType)
            {
                tileInfos.Remove(temp);
                break;
            }
        }
    }*/

    public void Clear()
    {
        //overflowCount = 0;
        boxCollider2D.enabled = true;
        tileInfos.Clear();
    }
}