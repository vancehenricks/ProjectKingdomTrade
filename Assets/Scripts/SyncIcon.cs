/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, September 2020
 */

using DebugHandler;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SyncIcon : MonoBehaviour
{

    public string type;
    public float _zLevelFlag;
    public float _xPadding;
    public float _yPadding;

    public GenericObjectHolder genericObjectHolder;

    public TileInfo _tile;
    private Image imageImage;

    private Coroutine syncCoroutine;

    public void Initialize(TileInfo tile, float xPadding = 0f, float yPadding = 0f, float zLevelFlag = 0f)
    {
        imageImage = tile.tileEffect.imageImage;
        _zLevelFlag = zLevelFlag;
        _xPadding = xPadding;
        _yPadding = yPadding;

        _tile = tile;

        Sync();
    }

    public void Sync(bool start)
    {
        if (start)
        {
            if (syncCoroutine != null)
            {
                StopCoroutine(syncCoroutine);
            }
            syncCoroutine = StartCoroutine(SyncCoroutine());

            if (type == "Select")
            {
                _tile.selected = true;
            }
        }
        else if (syncCoroutine != null)
        {
            StopCoroutine(syncCoroutine);
        }
    }

    public void SetActive(bool active)
    {
        foreach (Image image in genericObjectHolder.images)
        {
            image.enabled = active;
        }

        foreach (TextMeshProUGUI text in genericObjectHolder.texts)
        {
            text.enabled = active;
        }
    }
    public void Destroy()
    {
        if (syncCoroutine != null)
        {
            StopCoroutine(syncCoroutine);
        }

        if (type == "Select")
        {
            _tile.selected = false;
        }

        Destroy(gameObject);
    }

    private IEnumerator SyncCoroutine()
    {
        while (_tile != null)
        {
            Sync();

            yield return null;
        }

        Destroy();
    }

    private void Sync()
    {
        gameObject.transform.position = new Vector3(_tile.transform.position.x + _xPadding, _tile.transform.position.y + _yPadding, _zLevelFlag);
        SetActive(imageImage.enabled);
    }
}
