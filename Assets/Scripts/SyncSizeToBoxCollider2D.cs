/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, October 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncSizeToBoxCollider2D : MonoBehaviour {

    public BoxCollider2D boxCollider2D;
    public RectTransform grid;
    public bool updateWidth;
    public bool updateHeight;
    public Vector2 padding;

    private void Awake()
    {
        SyncSize.doSync += DoSync;
    }

    private void DoSync ()
    {
        Vector2 newSize = boxCollider2D.size;

        if (updateWidth)
        {
            newSize = new Vector2(grid.sizeDelta.x, newSize.y);
        }

        if (updateHeight)
        {
            newSize = new Vector2(newSize.x, grid.sizeDelta.y);
        }

        newSize = new Vector2(newSize.x + padding.x, newSize.y + padding.y);

        boxCollider2D.size = newSize;
    }
}
