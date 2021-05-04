/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, September 2020
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SyncIcon : MonoBehaviour
{

    public float _zLevelFlag;
    public float _xPadding;
    public float _yPadding;
    public bool alwaysOn;
    public bool syncThroughSibling;

    public GenericObjectHolder genericObjectHolder;

    public TileInfo _tile;
    private GameObject image;

    public void Initialize(TileInfo tile, float xPadding = 0f, float yPadding = 0f, float zLevelFlag = 0f)
    {
        image = tile.tileEffect.image;
        _zLevelFlag = zLevelFlag;
        _xPadding = xPadding;
        _yPadding = yPadding;

        _tile = tile;

        Sync();
    }

    public void Sync(bool start)
    {
        if (start || alwaysOn)
        {
            StartCoroutine(SyncCoroutine());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    public void SetActive(bool active)
    {
        if(alwaysOn) return;

        foreach (GameObject obj in genericObjectHolder.objects)
        {
            obj.SetActive(active);
        }
    }

    private IEnumerator SyncCoroutine()
    {
        while (true)
        {
            if (syncThroughSibling)
            {
                SyncThroughSibling();
            }
            else
            {
                Sync();
            }

            yield return null;
        }
    }

    private void Sync()
    {
        try
        {
            gameObject.transform.position = new Vector3(_tile.transform.position.x + _xPadding, _tile.transform.position.y + _yPadding, _zLevelFlag);

            foreach (GameObject obj in genericObjectHolder.objects)
            {
                obj.SetActive(image.activeSelf);
            }
        }
        catch (System.Exception e)
        {
            CDebug.Log(this,e,LogType.Warning);
            Destroy(gameObject);
        }

    }

    private void SyncThroughSibling()
    {
        try
        {
            gameObject.transform.position = new Vector3(_tile.transform.position.x + _xPadding, _tile.transform.position.y + _yPadding, _zLevelFlag);

            foreach (GameObject obj in genericObjectHolder.objects)
            {
                if (_tile.transform.GetSiblingIndex() == _tile.transform.parent.childCount - 1)
                {
                    obj.SetActive(true);
                }
                else if (_tile.transform.parent.name != "Grid")
                {
                    obj.SetActive(false);
                }
            }
        }
        catch (System.Exception e)
        {
            CDebug.Log(this, e, LogType.Warning);
            Destroy(gameObject);
        }

    }
}
