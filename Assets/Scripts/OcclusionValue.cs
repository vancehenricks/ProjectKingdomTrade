/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, May 2021
 */
using UnityEngine;

public struct OcclusionValue
{
    readonly public Vector3 screenPos;
    readonly public Vector2Int screenSize;
    readonly public int overflow;

    public OcclusionValue(Vector3 _screenPos, Vector2Int _screenSize, int _overflow) : this()
    {
        screenPos = _screenPos;
        screenSize = _screenSize;
        overflow = _overflow;
    }

    public OcclusionValue(OcclusionValue occlusionValue)
    {
        screenPos = occlusionValue.screenPos;
        screenSize = occlusionValue.screenSize;
        overflow = occlusionValue.overflow;
    }
}