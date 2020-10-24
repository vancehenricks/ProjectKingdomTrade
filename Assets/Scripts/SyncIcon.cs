/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, September 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SyncIcon : MonoBehaviour
{

    public float _zLevelFlag;
    public float _xPadding;
    public float _yPadding;

    public GenericObjectHolder genericObjectHolder;

    private TileInfo _tile;
    private Image internalImage;
    private GameObject image;

    private bool start;

    public void Initialize(TileInfo tile, float xPadding = 0f, float yPadding = 0f,  float zLevelFlag = 0f)
    {
        image = tile.tileCaller.image;
        _zLevelFlag = zLevelFlag;
        _xPadding = xPadding;
        _yPadding = yPadding;

        _tile = tile;
        start = true;
    }

    private void Update()
    {
        if (!start) return;

        try
        {
            gameObject.transform.position = new Vector3(_tile.transform.position.x+_xPadding, _tile.transform.position.y+_yPadding, _zLevelFlag);

            foreach (GameObject obj in genericObjectHolder.objects)
            {
                obj.SetActive(image.activeSelf);
            }
        }
        catch
        {
            //start = false;
            Destroy(gameObject);
        }
	}
}
